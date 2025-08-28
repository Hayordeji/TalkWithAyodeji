using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IQdrantService
    {
        Task<ServiceResponseDto<bool>> CreateCollection(string collectionName, uint vectorSize);
        Task<ServiceResponseDto<bool>> AddVectorsToCollection(string collectionName, List<PointStruct> embeddings);
        //Task AddVectorsToCollection(string collectionName, List<OpenAIEmbeddingResponseDto> embeddings);
        Task<ServiceResponseDto<bool>> DeleteCollection(string collectionName);

        Task<ServiceResponseDto<List<string>>> SearchVector(string collectionName, List<float> vector, int limit = 5);
    }
}
