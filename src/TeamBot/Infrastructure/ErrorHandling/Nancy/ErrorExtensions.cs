using System.Linq;
using Nancy;

namespace TeamBot.Infrastructure.ErrorHandling.Nancy
{
    public static class ErrorExtensions
    {
        public static Response AsError(this IResponseFormatter formatter, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return JsonErrorResponse.FromMessage(message, formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json")), statusCode);
        }

        public static Response AsBadRequest(this IResponseFormatter formatter, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return JsonErrorResponse.FromMessage(message, formatter.Serializers.FirstOrDefault(s => s.CanSerialize("application/json")), statusCode);
        }
    }
}