using System;
using System.Collections.Generic;
using Client.ConsoleWrappers.UserInputs;
using Shared.Models.Responses;

namespace Client.ConsoleWrappers
{
    public class ChatWrapper : ConsoleWrappersBase
    {
        private List<Message> messages = new List<Message>();


        protected override void RenderActionSection() => Console.WriteLine($"/1 onlinw users");

        protected override void RenderContent()
        {
            foreach(var message in messages)
            {
                WriteLine(message.Content, message.Color);
            }
        }

        public void AddMessage(TextResponse response) => AddMessage(new Message(response));
        public void AddSelfMessage(string content) => AddMessage(new Message(content));


        private void AddMessage(Message message)
        {
            messages.Add(message);
            Render();
        }
        private class Message
        {
            public string Content { get; }
            public ConsoleColor Color { get; }

            public Message(TextResponse response)
            {
                Color = ConsoleColor.Gray;
                Content = $"{response.Sender} : {response.Content}";
            }
            public Message(string selfMessage)
            {
                Color = ConsoleColor.Gray;
                Content = $"$you$ : {selfMessage}";
            }
        }

        public new UserInput ReadInput()
        {
            return base.ReadInput();
        }
    }
}
