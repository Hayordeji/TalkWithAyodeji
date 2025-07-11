using System.ComponentModel.DataAnnotations;

namespace TalkWithAyodeji.Data.DatabaseObject
{
    public class AIChatHistory
    {
        [Key]
        public string? Id { get; set; }
        public string? ConnectionId { get; set; } = string.Empty;
        public List<Message>? MessageList { get; set; }
    }
}
