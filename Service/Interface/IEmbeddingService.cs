using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Embedding;
using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IEmbeddingService
    {
        Task<ServiceResponseDto<List<float>>> CreateQueryEmbedding(string query, int dimensions);
        Task<ServiceResponseDto<List<PointStruct>>> CreateEmbeddings(List<string> texts, int dimensions);

    }
}
