using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;

using TalkWithAyodeji.Service.Dto.Message;
using TalkWithAyodeji.Service.Dto.Response;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _config;
        private readonly IChatCompletionService _chatClient;
        private readonly IEmbeddingService _embeddingService;
        private readonly IRedisService _redisService;
        private readonly IQdrantService _qdrantService;
        private ChatHistory? chatHistory = new();
        public AIService(IConfiguration config, IChatCompletionService chatClient, IEmbeddingService embeddingService,
            IRedisService redisService, IQdrantService qdrantService)
        {
            _config = config;
            _chatClient = chatClient;
            _embeddingService = embeddingService;
            _redisService = redisService;
            _qdrantService = qdrantService;
        }

        

        public async Task<ServiceResponseDto<string>> AskAI(string question, string connectionId)
        {
            //FETCH CACHED HISTORY IF THERE IS ANY
            string? serializedChatHistory = await _redisService.GetData<string>($"chatHistory_{connectionId}");

            try
            {
                if (serializedChatHistory is null)
                {

                    //EMBED THE QUESTION
                    var embeddingResult = await _embeddingService.CreateQueryEmbedding(question, 64);

                    //FETCH SIMILAR POINTS FROM THE VECTOR STORE
                    var searchResult = await _qdrantService.SearchVector("TalkWithAyodeji", embeddingResult.Data, 5);

                    //GATHER THE TEXTS IN EACH POINT AND ADD IT TO THE CHATBOT SYSTEM PROMPT
                    string context = string.Join("\n", searchResult.Data);
                    var genericPrompt = await InitializeSystemPrompt(context);

                    //INITIALIZE THE CHAT HISTORY
                    ChatHistory newChatHistory = new ChatHistory();
                    chatHistory = newChatHistory;

                    //ADD THE SYSTEM PROMPT TO THE CHAT HISTORY
                    chatHistory.AddSystemMessage(genericPrompt.Data);
                }
                else if (serializedChatHistory != null)
                {
                    //DESERIALIZED CHAT HISTORY INTO AN OBJECT
                    var deserializedChatHistory = JsonConvert.DeserializeObject<ChatHistoryDto>(serializedChatHistory);

                    // Reconstruct ChatHistory
                    foreach (var msg in deserializedChatHistory.Messages)
                    {
                        chatHistory.AddMessage(msg.Role, msg.Content);
                    }

                    //EMBED THE QUESTION
                    var embeddingResult = await _embeddingService.CreateQueryEmbedding(question, 64);

                    //FETCH SIMILAR POINTS FROM THE VECTOR STORE
                    var searchResult = await _qdrantService.SearchVector("TalkWithAyodeji", embeddingResult.Data, 5);

                    //GATHER THE TEXTS IN EACH POINT AND ADD IT TO THE CHATBOT SYSTEM PROMPT
                    string context = string.Join("\n", searchResult.Data);

                    //ADD THE SYSTEM PROMPT TO THE CHAT HISTORY
                    chatHistory.AddSystemMessage(context);

                }
                //ADD USER QUESTION TO THE CHAT HISTORY
                chatHistory.AddUserMessage(question);
                var response = await _chatClient.GetChatMessageContentAsync(chatHistory);
                if (response == null)
                {
                    return ServiceResponseDto<string>.ErrorResponse("Something went wrong when asking AI the question", default, response.Content);
                }

                // ADD THE AI RESPONSE TO THE CHAT HISTORY
                chatHistory.Add(response);

                //MAP CHAT MESSAGE AND ROLE TO A NEW OBJECT
                var dto = new ChatHistoryDto
                {
                    Messages = chatHistory.Select(m => new ChatMessageDto
                    {
                        Role = m.Role,
                        Content = m.Content
                    }).ToList()
                };
                var json = JsonConvert.SerializeObject(dto);

                //CACHE THE RESPONSE
                await _redisService.SetData<string>($"chatHistory_{connectionId}", json);

                //RETURN RESPONSE
                return ServiceResponseDto<string>.SuccessResponse("Success", response.Content);

            }
            catch (Exception ex)
            {
                return ServiceResponseDto<string>.ErrorResponse("An unexpected error occured while asking AI the question", default, ex.Message);
            }
        }

        public async Task<ServiceResponseDto<string>> InitializeSystemPrompt(string data)
        {
            string filePath = ".\\Service\\Helpers\\GenericPrompt2.txt";
            string fileContent = await File.ReadAllTextAsync(filePath);
            string updatedContent = fileContent.Replace("{context}", data);
            return ServiceResponseDto<string>.SuccessResponse("Successfully fetched prompt", updatedContent);
        }
    }
}
