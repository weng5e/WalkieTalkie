using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TTSService
{
    public interface ITextToSpeechService
    {
        Task<Stream> GetAudioAsync(string text, CancellationToken cancellationToken);
    }
}
