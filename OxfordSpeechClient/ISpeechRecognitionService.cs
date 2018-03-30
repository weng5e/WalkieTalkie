using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordSpeechClient
{
    public interface ISpeechRecognitionService : IDisposable
    {
        Task<SpeechRecognitionResult> RecognizeSpeechAsync(Stream audioStream, CancellationToken token = default(CancellationToken));
    }
}
