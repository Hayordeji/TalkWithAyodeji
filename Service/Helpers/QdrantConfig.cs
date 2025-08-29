namespace TalkWithAyodeji.Service.Helpers
{
    public class QdrantConfig
    {
        public string Endpoint { get; set; }
        public string? ApiKey { get; set; }
        public int Timeout { get; set; } = 30;
        public int MaxRetries { get; set; } = 3;
    }
}
