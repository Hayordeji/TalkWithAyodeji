using Newtonsoft.Json;
using OpenAI;
using OpenAI.Embeddings;
using Qdrant.Client.Grpc;
using TalkWithAyodeji.Service.Dto.Embedding;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;
using TalkWithAyodeji.Service.Mapper;
using tryAGI.OpenAI;

namespace TalkWithAyodeji.Service.Implementation
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmbeddingService> _logger;
        private readonly IHttpClientService _httpClient;
        private readonly OpenAIClient _AIClient;
        private readonly OpenAiClient _openAiClient;
        public EmbeddingService(IConfiguration config, ILogger<EmbeddingService> logger, IHttpClientService httpClient, OpenAIClient aIClient, OpenAiClient openAiClient)
        {
            _config = config;
            _logger = logger;
            _httpClient = httpClient;
            _AIClient = aIClient;
            _openAiClient = openAiClient;
        }

        //public async Task<ServiceResponseDto<OpenAIEmbeddingResponseDto>> CreateEmbedding(string text, int dimensions)
        //{
        //    try
        //    {
        //        //CREATE TEH REQUEST BODY
        //        var data = new EmbeddingCreateDto()
        //        {
        //            input = text,
        //            model = "text-embedding-3-small",
        //            //dimensions = dimensions
        //        };
        //        //SET THE AUTHORIZATION HEADER
        //       _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
        //        var response = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
        //        response.EnsureSuccessStatusCode();

        //        //READ THE RESPONSE CONTENT
        //        var responseContent = await response.Content.ReadAsStringAsync();
        //        var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);

        //        return ServiceResponseDto<OpenAIEmbeddingResponseDto>.SuccessResponse("Text sucessfully embedded", deserializedReponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

        //        return ServiceResponseDto<OpenAIEmbeddingResponseDto>.ErrorResponse("An unexpected error occured while embedding",default, ex.Message);
        //    }
        //}

        public async Task<ServiceResponseDto<List<PointStruct>>> CreateEmbeddings(List<string> texts, int dimensions)
        {
            try
            {
                List<PointStruct> points = new List<PointStruct>();
                foreach (string text in texts)
                {

                    var floats = await CreateQueryEmbedding(text, dimensions);

                    var embedding = floats.Data.ToEmbeddingStoreDto(text);
                    points.Add(embedding);
                }

                return ServiceResponseDto<List<PointStruct>>.SuccessResponse("Succesfully created all embeddings", points);

            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

                return ServiceResponseDto<List<PointStruct>>.ErrorResponse("An unexpected error occured while creating embeddings", default, ex.Message);
            }
        }

        //public async Task<ServiceResponseDto<List<float>>> CreateQueryEmbedding(string query, int dimensions)
        //{
        //    try
        //    {
        //        OpenAIEmbeddingResponseDto response = new OpenAIEmbeddingResponseDto();

        //        //CREATE TEH REQUEST BODY
        //        var data = new EmbeddingCreateDto()
        //        {
        //            input = query,
        //            //dimensions = dimensions,
        //            model = "text-embedding-3-small",
        //        };
                    
        //        //SET THE AUTHORIZATION HEADER
        //        _httpClient.SetAuthorizationHeader("bearer", $"{_config["OpenAI:APIKey"]}");
        //        var result = await _httpClient.PostAsync<EmbeddingCreateDto>("https://api.openai.com/v1/embeddings", data);
        //        result.EnsureSuccessStatusCode();

        //        //READ THE RESPONSE CONTENT
        //        var responseContent = await result.Content.ReadAsStringAsync();
        //        var deserializedReponse = JsonConvert.DeserializeObject<OpenAIEmbeddingResponseDto>(responseContent);

        //        var embedding = deserializedReponse.ToEmbeddingQueryDto();

        //        return ServiceResponseDto<List<float>>.SuccessResponse("Succesfully created all embeddings", embedding);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"An unexpected error occured while embedding.... Error : {ex.Message}.... StackTrace : {ex.StackTrace}");

        //        return ServiceResponseDto<List<float>>.ErrorResponse("An unexpected error occured while creating embeddings", default, ex.Message);
        //    }
        //}

        public async Task<ServiceResponseDto<List<float>>> CreateQueryEmbedding(string query, int dimensions)
        {
           
            var result2 = await _openAiClient.Embeddings.CreateEmbeddingAsync(new CreateEmbeddingRequest
            {
                Input = query,
                Model = "text-embedding-3-small",
                Dimensions = 64
            });

            var tooo = result2.Data.Select(x => x.Embedding1).First();
            List<float> floats = tooo.Select(x => (float)x).ToList();

            
            return ServiceResponseDto< List<float>>.SuccessResponse("Successful", floats);
        }
    }
}
