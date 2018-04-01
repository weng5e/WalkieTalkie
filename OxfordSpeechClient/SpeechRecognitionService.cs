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
        private readonly CustomSpeechServiceOptions _options;
        private readonly DataRecognitionClient _client;

        public SpeechRecognitionService(CustomSpeechServiceOptions options)
        {
            _options = options;
            _client = SpeechRecognitionServiceFactory.CreateDataClient(SpeechRecognitionMode.ShortPhrase,
                                                                                 "en-us",
                                                                                 _options.PrimaryKey,
                                                                                 _options.SecondaryKey,
                                                                                 _options.ServiceUrl);
            _client.AuthenticationUri = _options.AuthenticationUrl;
        }

        public async Task<SpeechRecognitionResult> RecognizeSpeechAsync(Stream audioStream, CancellationToken token = default(CancellationToken))
        {
            return await RecognitionSession.RecognizeSpeechAsync(_client, audioStream, token);
        }

        public void Dispose()
        {
        }
    }
}
