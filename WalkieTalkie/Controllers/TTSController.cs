using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
            var audioStream = await _ttsService.GetAudioAsync(text);

            //SoundPlayer player = new SoundPlayer(audioStream);
            //player.Play();

            return File(audioStream, "audio/wav"); // FileStreamResult
        }
    }
}
