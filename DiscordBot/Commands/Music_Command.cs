using Discord;
using Discord.Audio;
using Discord.Commands;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using static DiscordBot.Helpers;

namespace DiscordBot {
	public class Music_Command : ModuleBase<SocketCommandContext> {
		private static IAudioClient _vClient;

		private static CancellationTokenSource _cancelPlayTokenSource;
		private static CancellationToken _cancelPlayToken;

		[Command("join", RunMode = RunMode.Async), Alias("summon", "invite")]
		public async Task Join()  => _vClient = await (Context.User as IVoiceState).VoiceChannel.ConnectAsync();

		[Command("leave", RunMode = RunMode.Async), Alias("dismiss")]
		public async Task Leave() => await _vClient.StopAsync();

		[Command("play", RunMode = RunMode.Async)]
		public async Task Play() {
			await SendAudioAsync(GetMusic().FirstOrDefault());
		}

		[Command("stop", RunMode = RunMode.Async)]
		public async Task Stop() { 
			if(_cancelPlayTokenSource != null) { 
				_cancelPlayTokenSource.Cancel();
			}
		}

		[Command("next", RunMode = RunMode.Async)]
		public async Task Next() => await Play();

		[Command("youtube", RunMode = RunMode.Async)]
		public async Task Youtube([Remainder]string url) {
			var file = await Helpers.Download(url);
			await SendAudioAsync(file);
		}

		[Command("say", RunMode = RunMode.Async)]
		public async Task Say([Remainder]string text = null) {
			await Stop();
			var outFormat = new WaveFormat(48000, 16, 2);
			using (var ss = new SpeechSynthesizer())
			using (var discord = _vClient.CreatePCMStream(AudioApplication.Music))
			using (var tstream = new MemoryStream()) {
				ss.Volume = 100;
				ss.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, new System.Globalization.CultureInfo("nl-NL"));
				
				ss.SetOutputToWaveStream(tstream);
				ss.Speak(text);
				tstream.Flush();
				tstream.Seek(0, SeekOrigin.Begin);

				using (var wave = new WaveFileReader(tstream))
				using (var resampler = new MediaFoundationResampler(wave, outFormat)) {
					resampler.ResamplerQuality = 60;
					int blockSize = outFormat.AverageBytesPerSecond / 50;
					byte[] buffer = new byte[blockSize];
					int byteCount;
					while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) {
						if (byteCount < blockSize) { for (int i = byteCount; i < blockSize; i++) { buffer[i] = 0; } }
						discord.Write(buffer, 0, blockSize);
					}
					wave.Flush();
				}
				tstream.Flush();
				discord.Flush();
			}
		}

		public async Task SendAudioAsync(YoutubeResult result) {
			if (result.Meta != null) {
				await ReplyAsync(":musical_note: " + result.Meta.FullName);
				await Stop();
			}
			await SendAudioAsync(result.Path);
		}

		public async Task SendAudioAsync(string path) {
			_cancelPlayTokenSource = new CancellationTokenSource();
			_cancelPlayToken = _cancelPlayTokenSource.Token;
			// Your task: Get a full path to the file if the value of 'path' is only a filename.
			using (var output = CreateStream(path).StandardOutput.BaseStream)
			using (var stream = _vClient.CreatePCMStream(AudioApplication.Music)) {
				try { await output.CopyToAsync(stream,81920, _cancelPlayToken); } finally { await stream.FlushAsync(); }
			}
		}

		private Process CreateStream(string path) {
			return Process.Start(new ProcessStartInfo {
				FileName = "ffmpeg.exe",
				Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true
			});
		}
	}
}
