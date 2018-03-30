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

        public SpeechRecognitionService(CustomSpeechServiceOptions options)
        {
            _options = options;

        }

        public async Task<SpeechRecognitionResult> RecognizeSpeechAsync(Stream audioStream, CancellationToken token = default(CancellationToken))
        {
            using (var client = SpeechRecognitionServiceFactory.CreateDataClient(SpeechRecognitionMode.ShortPhrase,
                                                                                 "en-us",
                                                                                 _options.PrimaryKey,
                                                                                 _options.SecondaryKey,
                                                                                 _options.ServiceUrl))
            {
                client.AuthenticationUri = _options.AuthenticationUrl;

                var tcs = new TaskCompletionSource<SpeechRecognitionResult>();

                client.OnResponseReceived += (sender, e) =>
                {
                    tcs.TrySetResult(SpeechRecognitionResult.FromCrisResult(e.PhraseResponse));
                };

                client.OnConversationError += (sender, e) =>
                {
                    tcs.TrySetException(new InvalidOperationException(e.SpeechErrorText));
                };

                int bytesRead = 0;
                byte[] buffer = new byte[1024];
                // Set the position to the beginning of the stream.
                audioStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    do
                    {
                        bytesRead = audioStream.Read(buffer, 0, buffer.Length);
                        client.SendAudio(buffer, bytesRead);
                    }
                    while (bytesRead > 0 && !token.IsCancellationRequested);
                }
                finally
                {
                    client.EndAudio();
                    var forgot = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(60));
                        tcs.TrySetException(new InvalidOperationException("Timed out."));
                    });
                }

                return await tcs.Task;
            }
        }

        public void Dispose()
        {
        }
    }
}
