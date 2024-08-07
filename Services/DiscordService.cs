using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PondWebApp.Services
{
	public class DiscordService(ILogger<DiscordService> logger, IServiceScopeFactory serviceScopeFactory) : IHostedService
	{
		private readonly ILogger<DiscordService> _logger = logger;
		private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
		private DiscordSocketClient? _client;

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_client = new DiscordSocketClient();
			_client.Log += Log;

			var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN");

			if (string.IsNullOrEmpty(token))
			{
				_logger.LogError("Error: DISCORD_TOKEN environment variable is not set.");
				return;
			}

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			
			_logger.LogInformation("Discord bot started.");

			_client.Ready += IsReady;
			_client.UserLeft += UserLeft;
		}

		public async Task IsReady()
		{
			var channel = _client.GetChannel(1270606722452553738) as ITextChannel;

			if (channel != null)
			{
				await channel.SendMessageAsync("# Discord PondApp has started!\nListening for:\n- If a user leaves.\n- If a new application comes in.");
			}
		}

		public async Task UserLeft(SocketGuild guild, SocketUser user)
		{
			using var scope = _serviceScopeFactory.CreateScope();
			var memberService = scope.ServiceProvider.GetRequiredService<MemberService>();

			await memberService.HandleUserLeft(user.Id);

			if (this._client == null)
				return;

			if (_client.GetChannel(1270606722452553738) is ITextChannel channel)
			{
				await channel.SendMessageAsync($"{user.Username} was removed from the Pond database.");
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			if (this._client == null)
				return Task.CompletedTask;

			_logger.LogInformation("Discord bot stopping.");
			return this._client.StopAsync();
		}

		private Task Log(LogMessage message)
		{
			_logger.LogInformation(message.ToString());
			return Task.CompletedTask;
		}
	}
}
