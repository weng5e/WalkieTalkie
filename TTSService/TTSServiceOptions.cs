using System;
using System.Collections.Generic;
using System.Text;

namespace TTSService
{
    public class TTSServiceOptions
    {
        public string AuthenticationUri { get; set; } = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

        public string SynthesizeUri { get; set; } = "https://speech.platform.bing.com/synthesize";

        public string ApiKey { get; set; }
    }
}
