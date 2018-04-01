using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OxfordSpeechClient
{
    public class RecognitionClientManager : IDisposable
    {
        private SemaphoreSlim _waitingSignal;
        private object _syncObj = new object();
        private Queue<DataRecognitionClient> _clients = new Queue<DataRecognitionClient>();

        public RecognitionClientManager(CustomSpeechServiceOptions options, int clientCount = 3)
        {
            _waitingSignal = new SemaphoreSlim(clientCount);
            for (int i = 0; i < clientCount; i++)
            {
                var client = SpeechRecognitionServiceFactory.CreateDataClient(SpeechRecognitionMode.ShortPhrase,
                                                                                 "en-us",
                                                                                 options.PrimaryKey,
                                                                                 options.SecondaryKey,
                                                                                 options.ServiceUrl);
                client.AuthenticationUri = options.AuthenticationUrl;

                _clients.Enqueue(client);
            }
        }

        public void Dispose()
        {
            Queue<DataRecognitionClient> clients = null;
            lock (_syncObj)
            {
                clients = _clients;
                _clients = null;
                if (clients == null) return;
            }

            foreach (var c in clients)
            {
                c.Dispose();
            }

        }

        public async Task<DataRecognitionClient> GetClientAsync()
        {
            await _waitingSignal.WaitAsync();

            lock (_syncObj)
            {
                if (_clients.Count == 0)
                {
                    throw new InvalidOperationException("Client queue size is zero.");
                }

                return _clients.Dequeue();
            }
        }

        public void ReleaseClient(DataRecognitionClient client)
        {
            lock (_syncObj)
            {
                _clients.Enqueue(client);
            }

            _waitingSignal.Release();
        }
    }
}
