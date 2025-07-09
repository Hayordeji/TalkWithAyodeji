using System.ComponentModel.DataAnnotations;

namespace TalkWithAyodeji.Data.DatabaseObject
{
    public class AIChatHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public string? ChatHistory { get; set; }
    }
}
