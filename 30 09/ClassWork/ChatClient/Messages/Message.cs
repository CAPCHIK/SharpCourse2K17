namespace ChatClient.Messages
{
    //Класс обязывает все наследуемые классы иметь тип сообщения
    public abstract class Message
    {
        //Абстрактный класс не знает, какой у него тип
        //И эьто решение принимают ниже стоящие классы, которые собственно
        //и представляют данные для каждого типа
        public abstract MessageType MessageType {get;}
    }
}