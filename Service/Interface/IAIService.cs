using TalkWithAyodeji.Service.Dto.Response;

namespace TalkWithAyodeji.Service.Interface
{
    public interface IAIService
    {
        //Task<ApiResponseDto<string>> AskAI(string question, string connectionId);
        Task<ServiceResponseDto<string>> AskAI(string question, string connectionId);

        Task<ServiceResponseDto<string>> InitializeSystemPrompt(string data);
    }
}
