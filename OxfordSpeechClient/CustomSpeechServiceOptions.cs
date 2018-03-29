namespace OxfordSpeechClient
{
    public sealed class CustomSpeechServiceOptions
    {
        public string[] ServiceUrls { get; set; }

        public string AuthenticationUrl { get; set; }

        public string PrimaryKey { get; set; }

        public string SecondaryKey { get; set; }
    }
}
