using System;
using Newtonsoft.Json;
using Shared.Models.Users;

namespace Shared.Models.Messages
{
    public class TextMessage : Message
    {
        //Поля сообщения доступны только для чтения
        //После создания экземпляра этого класса мы не сможем их изменить
        //Это полезно, поскольку это устраняет возможность случайной подмены 
        //Идентификатора или текста сообщения
        public Guid Id {get;}
        public string Content {get;}
        public override MessageType MessageType => MessageType.Text;
        

        //Конструктор для простого создания текстового сообщения
        //Он вызывает другой конструктор, куда передает переданный текст
        //И генерирует новый GUID. Так мы точно можем знать, что каждое новое сообщение
        //Будет иметь уникальный идентификатор, и не забудем его указать.
        public TextMessage(string content) : this(content, Guid.NewGuid()) 
        {}
        [JsonConstructor]
        public TextMessage(string content, Guid id)
        {
            Content = content;
            Id = id;
        }

        
        //Так как Id задается при создании объекта, мы можем
        //Использовать его для идентификации объекта. Если Id одинаковые, то 
        //и сами объекты тоже несут в себе одинаковую информацию
        public override bool Equals(object obj)
        {
            var otherText = obj as TextMessage;
            return otherText?.Id == this.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}