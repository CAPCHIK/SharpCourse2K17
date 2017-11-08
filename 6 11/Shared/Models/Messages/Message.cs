using Newtonsoft.Json;

namespace Shared.Models.Messages
{
    public class Message
    {
        public virtual MessageType MessageType { get; } =  MessageType.Unkwnown;

        [JsonConstructor]
        public Message(MessageType messageType)
        {
            MessageType = messageType;
        }
        public Message()
        {

        }
    }
}