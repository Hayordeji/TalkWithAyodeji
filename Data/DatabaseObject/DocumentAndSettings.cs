using System.ComponentModel.DataAnnotations;

namespace TalkWithAyodeji.Data.DatabaseObject
{
    public class DocumentAndSettings
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? Document { get; set; }
        public string FilePath { get; set; }
    }
}
