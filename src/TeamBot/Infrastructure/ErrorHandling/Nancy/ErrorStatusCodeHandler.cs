using System;
using System.Linq;
using Nancy;
using Nancy.ErrorHandling;
using Nancy.Responses.Negotiation;
using Nancy.ViewEngines;
using Serilog;
using TeamBot.Infrastructure.Environment;

namespace TeamBot.Infrastructure.ErrorHandling.Nancy
{
    public sealed class ErrorStatusCodeHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        private readonly ISerializer _serializer;

        public ErrorStatusCodeHandler(IViewFactory viewFactory, ISerializer serializer) : base(viewFactory)
        {
            if (viewFactory == null) 
                throw new ArgumentNullException("viewFactory");
            
            if (serializer == null) 
                throw new ArgumentNullException("serializer");

            _serializer = serializer;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.InternalServerError;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var clientWantsHtml = ShouldRenderFriendlyErrorPage(context);
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    if (clientWantsHtml)
                    {
                        context.Response = RenderView(context, "Shared/NotFound")
                            .WithStatusCode(HttpStatusCode.NotFound)
                            .WithHeader("Reason", "The resource you were looking for was not found.");
                    }
                    else
                    {
                        context.Response =
                            JsonErrorResponse.FromMessage("The resource you requested was not found.", _serializer,
                                HttpStatusCode.NotFound);
                    }
                    break;

                case HttpStatusCode.InternalServerError:
                    var exception = GetExceptionFromContext(context);

                    Log.Error(exception, "Error {Method} to {Url} in {ModuleName} to {ModulePath} as {User}",
                        context.Request.Method, context.Request.Url,
                        context.NegotiationContext == null ? "Unknown ModuleName" : context.NegotiationContext.ModuleName,
                        context.NegotiationContext == null ? "Unknown ModulePath" : context.NegotiationContext.ModulePath,
                        context.CurrentUser == null ? "Anonymous User" : context.CurrentUser.UserName);

                    if (clientWantsHtml)
                    {
                        context.Response = RenderView(context, "Shared/Error", new { ErrorMessage = exception.Message, FullException = AppEnvironment.IsProduction() ? string.Empty : exception.ToString() })
                            .WithStatusCode(HttpStatusCode.InternalServerError)
                            .WithHeader("Reason", exception.Message);
                    }
                    else
                    {
                        context.Response = JsonErrorResponse.FromException(exception, _serializer);
                    }
                    break;
            }
        }

        private static Exception GetExceptionFromContext(NancyContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            Object exceptionObject;
            context.Items.TryGetValue(NancyEngine.ERROR_EXCEPTION, out exceptionObject);
            var exception = (Exception) exceptionObject;
            return exception is RequestExecutionException ? exception.InnerException : exception;
        }

        private static bool ShouldRenderFriendlyErrorPage(NancyContext context)
        {
            if (context == null) 
                throw new ArgumentNullException("context");

            var enumerable = context.Request.Headers.Accept;

            var ranges = enumerable.OrderByDescending(o => o.Item2).Select(o => MediaRange.FromString(o.Item1)).ToList();
            foreach (var item in ranges)
            {
                if (item.Matches("application/json"))
                    return false;
                if (item.Matches("text/json"))
                    return false;
                if (item.Matches("text/html"))
                    return true;
            }

            return true;
        }
    }
}