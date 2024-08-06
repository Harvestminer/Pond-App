using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PondWebApp.Services
{
	public class DiscordService(HttpClient httpClient, string botToken)
	{
		private readonly HttpClient _httpClient = httpClient;
		private readonly string _botToken = botToken;

		/// <summary>
		/// Get Discord user id from Discord username
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public async Task<string?> GetUserIdByUsernameAsync(string username)
		{
			var url = "https://discord.com/api/v10/users/@me";

			try
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _botToken);
				var response = await _httpClient.GetStringAsync(url);
				var user = JObject.Parse(response);

				// Extract the user ID from the response
				var userId = user["id"]?.ToString();

				return userId;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching user ID: {ex.Message}");
				return null;
			}
		}
	}
}
