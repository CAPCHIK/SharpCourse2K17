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
        //Список сообщений, которые мы отправили
        static List<TextMessage> sendedMessages = new List<TextMessage>();
        //Клиент для удобного приема и отправки сообщений
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

            //Устанавливаем максимальное время ожидания сообщения в 1000 милисекунд
            client.Client.ReceiveTimeout = 1000;
            //Переменная message будет хранить в себе сообщение, считанное с консоли
            string message;
            //source является точкой генерации токенов для отмены работы потоков
            CancellationTokenSource source = new CancellationTokenSource();
            //Создаем отдельный поток выполнения для прослушивания сети
            //Передаем лямбду, в которой вызываем метод Listen с токеном для отмены прослушки
            var thread = new Thread(() => Listen(source.Token));
            //Запускаем поток
            thread.Start();
            //мы создем адресс рассылки, Broadcast говорит о том, что сообщение будет разослано
            //всем в локальной сети, а 5000 определяет порт, на котором должны быть получатели
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            //Считываем строку с консоли, сохраняем это значение в переменную message
            //И если это сообщение не равно строке exit то выполняем тело цикла
            while ((message = Console.ReadLine()) != "exit")
            {
                //Формируем TextMessage, описывающий набранное нами сообщение
                TextMessage textMessage = new TextMessage
                {
                    //Поле Text есть считанная строка
                    Text = message,
                    //Идентификатор это новосгенерированный Guid
                    Id = Guid.NewGuid()
                };
                //Добавляем сообщение в список отправленных нами сообщений
                sendedMessages.Add(textMessage);
                //Получаем JSON представление данных, хранимых в нашем текстовом сообщении
                var jsonString = JsonConvert.SerializeObject(textMessage);
                //Преобразуем JSON строку в массив байт, для передачи по сети
                var data = Encoding.UTF8.GetBytes(jsonString);
                //Отправляем все данные на ранее объявленую конечяную точку
                client.Send(data, data.Length, endPoint);
            }
            //Была введена строка exit и надо завершить все потоки
            source.Cancel();
            //Выводим сообщение о конце работы основоного потока
            System.Console.WriteLine("THE BIG END");
        }

        //Метод Listen слушает сеть, выполняется в отдельном потоке
        static void Listen(CancellationToken token)
        {
            //Бесконечный цикл, так как слушать сообщения нам необходимо бесконечное количество раз
            //А условие выхода из цикла обрабатывается внутри
            while (true)
            {
                //Код внутри try может бросить исключение, и программа не упадет, 
                //А перейдет в блок catch. После этошго просто продолжит свою работу
                try
                {
                    //point будет хранить адрес отправителя сообщения
                    IPEndPoint point = default(IPEndPoint);
                    //полуачем данные, и  в это же времия сохранили адрес отправителя
                    //Про ref - google
                    var data = client.Receive(ref point);
                    //Получаем из байтов JSON строку
                    var message = Encoding.UTF8.GetString(data);
                    //Получаем JSON словарь, который хранит весь переданный JSON объект
                    var obj = JsonConvert.DeserializeObject<JObject>(message);
                    //получаем все данные из поля MessageType, и приводим их к типу MessageType
                    var type = obj["MessageType"].ToObject<MessageType>();
                    //В зависимости от типа сообщения делаем разные вещи
                    switch (type)
                    {
                        //Кто-то прислал нам сообщение
                        case MessageType.TextMessage :
                            //Конвертируем  полученный ранее объект к типу TextMessage
                            var textMessage = obj.ToObject<TextMessage>();
                            //Выводим сообщение на консоль
                            System.Console.WriteLine($"message {textMessage.Text}");
                            //Отправляем обратно сообщение от том, что мы получили данное сообщение
                            SendAccept(textMessage.Id, point);
                            //выходим из switch
                            break;
                        //Кто-то подтвердил получение отправленного нами сообщения
                        case MessageType.AcceptMessage : 
                            //Конвертируем  полученный ранее объект к типу  AcceptMessage
                            var accMessage = obj.ToObject<AcceptMessage>();
                            //Находим целевое сообщение, используя метод Find.
                            //Здесь не обрабатывается ситуация, если такового сообщения в списке нет, так что аккуратнее
                            var targetMessage = sendedMessages.Find(M => M.Id == accMessage.ReceivedMessageId);
                            //У полученного сообщения увеличиваем на 1 счетчик прочтений
                            targetMessage.ReadedCount++;
                            //Говорим о том, что наше сообщение прочитали сколько-то раз
                            System.Console.WriteLine($"mesasage {targetMessage.Text} was readed {targetMessage.ReadedCount}");
                            break;
                        //Нам прислыли некоторый невалидный тип сообщения, умываем руки
                        default : 
                            System.Console.WriteLine("Invalid type");
                            break;
                    }
                    //System.Console.WriteLine($"getted {message}");
                }
                //Внуцтри блока try выпало исключение, и теперь будет выполнен код внутри catch
                catch   
                {
                    //если наш токен для отмены работы потока просит об отмене работы
                    if(token.IsCancellationRequested)
                    {
                        //Выводим сообщение о том, что мы завершаем поток прослушивания
                        System.Console.WriteLine("Second thread dead");
                        //Выходим из метода Listen, чем и завершаем выполнение второго потока
                        return;
                    }
                    //System.Console.WriteLine("I have exception");
                }
            }

        }
        //Метод SendAccept отправляет сообщение о том,
        //что мы прочли некоторое сообщение, отправленое одним из собеседников
        static void SendAccept(Guid acceptedMessageId, IPEndPoint sender)
        {
            //Формируем сообщение, хранящее кроме типа только идентификатор прочитанного сообщения
            var accept = new AcceptMessage
            {
                ReceivedMessageId = acceptedMessageId
            };
            //Конвертируем данные в json строку
            var jsonString = JsonConvert.SerializeObject(accept);
            //Конвертируем строку в массив байт
            var data = Encoding.UTF8.GetBytes(jsonString);
            //Отправляем эти байты тому, от кого пришло данное 
            client.Send(data, data.Length, sender);
        }
    }
}
