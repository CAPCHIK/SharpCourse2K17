using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.ConsoleWrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Models.Messages;
using Shared.Models.Responses;
using System.Collections.Generic;

namespace Client
{
    //Класс для непосредственной работы с сейтью. Он занимается отправкой и приемом данных
    //Используя его извне можно не думать о том, как именно передаются данные
    public class NetworkWrapper
    {
        //объект TCP клиента для подключения к серверу по протоколу TCP
        private readonly TcpClient tcpClient = new TcpClient();
        //Объект синхронизации, для того чтобы при отправке сообщений не могло проихойти коллизии.
        //Испольлзуется ниже в этом файле
        private readonly object sync = new object();
        //tokenSource управляет токенами завершения работы потоков.
        //Данный класс будет инициировать два потока, и нам надо будет их как то остановить
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        //Событие ResponseRecieved
        //Событие - возможность языка C# вызывать какие-то методы или лямбды
        //В тот момент, когда это понадобится. То есть когда мы создали NetworkWrapper
        //Мы можем подписаться на событие "пришло сообщение"
        //Мы не знаем, когда приидет сообщение. Но когда придет - наш код выполнится.
        //Использование ResponseRecieved находится в ClientCore.cs
        public event Action<JToken, StatusCode> ResponseRecieved;

        //Содерэит логику поиска сервера. Сервер может находиться на любом компьютере в локальной сети
        //И надо как-то выяснить, где именно. Поэтому мы опросим все комрьютеры,
        //И если на каком-то из них есть сервер, он даст о себе знать
        //Так же мы будем ждать сервер, если не можем его найти
        public void FindServer()
        {
            //Данные, которые мы будем рассылать в широковещательном UDP сообщении
            //Будем отсылвать 1 байт, просто потому что. В этом нет какой-то логики
            //Можем не отсылать данных вообще.
            //Так же в эту же переменнаю будут записаны данные, которые мы получим от сервера.
            //А именно - TCP порт сервера
            var data = new byte[] { 1 };
            //Сервер, если он есть в локальной сети, слушает UDP сообщения на 5000 порту
            //Потому мы быдем отсылать сообшение всем, на порт 5000
            var endPoint = new IPEndPoint(IPAddress.Broadcast, 5000);
            //Номер попытки нахождения сервера. То есть мы первый раз отправляем сообщение о поиске сервера
            int tryNum = 1;
            //Создаем формочку, которая будет отображать пользователю информацию, что мы 1 раз
            //Пытаемся подключиться к серверу.
            //Это нужно для того, чтобы в ситуации когда сервера нет, и мы пробуем найти его снова и снова,
            //Пользователь видел, что программа не висит, и все нормально
            var placeHolder = new PlaceHolderWrapper($"Finding server try #{tryNum}");
            //Сохдаем объект для отправки и приема данных по протоколу UDP
            //ОС сама выберет порт, на котором будет работать этот клиент, потому не передаем
            //аргументов в конструктор
            var udpClient = new UdpClient();
            //Устанавливаем, что данный клиент будет ждать сообщений только 1000 милисекунд
            //Если данных нет в течении 1000 секунд - будет брошено исключение
            //Сделано для того чтобы мы могли отослать новое сообщение, если на старое никто не ответил
            //Ведь сервер может быть запущен позже, чем клиент
            udpClient.Client.ReceiveTimeout = 1000;
            //Условно бесконечный цикл. Сам цикл будет работать вечно, но внутрянняя логика 
            //Прервет его выполнение в необходимый момент
            while (true)
            {
                //Если код внутри блока try выбросит исключение, выполнение перейдет в блок catch 
                //И мы сможем предпринять какие-то действия, обработать эту ситуацию
                //В данном случае, мы можем получить исключение времени ожидания,
                //Понять, что переходим на следующую итерацию, уведомить про это пользователя,
                //И послать новое сообщение
                try
                {
                    //Отправляем сообщение длиной в 1 байт всем пк на порт 5000
                    udpClient.Send(data, data.Length, endPoint);
                    //Ждем, когда сервер вернет нам свой порт.
                    //Если сервер прислал порт, он будет записан в массив data в виде 4-х байт
                    //В переменную endPoint запишется адресс сервера, откуда нам пришло сообщение
                    //Это и есть адрес сервера. Ведь он отправит нам ответ
                    data = udpClient.Receive(ref endPoint);
                    //заканчиваем цикл while, мы получили TCP порт, так что пора к нему присоединяться
                    break;
                }
                //В течении секунды не пришло сообщения
                catch
                {
                    //Увеличиваем счетчик попыток на 1
                    tryNum++;
                    //У нашей формы заглушки устанавливаем новый текст для отображения
                    //Так, при каждой попытке мы увидим на консоли ее номер
                    placeHolder.Content = $"Finding server try #{tryNum}";
                }
            }
            //Отчищаем UDP клиента, он нам больше не понадобится
            //метод Dispose освободит занятый порт
            udpClient.Dispose();
            //Получаем число, порт сервера, из байт, которые он нам прислал
            //Чило, а именно порт сервера, хранится в переменной типв=а int
            //Допустим, порт сервера это 5100
            //По сети могут быть переданы только байты, потому данные биты (нули и единицы)
            //Разбиваются на 4 байта. Почему 4? int занимает 32 бита
            //А один байт занимает 8 бит. Соответственно любое число типа int может быть разбито на 4 байта
            //Нам же надо склеить эти байты обратно в int переменнную
            //BitConverter умеет отдавать байты для конкретного числа и создавать число из байт.
            //Смело пользуемся им
            int targetPort = BitConverter.ToInt32(data, 0);
            //Устанавливаем соединение с сервером. его IP адрес мы получили при получении UDP сообщения
            tcpClient.Connect(endPoint.Address, targetPort);
            //Запускаем метод ListenConnectioin, этот метод будет слушать данные от сервера, и вызывать
            //Событие о том, что пришел какой-то отсет от сервера. Все, кто захотят об этом узнать - узнают
            Task.Factory.StartNew(ListenConnection);
        }



        public void SendMessage(Message message)
        {

            var jsonString = JsonConvert.SerializeObject(message);
            var data = Encoding.UTF8.GetBytes(jsonString);
            var dataLength = BitConverter.GetBytes(data.Length);
            lock (sync)
            {
                tcpClient.GetStream().Write(dataLength, 0, dataLength.Length);
                tcpClient.GetStream().Write(data, 0, data.Length);
            }
        }

        public List<string> GetUsersList()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            List<string> names = new List<string>();

            Action<JToken, StatusCode> lamda = (obj, statusCode) =>
            {
                if (statusCode == StatusCode.OnlineUsers)
                {
                    names = obj.ToObject<OnlineUsersResponse>().UserNames;
                    resetEvent.Set();
                }
            };
            ResponseRecieved += lamda;
            SendMessage(new OnlineUsersMessage());
            resetEvent.Wait();
            ResponseRecieved -= lamda;
            return names;
        }

        public Response GetResponse()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim();
            Response response = null;

            Action<JToken, StatusCode> lamda = (obj, statusCode) =>
            {
                response = obj.ToObject<Response>();
                resetEvent.Set();

            };
            ResponseRecieved += lamda;
            resetEvent.Wait();
            ResponseRecieved -= lamda;
            return response;
        }


        private async Task ListenConnection()
        {
            var stream = tcpClient.GetStream();
            var responseLengthBuffer = new byte[sizeof(int)];
            var responseBuffer = new byte[4096];
            while(!tokenSource.Token.IsCancellationRequested)
            {
                await stream.ReadAsync(responseLengthBuffer, 0, sizeof(int), tokenSource.Token);
                var responseLength = BitConverter.ToInt32(responseLengthBuffer, 0);
                await stream.ReadAsync(responseBuffer, 0, responseLength, tokenSource.Token);
                var jsonString = Encoding.UTF8.GetString(responseBuffer, 0, responseLength);
                var token = JToken.Parse(jsonString);
                ResponseRecieved?.Invoke(token, GetStatusCode(token));
            }
        }
        private StatusCode GetStatusCode(JToken token) => token.ToObject<Response>().StatusCode;

    }
}
