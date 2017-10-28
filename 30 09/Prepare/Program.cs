using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Prepare
{
    class Program
    {
        static UdpClient client;
        static void Main(string[] args)
        {
            client = new UdpClient(5000);
            System.Console.WriteLine(client.Client.ReceiveTimeout);
            client.Client.ReceiveTimeout = 5000;
            string message;
            System.Console.WriteLine("START");
            CancellationTokenSource source = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => Listen(source.Token));
            while ((message = Console.ReadLine()) != "exit")
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                client.Send(messageBytes,
                messageBytes.Length,
                new IPEndPoint(IPAddress.Loopback, 5000));
            }
            source.Cancel();
            task.Wait();
            client.Dispose();
            System.Console.WriteLine("Program was stopped");
        }

        static void Listen(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    IPEndPoint point = default(IPEndPoint);
                    var data = client.Receive(ref point);
                    System.Console.WriteLine(Encoding.UTF8.GetString(data));

                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    if (token.IsCancellationRequested)
                        return;
                    System.Console.WriteLine("timeout, but go to new iteration");
                }
            }
        }
    }
}
