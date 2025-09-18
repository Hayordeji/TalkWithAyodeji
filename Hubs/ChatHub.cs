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
            string connectionId = Context.ConnectionId; // Example: "abc123def456"

            _logger.LogInformation($"User connected with ID: {connectionId}");
            await base.OnConnectedAsync();
        }

        public async Task AskAIQuestion(string question)
        {
            //ASK AI THE QUESTION
            //await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", question, Context.ConnectionId);
            _logger.LogInformation($"User asked the chatbot a question : {question}");

            //GET RESPONSE FROM AI

            var response = await _aiClient.AskAI(question, Context.ConnectionId);
            _logger.LogInformation($"Chatbot response: {response}");

            //SEND THE RESPONSE TO THE GROUP
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", response.Data);
        }

        //public async Task SendPersonalMessage(string message)
        //{
        //    string connectionId = Context.ConnectionId;

        //    await Clients.Client(connectionId).SendAsync("ReceiveMessage", message, Context.ConnectionId);
        //    _logger.LogInformation($"You : {message}");

        //    //await _chatService.AddPersonalMessage(user, "ReceipientName", message);
        //    //await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}


    }
}
