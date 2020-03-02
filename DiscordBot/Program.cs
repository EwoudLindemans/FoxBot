using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using System.Linq;

namespace DiscordBot {

	

	public class Program {
		public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

		private static readonly IServiceCollection _map = new ServiceCollection();
		private static readonly CommandService _commands = new CommandService();
		public static DiscordSocketClient _client;
		private static IServiceProvider _services;


		public async Task MainAsync() {
			_client = new DiscordSocketClient();

			await InitCommands();

			_client.Log += Logger.Log;

			string token = ""; // Remember to keep this private!
			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();
			await _client.SetGameAsync("Playing Fox", "https://discordapp.com/oauth2/authorize?&client_id=388048410932150272&scope=bot&permissions=1878523073");

			// Block this task until the program is closed.
			await Task.Run(async () => {
				string line;
				while ((line = Console.ReadLine()) != null) {
					var id = _client.Guilds.FirstOrDefault().Channels.Where(x => x.Name == "general").FirstOrDefault().Id;
					await (_client.GetChannel(id) as IMessageChannel).SendMessageAsync(line);
				}
			});
			
			//await Task.Delay(Timeout.Infinite);
		}


		private async Task InitCommands() {
			// Repeat this for all the service classes
			// and other dependencies that your commands might need.
			_map.AddSingleton(new Listen_Command());
			_map.AddSingleton(new Music_Command());
			// When all your required services are in the collection, build the container.
			// Tip: There's an overload taking in a 'validateScopes' bool to make sure
			// you haven't made any mistakes in your dependency graph.
			_services = _map.BuildServiceProvider();

			// Either search the program and add all Module classes that can be found.
			// Module classes MUST be marked 'public' or they will be ignored.
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

			_client.MessageReceived += HandleCommandAsync;
		}

		private async Task HandleCommandAsync(SocketMessage arg) {
			// Bail out if it's a System Message.
			var msg = arg as SocketUserMessage;
			if (msg == null) return;
			if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

			int pos = 0;
			if (msg.HasCharPrefix('/', ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos)) {

				//This is a message for me, remove for other users to see
				//delete the original send message
				try {
					await arg.DeleteAsync();
				} catch (Exception ex) {
					await Logger.Log(new LogMessage(LogSeverity.Error, "program", "unable to delete message", ex));
				}

				// Create a Command Context.
				var context = new SocketCommandContext(_client, msg);

				// Execute the command. (result does not indicate a return value, 
				// rather an object stating if the command executed successfully).
				var result = await _commands.ExecuteAsync(context, pos, _services);

				// Uncomment the following lines if you want the bot
				// to send a message if it failed (not advised for most situations).

				if (!result.IsSuccess && result.Error != CommandError.UnknownCommand) {
					await msg.Channel.SendMessageAsync(result.ErrorReason);
				}
			}
		}
	}
}
