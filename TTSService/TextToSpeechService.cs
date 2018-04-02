using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TTSService
{
    public class TextToSpeechService : ITextToSpeechService, IDisposable
    {
        public TextToSpeechService(TTSServiceOptions options)
        {
            _client = new HttpClient();
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

        public async Task<Stream> GetAudioAsync(string text, CancellationToken cancellationToken)
        {
            //return await SynthesizeSession.GetAudioAsync(client, _options.SynthesizeUri, _authentication.GetAccessToken(), text);
            var inputOptions = GetDefaultOptions(text);

            _client.DefaultRequestHeaders.Clear();
            foreach (var header in inputOptions.Headers)
            {
                _client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, inputOptions.RequestUri)
            {
                Content = new StringContent(GenerateSsml(inputOptions.Locale, inputOptions.VoiceType.ToString(), inputOptions.VoiceName, inputOptions.Text))
            };

            var responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            Debug.WriteLine("Response status code: [{0}]", responseMessage.StatusCode);

            if (responseMessage.IsSuccessStatusCode)
            {
                return await responseMessage.Content.ReadAsStreamAsync();
            }
            else
            {
                throw new Exception(String.Format("Service returned {0}", responseMessage.StatusCode));
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        #region Private
        /// <summary>
        /// Generates SSML.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="name">The voice name.</param>
        /// <param name="text">The text input.</param>
        private static string GenerateSsml(string locale, string gender, string name, string text)
        {
            var ssmlDoc = new XDocument(
                              new XElement("speak",
                                  new XAttribute("version", "1.0"),
                                  new XAttribute(XNamespace.Xml + "lang", "en-US"),
                                  new XElement("voice",
                                      new XAttribute(XNamespace.Xml + "lang", locale),
                                      new XAttribute(XNamespace.Xml + "gender", gender),
                                      new XAttribute("name", name),
                                      text)));
            return ssmlDoc.ToString();
        }

        private InputOptions GetDefaultOptions(string text)
        {
            return new InputOptions()
            {
                RequestUri = new Uri(_options.SynthesizeUri),
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
                AuthorizationToken = "Bearer " + _authentication.GetAccessToken(),
            };
        }

        private readonly HttpClient _client;
        private readonly TTSServiceOptions _options;
        private readonly Authentication _authentication;
        #endregion
    }
}
