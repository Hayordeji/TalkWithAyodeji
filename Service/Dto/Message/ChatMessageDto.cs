using Microsoft.SemanticKernel.ChatCompletion;

namespace TalkWithAyodeji.Service.Dto.Message
{
    public class ChatMessageDto
    {
        public AuthorRole Role { get; set; }
        public string Content { get; set; }
    }
}
