using Grpc.Net.Client;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class QdrantService : IQdrantService
    {
        private readonly QdrantClient _client;
        private readonly ILogger<QdrantService> _logger;
        private readonly IEmbeddingService _embeddingService;
        private readonly IConfiguration _config;
        public QdrantService(QdrantClient client,ILogger<QdrantService> logger ,IEmbeddingService embeddingService, IConfiguration config)
        {
            _client = client;
            _logger = logger;
            _embeddingService = embeddingService;
            //_client = new QdrantClient(
            //  host: $"{_config["Qdrant:Host"]}",
            //  https: true,
            //  apiKey: $"{_config["Qdrant:ApiKey"]}"
            //);
        }
        public async Task<ServiceResponseDto<bool>> AddVectorsToCollection(string collectionName, List<PointStruct> embeddings)
        {
            try
            {
                //var  mappedEmbedding = embeddings.ToEmbeddingStoreDto();
               var result =  await _client.UpsertAsync(collectionName, embeddings);
                if (result.Status != UpdateStatus.Completed)
                {
                    return ServiceResponseDto<bool>.ErrorResponse($"Unable to add vectors to collection with name : {collectionName}", false);
                }
                return ServiceResponseDto<bool>.SuccessResponse($"Successfully added vectors to collection with name : {collectionName} ", true);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while adding vectors to collection.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<bool>.ErrorResponse($"An unexpected error occured while trying to add vectors to collection with name : {collectionName}", false, ex.Message);
            }

        }

        public async Task<ServiceResponseDto<bool>> CreateCollection(string collectionName, uint vectorSize)
        {
            try
            {

                 await _client.CreateCollectionAsync(collectionName, new VectorParams
                {
                    Size = vectorSize,
                    Distance = Distance.Cosine
                });
                return ServiceResponseDto<bool>.SuccessResponse($"Successfully created collection with name : {collectionName} ", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while creating collection.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<bool>.ErrorResponse($"An unexpected error occured while trying to create collection with name : {collectionName}", false, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<bool>> DeleteCollection(string collectionName)
        {
            try
            {

                await _client.DeleteCollectionAsync(collectionName);
                
                return ServiceResponseDto<bool>.SuccessResponse($"Successfully deleted collection with name : {collectionName} ", true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while deleting collection.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");
                return ServiceResponseDto<bool>.ErrorResponse($"An unexpected error occured while trying to delete collection with name : {collectionName}", false, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<List<string>>> SearchVector(string collectionName, List<float> vector, int limit = 5)
        {
            try
            {
                float[] vectorArray = vector.ToArray(); // Convert List<float> to ReadOnlyMemory<float>
                ReadOnlyMemory<float> queryVector = vectorArray;

                // return the 5 closest points
                var points = await _client.SearchAsync(
                  collectionName,
                  queryVector,
                  limit: (ulong)limit,
                  scoreThreshold: 0.3f // Optional: Set a score threshold for filtering results
                  );
                List<string> texts = new List<string>();
                foreach (ScoredPoint point in points)
                {
                    texts.Add(point.Payload["text"].StringValue);
                }

                return ServiceResponseDto<List<string>>.SuccessResponse("Successfully fetched vectors", texts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while searching for vectors in collection.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<string>>.ErrorResponse($"An unexpected error occured while trying to fetch vectors for collection with name : {collectionName}", default, ex.Message);
            }
        }
    }
}
