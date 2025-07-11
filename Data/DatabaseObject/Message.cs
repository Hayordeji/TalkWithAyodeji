using System.ComponentModel.DataAnnotations;

namespace TalkWithAyodeji.Data.DatabaseObject
{
    public class Message
    {
        [Key]
        public string Id { get; set; }
        public string Role { get; set; }
        public string Content { get; set; }
    }
}
