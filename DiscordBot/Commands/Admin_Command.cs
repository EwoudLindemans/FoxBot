using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands {

	public class RequireOwnerAttribute : PreconditionAttribute {
		// Override the CheckPermissions method
		public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services) {
			if(context.User.Id == 119125517080330240) {
				return PreconditionResult.FromSuccess();
			} else {
				await Logger.Log(new Discord.LogMessage(Discord.LogSeverity.Warning, "admin_permission", $"{context.User.Username} ({context.User.Id}) is using the /admin command"));
			}
			return PreconditionResult.FromError($"{context.User.Mention} Je hebt geen rechten om dit commando uit te voeren");
		}
	}


	public partial class Listen_Command : ModuleBase<SocketCommandContext> {
		[Group("admin"), RequireOwner]
		
		public class AdminModule : ModuleBase<SocketCommandContext> {
			[Command("remove")]
			[Summary("Echos a message.")]
			public async Task RemoveAsync(int count) {
				// ReplyAsync is a method on ModuleBase
				var readcollection = await this.Context.Message.Channel.GetMessagesAsync(count).Last();
				var messages = readcollection.ToList();
				var delete = messages.Where(x => x.Author.IsBot || x.Content.StartsWith(";;") || x.Content.StartsWith("/") || x.Content.StartsWith("!"));
				foreach (var item in delete) {
					await item.DeleteAsync();
				}
				await ReplyAsync("Jobs done: I removed all bot messages");
			}
		}
	}

}
