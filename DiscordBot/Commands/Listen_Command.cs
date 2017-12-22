using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot {
	public partial class Listen_Command : ModuleBase<SocketCommandContext> {
		//[Command("say")]
		//[Summary("Echos a message.")]
		//public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo) {
		//	// ReplyAsync is a method on ModuleBase
		//	await ReplyAsync(echo);
		//}


		[Command("code")]
		[Summary("Echos a message.")]
		public async Task CodeAsync(string language, [Remainder]string text) {
			// ReplyAsync is a method on ModuleBase
			await ReplyAsync("```" + language + "\n" + text + "```");
		}



		[Command("ddos")]
		[Summary("Echos a message.")]
		public async Task DdosAsync([Remainder] [Summary("Doing a ddos")] string echo) {
			// ReplyAsync is a method on ModuleBase
			await ReplyAsync("ddos in progress.");
			await Task.Delay(1);
			await ReplyAsync("ddos in progress.");
			await Task.Delay(1);
			await ReplyAsync("system failure.");
			await Task.Delay(1);
			await ReplyAsync("failure.");
			await Task.Delay(2);
			await ReplyAsync(".....");
			await Task.Delay(1);
			await ReplyAsync("im almost down.");
			await Task.Delay(1);
			await ReplyAsync("death...");
		}
	}
}
