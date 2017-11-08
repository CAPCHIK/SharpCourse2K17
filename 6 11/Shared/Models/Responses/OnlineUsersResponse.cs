using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.Responses
{
    public class OnlineUsersResponse : Response
    {
        public List<string> UserNames { get; }
        [JsonConstructor]
        public OnlineUsersResponse(List<string> userNames) : base(StatusCode.OnlineUsers)
        {
            UserNames = userNames;
        }

    }
}
