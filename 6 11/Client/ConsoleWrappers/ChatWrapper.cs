using System;
using System.Collections.Generic;
using Client.ConsoleWrappers.UserInputs;
using Shared.Models.Responses;

namespace Client.ConsoleWrappers
{
    //Форма для отрисовки чата
    public class ChatWrapper : ConsoleWrappersBase
    {
        //Список сообщений, которые надо отрисовывать
        private List<Message> messages = new List<Message>();

        //Имеем одно действие, вывести список пользователей онлайн
        protected override void RenderActionSection() => Console.WriteLine($"/1 onlinw users");
        //Контент есть отрисовка каждого сообщения друг за другом
        protected override void RenderContent()
        {
            foreach(var message in messages)
            {
                //Метод WriteLine определен в базовом классе, и пишет строку нужным цветом
                WriteLine(message.Content, message.Color);
            }
        }
        //Публичный метод для добавления в формe сообщение от третьего лица
        //Просто вызываем AddMessage с экземпляром класса Message, основанном на TextResponse
        public void AddMessage(TextResponse response) => AddMessage(new Message(response));
        //Публичный метод для добавления в формк сообщение от нас самих
        //Просто вызываем AddMessage с экземпляром класса Message, основанном на простом тексте
        public void AddSelfMessage(string content) => AddMessage(new Message(content));

        //добавление сообщения
        private void AddMessage(Message message)
        {
            //Добавляем сообщение в коллекцию сообщения 
            messages.Add(message);
            //Отрисовываем содержимое
            Render();
        }
        //Внутренний класс Message, доступен только внутри этой формы
        private class Message
        {
            //Текст сообщения
            public string Content { get; }
            //Цвет текст сообщения
            public ConsoleColor Color { get; }
            //При получении сообщения от пользователя цвет сервый
            //А строка имеет формат имя : текст
            public Message(TextResponse response)
            {
                Color = ConsoleColor.Gray;
                Content = $"{response.Sender} : {response.Content}";
            }
            //Свои сообщения отображаются почти так же, 
            //Но вместо своего ника пишем $you$
            public Message(string selfMessage)
            {
                Color = ConsoleColor.Gray;
                Content = $"$you$ : {selfMessage}";
            }
        }
        //Считывание данных просто возвращает обычную реализацию
        //Никакой своей логики сдесь не делаем
        public new UserInput ReadInput()
        {
            return base.ReadInput();
        }
    }
}
