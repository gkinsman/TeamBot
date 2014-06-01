using System;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using TeamBot.Infrastructure.Environment;

namespace TeamBot.Infrastructure.ErrorHandling.Nancy
{
    public class JsonErrorResponse : JsonResponse
    {
        private readonly Error _error;

        private JsonErrorResponse(Error error, ISerializer serializer)
            : base(error, serializer)
        {
            if (error == null) 
                throw new ArgumentNullException("error");
            
            if (serializer == null) 
                throw new ArgumentNullException("serializer");

            _error = error;
        }

        public string ErrorMessage { get { return _error.ErrorMessage; } }
        public string FullException { get { return _error.FullException; } }

        public static JsonErrorResponse FromMessage(string message, ISerializer serializer, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            if (message == null) 
                throw new ArgumentNullException("message");

            if (serializer == null) 
                throw new ArgumentNullException("serializer");

            return (JsonErrorResponse) new JsonErrorResponse(new Error { ErrorMessage = message }, serializer)
                .WithStatusCode(statusCode)
                .WithHeader("Reason", message);
        }

        public static JsonErrorResponse FromException(Exception ex, ISerializer serializer)
        {
            if (ex == null) 
                throw new ArgumentNullException("ex");

            if (serializer == null) 
                throw new ArgumentNullException("serializer");

            var error = new Error { ErrorMessage = ex.Message, FullException = AppEnvironment.IsProduction() ? null : ex.ToString() };

            return (JsonErrorResponse) new JsonErrorResponse(error, serializer)
                .WithStatusCode(HttpStatusCode.InternalServerError)
                .WithHeader("Reason", error.ErrorMessage);
        }

        private class Error
        {
            public string ErrorMessage { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string FullException { get; set; }
        }
    }
}