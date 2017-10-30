using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace ChatClient
{
    class Program
    {
        static UdpClient client = new UdpClient(5000);
        static void Main(string[] args)
        {
            client.Client.ReceiveTimeout = 1000;
            string message;
            CancellationTokenSource source = new CancellationTokenSource();
            var thread = new Thread(() => Listen(source.Token));
            thread.Start();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            while ((message = Console.ReadLine()) != "exit")
            {
                var data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, endPoint);
            }
            source.Cancel();
            System.Console.WriteLine("THE BIG END");
        }

        static void Listen(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    IPEndPoint point = default(IPEndPoint);
                    var data = client.Receive(ref point);
                    var message = Encoding.UTF8.GetString(data);
                    System.Console.WriteLine($"getted {message}");
                }
                catch
                {
                    if(token.IsCancellationRequested)
                    {
                        System.Console.WriteLine("Second thread dead");
                        return;
                    }
                    System.Console.WriteLine("I have exception");
                }
            }
        }
    }
}
