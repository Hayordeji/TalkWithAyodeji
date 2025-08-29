using Newtonsoft.Json;

namespace TalkWithAyodeji.Service.Dto.Embedding
{
    public class OpenAIEmbeddingResponseDto
    {
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("data")]
        public List<Embedding> Data { get; set; }
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("usage")]
        public Usage Usage { get; set; }

    }

    public class Embedding
    {
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("embedding")]
        public List<float> embedding { get; set; }
        [JsonProperty("index")]
        public int Index { get; set; }
    }

    public class Usage
    {
        [JsonProperty("prompt_tokens")]
        public int Prompt_Tokens { get; set; }
        [JsonProperty("total_tokens")]
        public int Total_Tokens { get; set; }

    }
}
