using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TTSService
{
    public class SynthesizeSession : IDisposable
    {
        public static async Task<Stream> GetAudioAsync(Synthesize client, string requestUri, string accessToken, string text)
        {
            using (var session = new SynthesizeSession(client))
            {
                var forget = client.Speak(session._cancellationSource.Token, new InputOptions()
                {
                    RequestUri = new Uri(requestUri),
                    // Text to be spoken.
                    Text = text,
                    VoiceType = Gender.Female,
                    // Refer to the documentation for complete list of supported locales.
                    Locale = "en-US",
                    // You can also customize the output voice. Refer to the documentation to view the different
                    // voices that the TTS service can output.
                    VoiceName = "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)",
                    // Service can return audio in different output format.
                    OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                    AuthorizationToken = "Bearer " + accessToken,
                });

                var forgot = Task.Run(async () =>
                {
                    var workingTask = session._tcs.Task;
                    await Task.Delay(TimeSpan.FromSeconds(60), session._cancellationSource.Token);
                    if (workingTask.IsCompleted || workingTask.IsFaulted) return;
                    session._tcs.TrySetException(new InvalidOperationException("Timed out."));
                });

                return await session._tcs.Task;
            }
        }

        public void Dispose()
        {
            _client.OnAudioAvailable -= OnAudioAvailable;
            _client.OnError -= OnError;
        }

        #region Private
        /// <summary>
        /// This method is called once the audio returned from the service.
        /// It will then attempt to play that audio file.
        /// Note that the playback will fail if the output audio format is not pcm encoded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="GenericEventArgs{Stream}"/> instance containing the event data.</param>
        private void OnAudioAvailable(object sender, GenericEventArgs<Stream> args)
        {
            Console.WriteLine(args.EventData);
            _tcs.TrySetResult(args.EventData);
            _cancellationSource.Cancel();
        }

        /// <summary>
        /// Handler an error when a TTS request failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenericEventArgs{Exception}"/> instance containing the event data.</param>
        private void OnError(object sender, GenericEventArgs<Exception> e)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", e.ToString());
            _tcs.TrySetException(e.EventData);
            _cancellationSource.Cancel();
        }

        private SynthesizeSession(Synthesize client)
        {
            _client = client;
            _client.OnAudioAvailable += OnAudioAvailable;
            _client.OnError += OnError;
        }

        private TaskCompletionSource<Stream> _tcs = new TaskCompletionSource<Stream>();
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private readonly Synthesize _client;
        #endregion
    }
}
