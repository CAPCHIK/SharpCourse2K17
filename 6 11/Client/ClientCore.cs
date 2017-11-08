using System.Net.Sockets;
using System.Net;
using System;
using Client.ConsoleWrappers;
using Client.ConsoleWrappers.UserInputs;
using Shared.Models.Messages;
using Newtonsoft.Json;
using System.Text;
using Shared.Models.Responses;

namespace Client
{
    public class ClientCore
    {
        private readonly NetworkWrapper networkManager = new NetworkWrapper();

        public void Work()
        {
            networkManager.FindServer();

            EnterInSystem();

            var chatForm = new ChatWrapper();
            networkManager.ResponseRecieved += (token, status) => {
                switch(status)
                {
                    case StatusCode.TextMessage:
                        chatForm.AddMessage(token.ToObject<TextResponse>());
                        break;
                }
            };
            while(true)
            {
                switch(chatForm.ReadInput())
                {
                    case StringInput str:
                        chatForm.AddSelfMessage(str.InputString);
                        networkManager.SendMessage(new TextMessage(str.InputString));
                        break;
                    case ActionInput act when (act.ActionIndex == 1):
                        chatForm.StopRendering();
                        ShowOnlineUsers();
                        chatForm.StartRendering();
                        break;
                }

            }
        }

        private void ShowOnlineUsers()
        {
            var usersOnline = new OnlineUsersWrapper();
            usersOnline.SetUsersList(networkManager.GetUsersList());
            usersOnline.WaitInput();
        }

        private void EnterInSystem()
        {
            var firstChoose = new LoginOrRegisterWrapper().GetUserChoose() as ActionInput;
            switch (firstChoose.ActionIndex)
            {
                case 1:
                    HandleLogin();
                    break;
                case 2:
                    HandleRegister();
                    break;
            }
        }

        private void HandleRegister(string message = "Registration")
        {
            var registerWrapper = new LoginPasswordPairWrapper(message);
            var pair = registerWrapper.GetPair();
            var registerMessage = new RegistrationMessage()
            {
                UserName = pair.Login,
                UserPassword = pair.Password
            };
            networkManager.SendMessage(registerMessage);
            var response = networkManager.GetResponse();
            switch (response.StatusCode)
            {
                case StatusCode.OK:
                    return;
                case StatusCode.UserNameBusy:
                    HandleRegister($"Name {pair.Login} is busy");
                    return;
                default:
                    throw new Exception($"Status code is bad :( {response.StatusCode}");
            }
        }

        private void HandleLogin(string message = "Login")
        {
            var loginWrapper = new LoginPasswordPairWrapper(message);
            var pair = loginWrapper.GetPair();
            var LoginMessage = new LoginMessage()
            {
                Name = pair.Login,
                Password = pair.Password
            };
            networkManager.SendMessage(LoginMessage);
            var response = networkManager.GetResponse();
            switch (response.StatusCode)
            {
                case StatusCode.OK:
                    return;
                case StatusCode.IncorrectLoginOrPassword:
                    HandleLogin($"incorrect login or password");
                    return;
                default:
                    throw new Exception($"Status code is bad :( {response.StatusCode}");
            }
        }
    }
}