using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.Messages
{
    //Запрос на список людей онлайн
    public class OnlineUsersMessage : Message
    {
        //Тип - запрос на людей онлайн
        public override MessageType MessageType => MessageType.OnlineRequest;
    }
}
