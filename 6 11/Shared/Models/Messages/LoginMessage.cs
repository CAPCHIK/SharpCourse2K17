namespace Shared.Models.Messages
{
    public class LoginMessage : Message
    {
        public override MessageType MessageType => MessageType.Login;

        public string Name {get; set;}
        public int Password {get; set;}
    }
}