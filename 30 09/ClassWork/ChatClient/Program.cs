using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatClient.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ChatClient
{
    class Program
    {
        static List<TextMessage> sendedMessages = new List<TextMessage>();
        static UdpClient client = new UdpClient(5000);
        static void Main(string[] args)
        {
            // TextMessage tMessage = new TextMessage
            // {
            //     Text = "Hello world",
            //     Id = Guid.NewGuid(),
            //     MessageType = MessageType.TextMessage
            // };
            // string json = JsonConvert.SerializeObject(tMessage
            //                             ,Formatting.Indented);
            // System.Console.WriteLine(json);

            // var readetData = Console.ReadLine();

            // AcceptMessage accMessage
            //  = JsonConvert.DeserializeObject<AcceptMessage>(readetData);
            
            // System.Console.WriteLine($"received id: {accMessage.ReceivedMessageId}");
            // System.Console.WriteLine($"Type: {accMessage.MessageType}");
            client.Client.ReceiveTimeout = 1000;
            string message;
            CancellationTokenSource source = new CancellationTokenSource();
            var thread = new Thread(() => Listen(source.Token));
            thread.Start();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            while ((message = Console.ReadLine()) != "exit")
            {
                TextMessage textMessage = new TextMessage
                {
                    Text = message,
                    Id = Guid.NewGuid()
                };
                sendedMessages.Add(textMessage);
                var jsonString = JsonConvert.SerializeObject(textMessage);
                var data = Encoding.UTF8.GetBytes(jsonString);
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
                    var obj = JsonConvert.DeserializeObject<JObject>(message);
                    var type = obj["MessageType"].ToObject<MessageType>();
                    switch (type)
                    {
                        case MessageType.TextMessage :
                            var textMessage = obj.ToObject<TextMessage>();
                            System.Console.WriteLine($"message {textMessage.Text}");
                            SendAccept(textMessage.Id, point);
                            break;
                        case MessageType.AcceptMessage : 
                            var accMessage = obj.ToObject<AcceptMessage>();
                            var targetMessage = sendedMessages.Find(M => M.Id == accMessage.ReceivedMessageId);
                            targetMessage.ReadedCount++;
                            System.Console.WriteLine($"mesasage {targetMessage.Text} was readed {targetMessage.ReadedCount}");
                            break;
                        default : 
                            System.Console.WriteLine("Invalid type");
                            break;
                    }
                    System.Console.WriteLine($"getted {message}");
                }
                catch   
                {
                    if(token.IsCancellationRequested)
                    {
                        System.Console.WriteLine("Second thread dead");
                        return;
                    }
                    //System.Console.WriteLine("I have exception");
                }
            }

        }

        static void SendAccept(Guid acceptedMessageId, IPEndPoint sender)
        {
            var accept = new AcceptMessage
            {
                ReceivedMessageId = acceptedMessageId
            };
            var jsonString = JsonConvert.SerializeObject(accept);
            var data = Encoding.UTF8.GetBytes(jsonString);
            client.Send(data, data.Length, sender);
        }
    }
}
