namespace Shared.Models.Messages
{
    //Сообщение запрос на вход
    public class LoginMessage : Message
    {
        //Тип сообщения - логин
        public override MessageType MessageType => MessageType.Login;
        //Тип поё имя
        public string Name {get; set;}
        //Тип мой пароль
        public int Password {get; set;}
    }
}