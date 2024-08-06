using Newtonsoft.Json.Linq;

namespace PondWebApp.Services
{
	public class MinecraftService(HttpClient httpClient)
	{
		private readonly HttpClient _httpClient = httpClient;

		/// <summary>
		/// Get a Minecraft username from a Minecraft uuid
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns></returns>
		public async Task<string?> GetUsernameFromUuidAsync(string uuid)
		{
			var url = $"https://sessionserver.mojang.com/session/minecraft/profile/{uuid}";

			try
			{
				var response = await _httpClient.GetStringAsync(url);
				var profile = JObject.Parse(response);

				var name = profile?["name"]?.ToString();

				return name;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching username: {ex.Message}");
				return null;
			}
		}

		/// <summary>
		/// Get the Minecraft uuid from a Minecraft username
		/// </summary>
		/// <param name="uuid"></param>
		/// <returns></returns>
		public async Task<string?> GetUuidFromUsernameAsync(string username)
		{
			var url = $"https://api.mojang.com/users/profiles/minecraft/{username}";

			try
			{
				var response = await _httpClient.GetStringAsync(url);
				var user = JObject.Parse(response);

				// Extract UUID from the response
				var uuid = user["id"]?.ToString();

				return uuid;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching UUID: {ex.Message}");
				return null;
			}
		}
	}
}
