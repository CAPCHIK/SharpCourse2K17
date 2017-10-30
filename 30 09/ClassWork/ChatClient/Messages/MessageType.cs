namespace ChatClient.Messages
{
    //Тип сообщения, enum есть набор называний для определенных чисел
    //Более подробно - google
    public enum MessageType
    {
        //Определяет, что сообщение содержит сам текст
        TextMessage,
        //Определяет, что сообщение содержит информацию о подтверждении прочтения
        AcceptMessage
    }
}