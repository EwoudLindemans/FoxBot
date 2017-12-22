using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace DiscordBot {
	class Helpers {
		private static readonly string DownloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp"); /*Path.GetTempPath() ??*/



		/// <summary>
		/// Download Song or Video
		/// </summary>
		/// <param name="url">The URL to the Song or Video</param>
		/// <returns>The File Location to the downloaded Song or Video</returns>
		public static async Task<string> Download(string url) {
			if (url.ToLower().Contains("youtube.com")) {
				return await DownloadFromYouTube(url);
			} else {
				throw new Exception("Video URL not supported!");
			}
		}

		/// <summary>
		/// Download the Video from YouTube url and extract it
		/// </summary>
		/// <param name="url">URL to the YouTube Video</param>
		/// <returns>The File Path to the downloaded mp3</returns>
		private static async Task<string> DownloadFromYouTube(string url) {
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

			new Thread(() => {
				int count = 0;
				
				//youtube-dl.exe
				IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);
				VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

				if (video.RequiresDecryption) {
					DownloadUrlResolver.DecryptDownloadUrl(video);
				}

				string file = Path.Combine(DownloadPath, "botsong" + ++count + video.VideoExtension); ;
				var audioDownloader = new VideoDownloader(video, file);
				audioDownloader.Execute();

				if (File.Exists(file)) {
					tcs.SetResult(file);
				} else {
					tcs.SetResult(null);
				}
			}).Start();

			string result = await tcs.Task;
			if (result == null)
				throw new Exception("youtube-dl.exe failed to download!");

			//Remove \n at end of Line
			result = result.Replace("\n", "").Replace(Environment.NewLine, "");

			return result;
		}

	}
}
