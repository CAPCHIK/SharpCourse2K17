using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using Shared.Models.Responses;

namespace Server
{
    //Класс обертка над сетью, для сокрытия логики взаимодействия
    //с непосредственно сетью
    //Более подробно кухня именно передачи данных описана в аналогичном файле на стороне
    //Клиента, то есть в папке Client
    public class NetworkWrapper
    {
        //Задача, в которой будут слушаться UDP сообщения
        private Task udpTask;
        //Задача, в которой будут присоединяться TCP Клиенты
        private Task tcpTask;
        //Создаем UDP клиента для прослушивания сообщений
        //Сервер ли я?
        private UdpClient udpClient = new UdpClient(5000);
        //Полу для TCP листенера, будет инициализированно позже
        private TcpListener tcpListener;
        //Порт, на котором будет работать TCPListener, так же будет определен
        //Позже
        private int tcpPort;
        //Токен для отмены работ слушателей. Передастся из вне
        private readonly CancellationToken cancellationToken;
        //Событие вызывается когда к серверу по TCP 
        //Подключился некоторый компьютер
        public event Action<RemoteClient> ClientAccepted;
        //Конструктор. Принимает в себя токен завершения работы
        public NetworkWrapper(CancellationToken cancellationToken)
        {
            //Перебираем порты начиная с 5100 до бесконечности
            for (int port = 5100; ; port++)
                //Пробуем
                try
                {
                    //создать TcpListener на данном порту
                    tcpListener = new TcpListener(IPAddress.Any, port);
                    //Если получилось, начинаем слушать
                    tcpListener.Start();
                    //Сохраняем порт, на котором смогли заработать
                    tcpPort = port;
                    //Выводим сообщение о том, что запустились на таком-то порту
                    Console.WriteLine($"TCP Listener was start on {port} port");
                    //Прерываем цикл перебора портов
                    break;
                }
                //Если не удалось создать слушателя на выбранном порту
                //Ничего не делаем, просто запускаем новую итерацию
                catch { }
            //Сохраняем в приватное поле токен отмены работы
            this.cancellationToken = cancellationToken;

        }
        //Начало работы 
        public void Start()
        {
            //Запускаем слушателя UDP датаграм
            udpTask = Task.Factory.StartNew(ListenUDP);
            //Запускаем слушателя TCP клиентов
            tcpTask = Task.Factory.StartNew(ListenClients);
        }
        //Метод, который слушает UDP сообщения
        private async Task ListenUDP()
        {
            //Сохраняем порт, на котором работает TCP клиент в массив байт
            var data = BitConverter.GetBytes(tcpPort);
            //Пока не запрошена отмена работы
            while (!cancellationToken.IsCancellationRequested)
            {
                //Получаем сообщение
                //Испольщуется не метод Receive, как раньше
                //А ReceiveAsync() суть почти одинаковая. В данном курсе мы не
                //Разбираем асинхронность досконально
                var result = await udpClient.ReceiveAsync();
                //Отправляем тому, кто прислал нам сообщение порт
                //На котором сидит TCP Listener
                await udpClient.SendAsync(data, data.Length, result.RemoteEndPoint);
            }
        }
        //Метод, который слушает TCP клиентов
        private async Task ListenClients()
        {
            //Поука не запрошена отмена работы
            while (!cancellationToken.IsCancellationRequested)
            {
                //Пишем, что ждем клиентов
                Console.WriteLine("Waiting client...");
                //Получаем подсоединившегося клиента
                var client = await tcpListener.AcceptTcpClientAsync();
                //Пишем о том, что мы подсоединили клиента
                Console.WriteLine($"Accepted client");
                //Запускаем новую задачу, которая будет слушать данные именно от этого клиента
                var t = Task.Factory.StartNew(async () => await HandleClient(client));
            }
        }
        //Метод обработки конкретного клиента
        private async Task HandleClient(TcpClient client)
        {
            //Оборачиваем TcpClient в наш собственный класс RemoteClient
            //Сделано для сопоставления TCP соединения и нашей моделей пользователя
            //Чтобы понимать, по какому соединению необходимо отправлять данные
            RemoteClient remoteClient = new RemoteClient(
                //Аргументом является функция отправки сообщения
                //Этому клиенту. Мы и пишем, что при отправке сообщения
                //Этому клиенту надо отправить данное сообщение именно на этот client
                R => SendResponseToClient(R, client));
            //Вызываем событие о том, что клиент присоединился. 
            //Так внешний код сможет узнать о том, что подсоединился новый клиент
            //И начать следить ща его действиями 
            ClientAccepted?.Invoke(remoteClient);
            //Пока не отменили работу
            while (!cancellationToken.IsCancellationRequested)
            {
                //Буффер для хранения длины сообщения
                var messageLength = new byte[4];
                //Буффер для хранения самого сообщения
                var buffer = new byte[4096];
                //Читаем 4 байта, представляющие длину сообщения
                await client.GetStream().ReadAsync(messageLength, 0, 4);
                //Преобразуем эти 4 байта в число
                var length = BitConverter.ToInt32(messageLength, 0);
                //Читаем все сообщение в спец буффер для сообщения
                //Читаем именно нужное нам колчество денег
                await client.GetStream().ReadAsync(buffer, 0, length);
                //Получаем из считанныз байт саму строку JSON
                //которую нам прислал клиент
                var jsonString = Encoding.UTF8.GetString(buffer, 0, length);
                //Разбираем строку и кладем все данные в JToken
                var token = JToken.Parse(jsonString);
                //Выводим сообщение  о том, что мы получили некое сообщение
                Console.WriteLine("Received message");
                //Уведомляем клиента о том, что пришло новое сообщение
                remoteClient.Notify(token);
            }
        }
        //Метод для отправки данных некоторому клиенту
        //Данный метод используется в аргументе конструктора 
        //RemoteClient для определения, как отправлять данные
        //Именно этому удаленному клиенту
        private void SendResponseToClient(Response response, TcpClient client)
        {
            //Приводим объект response в строку в формате JSON и
            //Получаем из нее массив байт
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));
            //Полуячаем длину сообщения, сколько именно байт будет передано
            var dataLengthBytes = BitConverter.GetBytes(data.Length);
            //Блокируем данного клиента
            //Нам не надо блокировать некий общий ресурс, ибо запись ведется именно
            //В этого TcpClient так что надо обезапасить именно запись в него
            //При записи в другие TcpClient будут блокироваться они
            //И в итоге доступ к каждому из них будет синхронизирован
            lock(client)
            {
                //Записываем в поток для клиента длину сообщения, чтобы клиент мог понять
                //Размер самого сообщения
                client.GetStream().Write(dataLengthBytes, 0, dataLengthBytes.Length);
                //Записываем само сообщение
                client.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}
