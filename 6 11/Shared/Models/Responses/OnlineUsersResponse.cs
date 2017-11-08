using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.Responses
{
    //Ответ, хранящий список имен пользователей онлайн
    public class OnlineUsersResponse : Response
    {
        //Свойство список имен
        public List<string> UserNames { get; }
        //Конструктор, аргумент или руками или библиотекой JSON
        [JsonConstructor]
        public OnlineUsersResponse(List<string> userNames) : base(StatusCode.OnlineUsers)
        {
            UserNames = userNames;
        }

    }
}
