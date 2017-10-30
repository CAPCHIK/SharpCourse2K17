using System;
using Newtonsoft.Json;

namespace ChatClient.Messages
{
    public class TextMessage : Message
    {
        public Guid Id {get; set;}
        public string Text {get; set;}

        [JsonIgnore]
        public int ReadedCount {get; set;}

        public override MessageType MessageType => MessageType.TextMessage;
    }
}