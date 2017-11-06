using Newtonsoft.Json;

namespace Shared.Models.Responses
{
    public class Response
    {
        public StatusCode StatusCode {get;}

        [JsonConstructor]
        public Response(StatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        public static Response GoodResponse => new Response(StatusCode.OK);
    }
}