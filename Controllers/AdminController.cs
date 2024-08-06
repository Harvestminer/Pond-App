using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PondWebApp.Domain;
using PondWebApp.Models;
using PondWebApp.Services;

namespace PondWebApp.Controllers
{
	public class AdminController(ILogger<LoginController> logger, MemberService memberService, MinecraftService minecraftService) : Controller
	{
		private readonly ILogger<LoginController> _logger = logger;
		private readonly MemberService _memberService = memberService;
		private readonly MinecraftService _minecraftService = minecraftService;

		public IActionResult Index()
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			if (User.Identity.Name == null)
				return RedirectToAction("Index", "Login");

			AdminHomeViewModel ahvm = new AdminHomeViewModel()
			{
				Username = User.Identity.Name
			};

			return View(ahvm);
		}

		public async Task<IActionResult> ServerMembers(string sortOrder, string searchString)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			ViewBag.MinecraftSortParm = string.IsNullOrEmpty(sortOrder) ? "minecraft_desc" : "";
			ViewBag.DiscordSortParm = sortOrder == "disc_asc" ? "disc_desc" : "disc_asc";
			ViewBag.DateSortParm = sortOrder == "date_asc" ? "date_desc" : "date_asc";
			ViewBag.DateModSortParm = sortOrder == "datemod_asc" ? "datemod_desc" : "datemod_asc";

			var members = await _memberService.GetAllMembers();
			var count = members.Count;

			if (!string.IsNullOrEmpty(searchString))
			{
				searchString = searchString.ToLower();
				members = members.Where(s => s.MinecraftUsername.ToLower().Contains(searchString) || (!string.IsNullOrEmpty(s.DiscordUsername) && s.DiscordUsername.ToLower().Contains(searchString))).ToList();
			}

			switch (sortOrder)
			{
				case "minecraft_desc":
					members = members.OrderByDescending(s => s.MinecraftUsername).ToList();
					break;
				case "date_asc":
					members = members.OrderBy(s => s.DateAdded).ToList();
					break;
				case "date_desc":
					members = members.OrderByDescending(s => s.DateAdded).ToList();
					break;
				case "datemod_asc":
					members = members.OrderBy(s => s.DateModified).ToList();
					break;
				case "datemod_desc":
					members = members.OrderByDescending(s => s.DateModified).ToList();
					break;
				case "disc_asc":
					members = members.OrderBy(s => s.DiscordUsername).ToList();
					break;
				case "disc_desc":
					members = members.OrderByDescending(s => s.DiscordUsername).ToList();
					break;
				default:
					members = members.OrderBy(s => s.MinecraftUsername).ToList();
					break;
			}

			foreach (var item in members)
			{
				if (item.MinecraftUUID == null)
					continue;
				if (item.MinecraftUsername != null)
					continue;

				string? username = await _minecraftService.GetUsernameFromUuidAsync(item.MinecraftUUID);

				if (username == null)
					ModelState.AddModelError(string.Empty, $"A username couldn't be found from the Minecraft uuid: {item.MinecraftUUID}");
				else
					item.MinecraftUsername = username;
			}

			AdminViewModel model = new AdminViewModel()
			{
				Members = members,
				MemberCount = count
			};

			return View(model);
		}

		public IActionResult CreateMember()
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			return View();
		}

		public IActionResult AddMembersFromFile()
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			return View();
		}

		[HttpGet]
		[ActionName("DownloadWhitelist")]
		public async Task<IActionResult> DownloadWhitelistOnPost()
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			var whitelist = new List<JsonMember>();

			var members = await this._memberService.GetAllMembers();

			foreach (var item in members)
			{
				whitelist.Add(new JsonMember { name = item.MinecraftUsername, uuid = item.MinecraftUUID });
			}

			using var textWriter = new StringWriter();
			string json = JsonConvert.SerializeObject(whitelist, Formatting.Indented);

			var byteArray = System.Text.Encoding.UTF8.GetBytes(json);

			var result = new FileContentResult(byteArray, "application/json")
			{
				FileDownloadName = "whitelist.json"
			};

			return result;
		}

		[HttpPost]
		[ActionName("AddMembersFromFile")]
		public async Task<IActionResult> AddMembersFromFileOnPost(AddMembersFromFileViewModel fileModel)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			if (fileModel == null)
			{
				return View();
			}

			if (fileModel.JsonFile == null || fileModel.JsonFile.Length == 0)
			{
				ModelState.AddModelError("JsonFile", "Please upload a JSON file.");
				return View();
			}

			if (!Path.GetExtension(fileModel.JsonFile.FileName).Equals(".json", StringComparison.CurrentCultureIgnoreCase))
			{
				ModelState.AddModelError("JsonFile", "Only JSON files are allowed.");
				return View();
			}

			using var stream = new StreamReader(fileModel.JsonFile.OpenReadStream());
			var jsonContent = await stream.ReadToEndAsync();
			var membersJson = JsonConvert.DeserializeObject<List<JsonMember>>(jsonContent);

			if (membersJson == null || membersJson.Count == 0)
				return RedirectToAction("ServerMembers", "Admin");

			List<Member> dbMembers = await this._memberService.GetAllMembers();

			List<Member> members = [];
			foreach (var mem in membersJson)
			{
				mem.uuid = mem.uuid.Replace("-", "");

				Member? dbMember = dbMembers?.Where(x => x.MinecraftUUID == mem.uuid).SingleOrDefault();

				if (dbMember?.MinecraftUUID == mem.uuid)
					continue;

				string? username = await _minecraftService.GetUsernameFromUuidAsync(mem.uuid);

				if (username == null)
					continue;

				if (dbMember != null && username == dbMember.MinecraftUsername)
					continue;

				members.Add(new Member()
				{
					DateAdded = DateTime.Now,
					MinecraftUsername = username,
					MinecraftUUID = mem.uuid
				});
			}

			await this._memberService.AddMemberRange(members);

			return RedirectToAction("ServerMembers", "Admin");
		}

		[HttpGet("EditMember/{id:int}")]
		public async Task<IActionResult> EditMember(int id)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			var member = await this._memberService.GetMemberFromID(id);

			if (member == null)
				return RedirectToAction("ServerMembers", "Admin");

			var emvm = new EditMemberViewModel()
			{
				Member = member
			};

			return View(emvm);
		}

		[HttpGet("RemoveMember/{id:int}")]
		public async Task<IActionResult> RemoveMember(int id)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			await this._memberService.RemoveMember(id);

			return RedirectToAction("ServerMembers", "Admin");
		}

		[HttpPost("EditMember/{id:int}")]
		[ActionName("EditMember")]
		public async Task<IActionResult> EditMemberOnPost(int id, Member member)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			var emvm = new EditMemberViewModel()
			{
				Member = member
			};

			string? uuid = await _minecraftService.GetUuidFromUsernameAsync(member.MinecraftUsername);

			if (uuid == null)
			{
				ModelState.AddModelError(string.Empty, $"An uuid couldn't be fetched with that Minecraft username: {member.MinecraftUsername}");
				return View(emvm);
			}

			var dbMember = await this._memberService.GetMemberFromID(id);

			if (dbMember == null)
				return RedirectToAction("ServerMembers", "Admin");

			if (uuid != dbMember.MinecraftUUID && await this._memberService.IsMemberPresent(uuid))
			{
				ModelState.AddModelError(string.Empty, $"A member with that Minecraft username already exists.");
				return View(emvm);
			}

			dbMember.MinecraftUsername = member.MinecraftUsername;
			dbMember.DiscordUsername = member.DiscordUsername;
			dbMember.MinecraftUUID = uuid;
			dbMember.DiscordUUID = member.DiscordUUID;
			dbMember.DateModified = DateTime.Now;

			await this._memberService.EditMember(dbMember);

			return RedirectToAction("ServerMembers", "Admin");
		}

		[HttpPost]
		[ActionName("CreateMember")]
		public async Task<IActionResult> CreateMemberOnPost(Member member)
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
				return RedirectToAction("Index", "Login");

			string? uuid = await _minecraftService.GetUuidFromUsernameAsync(member.MinecraftUsername);

			if (uuid == null)
			{
				ModelState.AddModelError(string.Empty, $"A uuid couldn't be fetched with that Minecraft username.");
				return View();
			}

			if (await this._memberService.IsMemberPresent(uuid))
			{
				ModelState.AddModelError(string.Empty, $"A member with that Minecraft username already exists.");
				return View();
			}

			member.DateAdded = DateTime.Now;
			member.MinecraftUUID = uuid;

			await this._memberService.AddMember(member);

			ModelState.AddModelError(string.Empty, $"A new member called: '{member.MinecraftUsername}' was created.");

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Login");
		}
	}
}
