using Newtonsoft.Json;
using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Embedding;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;
using TalkWithAyodeji.Service.Mapper;

namespace TalkWithAyodeji.Service.Implementation
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmbeddingService> _logger;
        private readonly IHttpClientService _httpClient;
        public EmbeddingService(IConfiguration config,ILogger<EmbeddingService> logger ,IHttpClientService httpClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<ServiceResponseDto<OpenAIEmbeddingResponseDto>> CreateEmbedding(string text, int dimensions)
        {
            try
            {
                //CREATE TEH REQUEST BODY
                var data = new EmbeddingCreateDto()
                {
                    input = text,
                    model = "text-embedding-3-small",
                    dimensions = dimensions
                };
                //SET THE AUTHORIZATION HEADER
               _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
                var response = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
                response.EnsureSuccessStatusCode();

                //READ THE RESPONSE CONTENT
                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);

                return ServiceResponseDto<OpenAIEmbeddingResponseDto>.SuccessResponse("Text sucessfully embedded", deserializedReponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<OpenAIEmbeddingResponseDto>.ErrorResponse("An unexpected error occured while embedding",default, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<List<PointStruct>>> CreateEmbeddings(List<string> texts, int dimensions)
        {
            try
            {
                List<PointStruct> points = new List<PointStruct>();
                foreach (string text in texts)
                {

                    //CREATE TEH REQUEST BODY
                    var data = new EmbeddingCreateDto()
                    {
                        input = text,
                        model = "text-embedding-3-small",
                        dimensions = dimensions
                    };

                    //SET THE AUTHORIZATION HEADER
                    _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
                    var response = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
                    response.EnsureSuccessStatusCode();

                    //READ THE RESPONSE CONTENT
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);

                    var embedding = deserializedReponse.ToEmbeddingStoreDto(text);
                    points.Add(embedding);
                }

                return ServiceResponseDto<List<PointStruct>>.SuccessResponse("Succesfully created all embeddings",points );

            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<PointStruct>>.ErrorResponse("An unexpected error occured while creating embeddings", default, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<List<float>>> CreateQueryEmbedding(string query, int dimensions)
        {
            try
            {
                OpenAIEmbeddingResponseDto response = new OpenAIEmbeddingResponseDto();

                //CREATE TEH REQUEST BODY
                var data = new EmbeddingCreateDto()
                {
                    input = query,
                    model = "text-embedding-3-small",
                    dimensions = dimensions
                };

                //SET THE AUTHORIZATION HEADER
                _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
                var result = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
                result.EnsureSuccessStatusCode();

                //READ THE RESPONSE CONTENT
                var responseContent = await result.Content.ReadAsStringAsync();
                var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);

                var embedding = deserializedReponse.ToEmbeddingQueryDto();

                return ServiceResponseDto<List<float>>.SuccessResponse("Succesfully created all embeddings", embedding);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<float>>.ErrorResponse("An unexpected error occured while creating embeddings", default, ex.Message);
            }
        }
    }
}
