using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Helpers
{
	public static class FFmpeg
	{
		public static float DetectDbOffset(string path) {
			var detectVolumeProcess = new Process();
			string detectVolumeProcessContent = "";
			detectVolumeProcess.StartInfo.FileName = "ffmpeg.exe";
			detectVolumeProcess.StartInfo.Arguments = $"-hide_banner -i \"{path}\" -filter:a volumedetect -f null /dev/null";
			detectVolumeProcess.StartInfo.UseShellExecute = false;
			detectVolumeProcess.StartInfo.RedirectStandardOutput = true;
			detectVolumeProcess.StartInfo.RedirectStandardError = true;
			detectVolumeProcess.EnableRaisingEvents = true;
			detectVolumeProcess.OutputDataReceived += (sender, args) => detectVolumeProcessContent += args.Data;
			detectVolumeProcess.ErrorDataReceived += (sender, args) => detectVolumeProcessContent += args.Data;
			detectVolumeProcess.Start();
			detectVolumeProcess.BeginOutputReadLine();
			detectVolumeProcess.BeginErrorReadLine();
			detectVolumeProcess.WaitForExit();

			//Regex max volume:
			var regex = new Regex("max_volume:(.*?)db", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			float dbOffset = 0;


			var match = regex.Match(detectVolumeProcessContent);
			if (match.Groups.Count > 0) {
				float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out dbOffset);
			}

			return dbOffset;
		}

		public static void AdjustVolume(string path, float dbIncrease) {
			var adjustVolume = new Process();
			string detectVolumeProcessContent = "";
			adjustVolume.StartInfo.FileName = "ffmpeg.exe";
			adjustVolume.StartInfo.Arguments = $"-hide_banner -loglevel panic -filter:a \"volume = {dbIncrease * -1}dB\"  -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1";
			adjustVolume.StartInfo.UseShellExecute = false;
			adjustVolume.StartInfo.RedirectStandardOutput = true;
			adjustVolume.StartInfo.RedirectStandardError = true;
			adjustVolume.EnableRaisingEvents = true;
			adjustVolume.OutputDataReceived += (sender, args) => detectVolumeProcessContent += args.Data;
			adjustVolume.ErrorDataReceived += (sender, args) => detectVolumeProcessContent += args.Data;
			adjustVolume.Start();
			adjustVolume.BeginOutputReadLine();
			adjustVolume.BeginErrorReadLine();
			adjustVolume.WaitForExit();
		}

		public static Process GetFFMpegStream(string path) {
			return Process.Start(new ProcessStartInfo {
				FileName = "ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}
	}
}
