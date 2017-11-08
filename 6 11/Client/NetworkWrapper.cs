using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.ConsoleWrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Models.Messages;
using Shared.Models.Responses;
using System.Collections.Generic;

namespace Client
{
    public class NetworkWrapper
    {
        private readonly TcpClient tcpClient = new TcpClient();
        private readonly object sync = new object();
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        public event Action<JToken, StatusCode> ResponseRecieved;

        public void FindServer()
        {
            var data = new byte[] { 1 };
            var endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            int tryNum = 1;
            var placeHolder = new PlaceHolderWrapper($"Finding server try #{tryNum}");
            var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 1000;
            while (true)
            {
                try
                {
                    udpClient.Send(data, data.Length, endPoint);
                    data = udpClient.Receive(ref endPoint);
                    break;
                }
                catch
                {
                    tryNum++;
                    placeHolder.Content = $"Finding server try #{tryNum}";
                }
            }
            udpClient.Dispose();
            int targetPort = BitConverter.ToInt32(data, 0);
            tcpClient.Connect(endPoint.Address, targetPort);
            Task.Factory.StartNew(ListenConnection);
        }



        public void SendMessage(Message message)
        {

            var jsonString = JsonConvert.SerializeObject(message);
            var data = Encoding.UTF8.GetBytes(jsonString);
            var dataLength = BitConverter.GetBytes(data.Length);
            lock (sync)
            {
                tcpClient.GetStream().Write(dataLength, 0, dataLength.Length);
                tcpClient.GetStream().Write(data, 0, data.Length);
            }
        }

        public List<string> GetUsersList()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            List<string> names = new List<string>();

            Action<JToken, StatusCode> lamda = (obj, statusCode) =>
            {
                if (statusCode == StatusCode.OnlineUsers)
                {
                    names = obj.ToObject<OnlineUsersResponse>().UserNames;
                    resetEvent.Set();
                }
            };
            ResponseRecieved += lamda;
            SendMessage(new OnlineUsersMessage());
            resetEvent.Wait();
            ResponseRecieved -= lamda;
            return names;
        }

        public Response GetResponse()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Response response = null;

            Action<JToken, StatusCode> lamda = (obj, statusCode) =>
            {
                response = obj.ToObject<Response>();
                resetEvent.Set();

            };
            ResponseRecieved += lamda;
            resetEvent.Wait();
            ResponseRecieved -= lamda;
            return response;
        }


        private async Task ListenConnection()
        {
            var stream = tcpClient.GetStream();
            var responseLengthBuffer = new byte[sizeof(int)];
            var responseBuffer = new byte[4096];
            while(!tokenSource.Token.IsCancellationRequested)
            {
                await stream.ReadAsync(responseLengthBuffer, 0, sizeof(int), tokenSource.Token);
                var responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);
                await stream.ReadAsync(responseBuffer, 0, responseLength, tokenSource.Token);
                var jsonString = Encoding.UTF8.GetString(responseBuffer, 0, responseLength);
                var token = JToken.Parse(jsonString);
                ResponseRecieved?.Invoke(token, GetStatusCode(token));
            }
        }
        private StatusCode GetStatusCode(JToken token) => token.ToObject<Response>().StatusCode;

    }
}
