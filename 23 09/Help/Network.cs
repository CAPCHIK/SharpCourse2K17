using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _23_09
{
    public class Network
    {
        void UDPListenerStart()
        {
            UdpClient client = new UdpClient(5000);
            var endPoint = default(IPEndPoint);
            byte[] data = client.Receive(ref endPoint);
        }

        void UDPSend()
        {
            UdpClient client = new UdpClient(5000);
            var endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            var data = Encoding.UTF8.GetBytes("Hello world");
            client.Send(data, data.Length, endPoint);
        }
    }
}