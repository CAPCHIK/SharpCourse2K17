using Newtonsoft.Json;

namespace Shared.Models.Responses
{
    //Базовый класс ответа от сервера
    public class Response
    {
        //Свойство статускод показывает, какой тип у этого сообщения
        public StatusCode StatusCode {get;}
        //Конструктор, аргумент или руками или библиотекой JSON
        [JsonConstructor]
        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        //Статичное свойство для быстрого доступа к OK Response
        public static Response GoodResponse => new Response(StatusCode.OK);
    }
}