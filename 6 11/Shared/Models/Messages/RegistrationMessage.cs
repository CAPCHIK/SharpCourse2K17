namespace Shared.Models.Messages
{
    //Сообщение - запрос на регистрацию
    public class RegistrationMessage : Message
    {
        //Тип - регистрация
        public override MessageType MessageType => MessageType.Register;
        //Желаемое имя пользователя
        public string UserName {get; set;}
        //Желаемый пароль
        public int UserPassword {get; set;}
    }
}