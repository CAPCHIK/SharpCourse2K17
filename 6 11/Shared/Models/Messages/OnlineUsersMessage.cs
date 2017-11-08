using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.Messages
{
    public class OnlineUsersMessage : Message
    {
        public override MessageType MessageType => MessageType.OnlineRequest;
    }
}
