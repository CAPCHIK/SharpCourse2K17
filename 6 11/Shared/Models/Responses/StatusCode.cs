namespace Shared.Models.Responses
{
    //Перечисление, показывает код ответа от сервера
    public enum StatusCode
    {
        //Все хорошо, логин или регистрация прошли успешно
        OK,
        //Неизвестная ошибка в проекте не используется
        UnknownError,
        //Имя пользователя занято, может быть при регистрации
        UserNameBusy,
        //Текстовое сообщение от третьего лица
        TextMessage,
        //Список пользователей онлайн
        OnlineUsers,
        //Неверная пара логин/пароль, при логине
        IncorrectLoginOrPassword
    }
}