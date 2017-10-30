using System;

namespace ChatClient.Messages
{
    public class TextMessage : Message
    {
        public Guid Id {get; set;}
        public string Text {get; set;}
    }
}