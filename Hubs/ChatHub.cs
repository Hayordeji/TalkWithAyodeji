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
        private readonly IAIService _aiClient;

        public ChatHub(ILogger<ChatHub> logger, IAIService aiClient)
        {
            _logger = logger;
            _aiClient = aiClient;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId; 

            _logger.LogInformation($"A new user with connection ID : {connectionId} has connected to the chat");
            await base.OnConnectedAsync();
        }

        public async Task AskAIQuestion(string question)
        {
            _logger.LogInformation($"A user asked the chatbot a question : {question}");

            //GET RESPONSE FROM AI
            var response = await _aiClient.AskAI(question, Context.ConnectionId);

            //SEND THE RESPONSE TO THE CHAT
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", response.Data, response.Data.ToString());
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            string connectionId = Context.ConnectionId;

            _logger.LogTrace($"A user has been disconnected with ID: {connectionId}");
            await base.OnDisconnectedAsync(ex);
        }
        


    }
}
