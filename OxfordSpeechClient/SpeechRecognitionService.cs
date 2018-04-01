using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordSpeechClient
{
    public sealed class SpeechRecognitionService : ISpeechRecognitionService
    {
        private readonly RecognitionClientManager _clientManager;

        public SpeechRecognitionService(CustomSpeechServiceOptions options)
        {
            _clientManager = new RecognitionClientManager(options);
        }

        public async Task<SpeechRecognitionResult> RecognizeSpeechAsync(Stream audioStream, CancellationToken token = default(CancellationToken))
        {
            var client = await _clientManager.GetClientAsync();
            try
            {
                return await RecognitionSession.RecognizeSpeechAsync(client, audioStream, token);
            }
            finally
            {
                _clientManager.ReleaseClient(client);
            }
        }

        public void Dispose()
        {
            _clientManager.Dispose();
        }
    }
}
