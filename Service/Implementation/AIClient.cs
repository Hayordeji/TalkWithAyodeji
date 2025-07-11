using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Service.Implementation
{
    public class AIClient : IAIClient
    {
        public Task<string> AskAI(string question, string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<string> InitializeSystemPrompt(string data)
        {
            throw new NotImplementedException();
        }
    }
}
