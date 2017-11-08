namespace Shared.Models.Messages
{
    public class RegistrationMessage : Message
    {
        public override MessageType MessageType => MessageType.Register;
        public string UserName {get; set;}
        public int UserPassword {get; set;}
    }
}