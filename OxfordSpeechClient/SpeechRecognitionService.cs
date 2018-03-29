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
                                                                                 options.PrimaryKey,
                                                                                 options.SecondaryKey,
                                                                                 options.ServiceUrl);
            _client.AuthenticationUri = _options.AuthenticationUrl;
        }

        public async Task<RecognitionResult> RecognizeSpeechAsync(Stream audioStream, CancellationToken token = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<RecognitionResult>();

            _client.OnResponseReceived += (sender, e) =>
            {
                tcs.SetResult(e.PhraseResponse);
            };

            _client.OnConversationError += (sender, e) =>
            {
                tcs.SetException(new InvalidOperationException(e.SpeechErrorText));
            };

            int bytesRead = 0;
            byte[] buffer = new byte[1024];

            try
            {
                do
                {
                    bytesRead = audioStream.Read(buffer, 0, buffer.Length);
                    _client.SendAudio(buffer, bytesRead);
                }
                while (bytesRead > 0 && !token.IsCancellationRequested);
            }
            finally
            {
                _client.EndAudio();
            }

            return await tcs.Task;
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
