using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Embedding;

namespace TalkWithAyodeji.Service.Mapper
{
    public static class EmbeddingMapper
    {
        public static PointStruct ToEmbeddingStoreDto(this List<float> body, string text)
        {

            var vector = body.ToArray();

            return new PointStruct
            {
                Id = Guid.NewGuid(),
                Vectors = vector,
                Payload =
                    {
                        ["text"] = text
                    }
            };
        }

        public static List<float> ToEmbeddingQueryDto(this OpenAIEmbeddingResponseDto body)
        {

            var newEmbedding = new Embedding();
            newEmbedding = body.Data.FirstOrDefault();
            var vector = newEmbedding.embedding.ToArray();

            return vector.ToList();
        }

        public static List<float> ToNormalEmbeddingQueryDto(this OpenAIEmbeddingResponseDto body)
        {

            var newEmbedding = new Embedding();
            newEmbedding = body.Data.FirstOrDefault();
            var vector = newEmbedding.embedding.ToArray();

            return vector.ToList();
        }


        public static List<PointStruct> ToEmbeddingsStoreDto(this List<OpenAIEmbeddingResponseDto> body)
        {

            List<PointStruct> points = new List<PointStruct>();

            foreach (var dto in body)
            {
                var newEmbedding = new Embedding();
                newEmbedding = dto.Data.FirstOrDefault();
                var vector = newEmbedding.embedding.ToArray();
                var newPoint = new PointStruct
                {
                    Id = Guid.NewGuid(),
                    Vectors = vector,
                    Payload =
                    {
                        ["description"] = "Testing the list",
                    }
                };
                points.Add(newPoint);
            }


            return points;
        }
    }
}
