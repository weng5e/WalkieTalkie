using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OxfordSpeechClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WalkieTalkie.Controllers
{
    [Route("api/[controller]")]
    public class SpeechController : Controller
    {
        private readonly ISpeechRecognitionService _speechRecognitionService;

        public SpeechController(ISpeechRecognitionService speechRecognitionService)
        {
            _speechRecognitionService = speechRecognitionService;
        }

        // POST /api/Speech/ASR
        [HttpPost("[action]")]
        public async Task<object> ASR(IFormFile audioFile)
        {

            //// full path to file in temp location
            //var filePath = Path.GetTempFileName();

            //using (var stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await audioFile.CopyToAsync(stream);
            //}

            using (var ms = new MemoryStream())
            {
                await audioFile.CopyToAsync(ms);
                var result = await _speechRecognitionService.RecognizeSpeechAsync(ms);
                return result;
            }
        }
    }
}
