using System;
using Newtonsoft.Json;

namespace Shared.Models.Responses
{
    public class TextResponse : Response
    {
        public string Sender { get; }
		public string Content { get; }

        [JsonConstructor]
        public TextResponse(string sender, string content) 
            : base(StatusCode.TextMessage)
        {
            Sender = sender;
            Content = content;
        }

    }
}
