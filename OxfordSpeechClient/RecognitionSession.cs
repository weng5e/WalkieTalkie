using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordSpeechClient
{
    public class RecognitionSession : IDisposable
    {
        private readonly DataRecognitionClient _client;
        private TaskCompletionSource<SpeechRecognitionResult> _tcs = new TaskCompletionSource<SpeechRecognitionResult>();
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();


        public static async Task<SpeechRecognitionResult> RecognizeSpeechAsync(DataRecognitionClient client, Stream audioStream, CancellationToken token = default(CancellationToken))
        {
            using (var session = new RecognitionSession(client))
            {
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
                        var workingTask = session._tcs.Task;
                        await Task.Delay(TimeSpan.FromSeconds(60), session._cancellationSource.Token);
                        if (workingTask.IsCompleted || workingTask.IsFaulted) return;
                        session._tcs.TrySetException(new InvalidOperationException("Timed out."));
                    });
                }

                return await session._tcs.Task;
            }
        }

        public void Dispose()
        {
            _client.OnResponseReceived -= this.OnResponseReceived;
            _client.OnConversationError -= this.OnConversationError;
        }

        private void OnResponseReceived(object sender, SpeechResponseEventArgs e)
        {
            _tcs.TrySetResult(SpeechRecognitionResult.FromCrisResult(e.PhraseResponse));
            _cancellationSource.Cancel();
        }

        private void OnConversationError(object sender, SpeechErrorEventArgs e)
        {
            _tcs.TrySetException(new InvalidOperationException(e.SpeechErrorText));
            _cancellationSource.Cancel();
        }

        private RecognitionSession(DataRecognitionClient client)
        {
            _client = client;
            _client.OnResponseReceived += this.OnResponseReceived;
            _client.OnConversationError += this.OnConversationError;
        }
    }
}
