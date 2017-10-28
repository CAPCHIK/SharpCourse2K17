using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDP_To_All
{
    class Program
    {
        
        static UdpClient client;
        static void Main(string[] args)
        {
            client = new UdpClient(5000);
            CancellationTokenSource source = new CancellationTokenSource();
            var listenTask = Task.Factory.StartNew(() => Listen(source.Token));
            string message;
            IPEndPoint toAllEndPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            while((message = Console.ReadLine()) != "exit")
            {
                var data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, toAllEndPoint);
            }
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
                catch (Exception ex)
                {
                    System.Console.WriteLine($"WFT? Exception? {ex.GetType().FullName} : {ex.Message}");
                    return;
                }
            }
        }
    }
}
