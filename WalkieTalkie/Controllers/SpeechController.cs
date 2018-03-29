using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // POST /api/Speech/ASR
        [HttpPost("[action]")]
        public async Task<string> ASR(IFormFile audioFile)
        {

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            return "result";
        }
    }
}
