using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using TalkWithAyodeji.Data.DatabaseObject;
using TalkWithAyodeji.Service;
using TalkWithAyodeji.Service.Interface;

namespace TalkWithAyodeji.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IAIClient _aiClient;

        public ChatHub(ILogger<ChatHub> logger, IAIClient aiClient)
        {
            _logger = logger;
            _aiClient = aiClient;
        }

        public async Task AskAIQuestion(string question, string chatName)
        {
            //ASK AI THE QUESTION
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", question, Context.ConnectionId);
            _logger.LogInformation($"User asked the chatbot a question : {question}");

            //GET RESPONSE FROM AI

            var response = await _aiClient.AskAI(question, Context.ConnectionId);
            _logger.LogInformation($"Chatbot response: {response}");

            //SEND THE RESPONSE TO THE GROUP
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "Ayodeji", response, "System Connection Id");
        }
    }
}
