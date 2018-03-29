using System;
using System.Collections.Generic;
using System.Text;

namespace OxfordSpeechClient
{
    public sealed class SpeechRecognitionService: ISpeechRecognitionService
    {
        private readonly CustomSpeechServiceOptions _options;

        public SpeechRecognitionService(CustomSpeechServiceOptions options)
        {
            _options = options;
        }
    }
}
