namespace TalkWithAyodeji.Service.Interface
{
    public interface IAIClient
    {
        Task<string> AskAI(string question, string connectionId);

        Task<string> InitializeSystemPrompt(string data);
    }
}
