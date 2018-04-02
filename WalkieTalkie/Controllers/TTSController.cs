using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TTSService;

namespace WalkieTalkie.Controllers
{
    [Route("api/[controller]")]
    public class TTSController : Controller
    {
        private readonly ITextToSpeechService _ttsService;

        public TTSController(ITextToSpeechService ttsService)
        {
            _ttsService = ttsService;
        }

        // GET /api/TTS/GetAudio?text="Hello"
        [HttpGet("[action]")]
        public async Task<object> GetAudio(string text)
        {
            if (TryDecodeBase64(text, out string decoded))
            {
                text = decoded;
            }

            var cts = new CancellationTokenSource();
            var workingTask = _ttsService.GetAudioAsync("You Just said: " + text, cts.Token);

            var forgot = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                if (workingTask.IsCompleted || workingTask.IsFaulted) return;
                cts.Cancel();
            });

            var audioStream = await workingTask;
            return File(audioStream, "audio/wav"); // FileStreamResult

            //SoundPlayer player = new SoundPlayer(audioStream);
            //player.Play();
        }

        private static bool TryDecodeBase64(string input, out string output)
        {
            try
            {
                byte[] data = Convert.FromBase64String(input);
                output = Encoding.UTF8.GetString(data);
                return true;
            }
            catch
            {
                output = null;
                return false;
            }
        }
    }
}
