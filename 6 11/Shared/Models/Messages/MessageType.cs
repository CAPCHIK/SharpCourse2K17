namespace Shared.Models.Messages
{
    //Типы сообщений к серверу
    public enum MessageType
    {
        //Неизвестный тип
        Unkwnown,
        //Текстовое сообщение
        Text,
        //Подтверждение получения сообщения
        //Не используется
        TextAccept,
        //Запрос на список онлайн людей
        OnlineRequest,
        //Запрос на вход
        Login,
        //Запрос на регистрацию
        Register
    }
}