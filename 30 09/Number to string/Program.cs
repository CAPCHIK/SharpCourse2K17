using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Number_to_string
{
    class  Message
    {
        public Guid ID {get; set;} = Guid.NewGuid();
        public string Text {get; set;}
        public int ReadsCount {get; set;}
    }
    class Program
    {
        static List<Message> messages = new List<Message>();
        static void Main(string[] args)
        {
            WriteMessage(new Message{Text = "Hi hi hi"});
            WriteMessage(new Message{Text = "ho hey"});
            var message = new Message{Text = "ho hey"};
            WriteMessage(message);
            WriteMessage(new Message{Text = "ahahaha lolka"});
            WriteMessage(new Message{Text = "latest message"});
            message.ReadsCount++;
            RenderChat();
        }
        static int iteration = 0;
        static object sync = new object();
        static void RenderChat()
        {
            //System.Console.WriteLine($"interation {iteration++}");
            lock(sync)
            {
            Console.Clear();
            foreach(var message in messages)
            {
                System.Console.WriteLine($"{message.ReadsCount} : {message.Text}");
            }
            }
        }
        static void WriteMessage(Message message)
        {
            messages.Add(message);
            RenderChat();
        }
    }
}
