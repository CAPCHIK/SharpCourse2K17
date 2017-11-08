using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Text;
using Server.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Shared.Models.Messages;
using System.Linq;
using Shared.Models.Responses;
using Shared.Models.Users;

namespace Server
{
    public class ServerCore
    {
        private DataStorage storage;
        private NetworkWrapper networkManager;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private List<RemoteClient> clients = new List<RemoteClient>();

        public ServerCore()
        {
            storage = new DataStorage();
            networkManager = new NetworkWrapper(tokenSource.Token);
            networkManager.ClientAccepted += ClientAccepted;
        }

        public void StartWork()
        {
            networkManager.Start();
        }

        public void Stop()
        {
            tokenSource.Cancel();
        }

        private void ClientAccepted(RemoteClient client)
        {
            clients.Add(client);
            client.MessageReceived += T => MessageReceived(client, T);
        }

        private void MessageReceived(RemoteClient client, JToken token)
        {
            var message = token.ToObject<Message>();
            Console.WriteLine($"Message type is {message.MessageType}");
            switch(message.MessageType)
            {
                case MessageType.Login:
                    HandleLogin(client, token.ToObject<LoginMessage>());
                    break;
                case MessageType.Register:
                    HandleRegistration(client, token.ToObject<RegistrationMessage>());
                    break;
                case MessageType.Text when (client.UserView != null):
                    SendTextMessage(client, token.ToObject<TextMessage>());
                    break;
                case MessageType.OnlineRequest:
                    SendUsersOnline(client);
                    break;
            }
        }

        private void SendUsersOnline(RemoteClient client)
        {
            var usernames = clients
                .Where(C => C.UserView != null)
                .Select(C => C.UserView.Name)
                .ToList();
            client.Send(new OnlineUsersResponse(usernames));
        }

        private void HandleLogin(RemoteClient client, LoginMessage loginMessage)
        {
            var targetaccount = storage.Find(U => U.Name == loginMessage.Name && U.Password == loginMessage.Password);
            if(targetaccount == null)
            {
                client.Send(new Response(StatusCode.IncorrectLoginOrPassword));
                return;
            }
            client.UserView = targetaccount;
            client.Send(Response.GoodResponse);
        }

        private void HandleRegistration(RemoteClient client, RegistrationMessage registrationMessage)
        {
            var userWithSomwName = storage.Find(U => U.Name == registrationMessage.UserName);
            if (userWithSomwName != null)
            {
                client.Send(new Response(StatusCode.UserNameBusy));
                return;
            }
            User user = new User
            {
                Name = registrationMessage.UserName,
                Password = registrationMessage.UserPassword
            };
            storage.RegisterUser(user);
            client.UserView = user;
            client.Send(Response.GoodResponse);
        }

        private void SendTextMessage(RemoteClient client, TextMessage textMessage)
        {
            var response = new TextResponse(client.UserView.Name, textMessage.Content);
            foreach(var targetClient in 
                    clients
                    .Where(U => U.UserView?.Name != client.UserView.Name))
            {
                targetClient.Send(response);
            }
        }
    }
}