namespace TalkWithAyodeji.Service
{
    public interface IAIClient
    {
        Task<string> AskAI(string question, string connectionId);

        Task<string> InitializeSystemPrompt(string data);
    }
}
