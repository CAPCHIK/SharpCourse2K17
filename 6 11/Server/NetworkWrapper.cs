using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using Shared.Models.Responses;

namespace Server
{
    public class NetworkWrapper
    {
        private Task udpTask;
        private Task tcpTask;

        private UdpClient udpClient = new UdpClient(5000);
        private TcpListener tcpListener;

        private int tcpPort;
        private readonly CancellationToken cancellationToken;

        public event Action<RemoteClient> ClientAccepted;

        public NetworkWrapper(CancellationToken cancellationToken)
        {
            for (int port = 5100; ; port++)
                try
                {
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    tcpPort = port;
                    Console.WriteLine($"TCP Listener was start on {port} port");
                    break;
                }
                catch { }

            this.cancellationToken = cancellationToken;

        }

        public void Start()
        {
            udpTask = Task.Factory.StartNew(ListenUDP);
            tcpTask = Task.Factory.StartNew(ListenClients);
        }

        private async Task ListenUDP()
        {
            var data = BitConverter.GetBytes(tcpPort);
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await udpClient.ReceiveAsync();
                await udpClient.SendAsync(data, data.Length, result.RemoteEndPoint);
            }
        }
        private async Task ListenClients()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Waiting client...");
                var client = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Accepted client");
                var t = Task.Factory.StartNew(async () => await HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            RemoteClient remoteClient = new RemoteClient(
                R => SendResponseToClient(R, client));
            ClientAccepted?.Invoke(remoteClient);
            while (!cancellationToken.IsCancellationRequested)
            {
                
                var messageLength = new byte[4];
                var buffer = new byte[4096];
                await client.GetStream().ReadAsync(messageLength, 0, 4);
                var length = BitConverter.ToInt32(messageLength, 0);

                await client.GetStream().ReadAsync(buffer, 0, length);
                var jsonString = Encoding.UTF8.GetString(buffer, 0, length);
                var token = JToken.Parse(jsonString);
                Console.WriteLine("Received message");
                remoteClient.Notify(token);
            }
        }

        private void SendResponseToClient(Response response, TcpClient client)
        {
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
            var dataLengthBytes = BitConverter.GetBytes(data.Length);
            lock(client)
            {
                client.GetStream().Write(dataLengthBytes, 0, dataLengthBytes.Length);
                client.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
