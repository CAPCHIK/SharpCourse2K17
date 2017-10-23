using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Threading;

namespace _23_09
{
    class PingPong
    {
        static UdpClient client;

        public static void Work()
        {
            client = new UdpClient(5001);
            var thr = new Thread(Listen);
            thr.IsBackground = false;
            thr.Start();
            while(true)
            {
                var str = Console.ReadLine();
                if(str == "exit")
                    return;
                IPEndPoint target = new IPEndPoint(IPAddress.Broadcast, 5001);
                var data = Encoding.UTF8.GetBytes("PING");
                client.Send(data, data.Length, target);
            }
        }

        private static void Listen()
        {
            var pongData = Encoding.UTF8.GetBytes("PONG");
            while(true)
            {
                IPEndPoint sender = default(IPEndPoint);
                byte[] data = client.Receive(ref sender);
                string text = Encoding.UTF8.GetString(data);
                if(text == "PING")
                    client.Send(pongData, pongData.Length, sender);
                else
                if(text == "PONG")
                {
                    System.Console.WriteLine($"Yey!! I recieved pong from {sender.Address}");
                }
                else
                    System.Console.WriteLine($"WTF I recieved {text} from {sender.Address}");

            }
        }
    }
}