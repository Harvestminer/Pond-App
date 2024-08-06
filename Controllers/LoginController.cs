using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PondWebApp.Models;
using PondWebApp.Services;
using System.Diagnostics;
using System.Security.Claims;

namespace PondWebApp.Controllers
{
	public class LoginController(ILogger<LoginController> logger, AdminService adminService) : Controller
	{
		private readonly ILogger<LoginController> _logger = logger;
		private readonly AdminService _adminService = adminService;

		public IActionResult Index()
		{
			if (IsAuthenticated())
				return RedirectToAction("Index", "Admin");

			return View();
		}

		[HttpPost]
		[ActionName("Index")]
		public async Task<IActionResult> SignIn(Admin loginAdmin)
		{
			if (ModelState.IsValid)
			{
				var admin = await _adminService.AuthenticateAdmin(loginAdmin, loginAdmin.Password);

				if (!admin)
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View();
				}

				var claims = new List<Claim>()
				{
					new (ClaimTypes.Name, loginAdmin.Username),
					new (ClaimTypes.Role, "Administrator"),
				};

				var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					AllowRefresh = true,
					IsPersistent = true,
					IssuedUtc = DateTimeOffset.UtcNow,
				};

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimIdentity),
					authProperties);

				_logger.LogInformation($"User {loginAdmin.Username} logged in at {DateTimeOffset.UtcNow}");

				return RedirectToAction("Index", "Admin");
			}

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private bool IsAuthenticated()
		{
			return Request.Cookies.ContainsKey(CookieAuthenticationDefaults.CookiePrefix + CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
