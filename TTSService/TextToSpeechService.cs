using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TTSService
{
    public class TextToSpeechService : ITextToSpeechService
    {
        private readonly TTSServiceOptions _options;
        private readonly Authentication _authentication;

        public TextToSpeechService(TTSServiceOptions options)
        {
            _options = options;
            _authentication = new Authentication(options);
            try
            {
                var accessToken = _authentication.GetAccessToken();
                Console.WriteLine("Token: {0}\n", accessToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed authentication.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);
                throw new Exception("Cannot set up TTS Authentication", ex);
            }
        }

        public async Task<Stream> GetAudioAsync(string text)
        {
            var client = new Synthesize();

            return await SynthesizeSession.GetAudioAsync(client, _options.SynthesizeUri, _authentication.GetAccessToken(), text);
        }
    }
}
