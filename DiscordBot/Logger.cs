using Discord;
using DiscordBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot {
	public static class Logger {
		public static string ToReadableString(this Exception ex, LogSeverity severity, StringBuilder sb = null, int indenting = 0) {
			var exceptionType = "";
			if (sb == null) {
				sb = new StringBuilder();
				sb.AppendLine($"[{DateTime.UtcNow.ToString("HH:mm:ss")}][{severity,8}][{ex.GetType()}]");
			} else {
				exceptionType = "[InnerException]";
			}

			string tabs = "";
			for (int i = 0; i < indenting; i++) { tabs = tabs + "\t"; }

			if (!string.IsNullOrWhiteSpace(ex.Message)) { sb.AppendLine($"{tabs}{exceptionType} {ex.Message}"); }
			if (!string.IsNullOrWhiteSpace(ex.StackTrace)) { sb.AppendLine($"{tabs}{exceptionType} {ex.StackTrace}"); }
			if (ex.InnerException != null && indenting < 10) {
				ex.InnerException.ToReadableString(severity, sb, indenting + 1);
			}

			return sb.ToString();
		}

		public static Task Log(LogMessage message) {
			switch (message.Severity) {
				case LogSeverity.Critical:
				case LogSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogSeverity.Info:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case LogSeverity.Verbose:
				case LogSeverity.Debug:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
			}
			Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception?.ToReadableString(message.Severity)}");
			Console.ResetColor();
			return Task.CompletedTask;
		}
	}
}
