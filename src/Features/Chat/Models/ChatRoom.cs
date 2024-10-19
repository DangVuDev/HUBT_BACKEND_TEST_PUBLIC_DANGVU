using System.Collections.Generic;

namespace HUBTSOCIAL.src.Features.Chat.Models
{
    public class ChatRoom
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public List<string> UserIds { get; set; } = new List<string>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
