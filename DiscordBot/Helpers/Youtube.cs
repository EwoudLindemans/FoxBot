using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VideoLibrary;

namespace DiscordBot.Helpers
{
	public class YoutubeResult
	{
		public YouTubeVideo Meta;
		public string Path;
	}

	public static class Youtube
	{
		private static readonly string DownloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp"); /*Path.GetTempPath() ??*/
		private static int Count = 0;

		/// <summary>
		/// Download Song or Video
		/// </summary>
		/// <param name="url">The URL to the Song or Video</param>
		/// <returns>The File Location to the downloaded Song or Video</returns>
		public static async Task<YoutubeResult> Download(string url) {
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
		private static async Task<YoutubeResult> DownloadFromYouTube(string url) {
			var youTube = YouTube.Default;
			var video = await youTube.GetVideoAsync(url);


			string file = Path.Combine(DownloadPath, "botsong" + ++Count + video.FileExtension);
			if (!Directory.Exists(DownloadPath)) {
				Directory.CreateDirectory(DownloadPath);
			}
			File.WriteAllBytes(file, video.GetBytes());

			if (File.Exists(file)) {
				return new YoutubeResult() {
					Meta = video,
					Path = file
				};
			}

			return null;
		}
	}
}
