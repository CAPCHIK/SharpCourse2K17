using System;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Shared.Models.Responses;
using Shared.Models.Users;

namespace Server.Models
{
    public class RemoteClient
    {
        
        private readonly Action<Response> sendResponse;

        public User UserView { get; set; }
        public event Action<JToken> MessageReceived;

        public RemoteClient(Action<Response> sendResponse)
        {
            this.sendResponse = sendResponse;
        }

        public void Send(Response response) =>
                    sendResponse(response);

        public void Notify(JToken token)
        {
            MessageReceived?.Invoke(token);
        }
    }
}
