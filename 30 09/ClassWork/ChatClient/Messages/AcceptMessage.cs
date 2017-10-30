using System;

namespace ChatClient.Messages
{
    public class AcceptMessage : Message
    {
        public Guid ReceivedMessageId {get; set;}

        public override MessageType MessageType => MessageType.AcceptMessage;
    }
}