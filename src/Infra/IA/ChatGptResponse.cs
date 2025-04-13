using System.Collections.Generic;

namespace Infra.IA;

public class ChatGptResponse
{
    public List<Choice> Choices { get; set; }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
