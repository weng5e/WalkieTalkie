using Microsoft.CognitiveServices.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxfordSpeechClient
{
    public class SpeechRecognitionResult
    {
        public string RecognitionStatus { get; private set; }
        public SpeechRecognizedPhrase[] Results { get; set; }

        public static SpeechRecognitionResult FromCrisResult(RecognitionResult crisResult)
        {
            var result = new SpeechRecognitionResult()
            {
                RecognitionStatus = crisResult.RecognitionStatus.ToString(),
                Results = crisResult.Results.Select(r => SpeechRecognizedPhrase.FromCrisResult(r)).ToArray()
            };
            return result;
        }

        private SpeechRecognitionResult() { }
    }

    public class SpeechRecognizedPhrase
    {
        public static SpeechRecognizedPhrase FromCrisResult(RecognizedPhrase crisResult)
        {
            var result = new SpeechRecognizedPhrase()
            {
                Confidence = crisResult.Confidence.ToString(),
                DisplayText = crisResult.DisplayText
                //LexicalForm = crisResult.LexicalForm,
                //InverseTextNormalizationResult = crisResult.InverseTextNormalizationResult,
                //MaskedInverseTextNormalizationResult = crisResult.MaskedInverseTextNormalizationResult,
            };
            return result;
        }

        public string Confidence { get; private set; }
        public string DisplayText { get; private set; }
        //public string LexicalForm { get; private set; }
        //public string InverseTextNormalizationResult { get; private set; }
        //public string MaskedInverseTextNormalizationResult { get; private set; }

        private SpeechRecognizedPhrase() { }
    }
}
