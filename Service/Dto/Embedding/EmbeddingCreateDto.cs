namespace TalkWithAyodeji.Service.Dto.Embedding
{
    public class EmbeddingCreateDto
    {
        public string input { get; set; }
        public string model { get; set; }
        public int dimensions { get; set; }
    }
}
