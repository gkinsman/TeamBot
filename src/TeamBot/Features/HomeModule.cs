using System;
using Nancy;

namespace TeamBot.Features
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => Response.AsRedirect("/app/");
            Get["/app/", true] = async (ctx, ct) => RenderView();
            Get["/app/{*}", true] = async (ctx, ct) => RenderView();
            Get["/Error", true] = async (ctx, ct) => { throw new Exception("Hello world!"); };
            Get["/NotFound", true] = async (ctx, ct) => new NotFoundResponse();
            Get["/Unauthorized", true] = async (ctx, ct) => new Response().WithStatusCode(HttpStatusCode.Unauthorized);
            Get["/Forbidden", true] = async (ctx, ct) => new Response().WithStatusCode(HttpStatusCode.Forbidden);
        }

        private object RenderView()
        {
            return View[@"app/index.html"]
                .WithHeader("Content-Type", "text/html; charset=utf-8")
                .WithHeader("Cache-Control", "no-cache, no-store, must-revalidate")
                .WithHeader("Pragma", "no-cache")
                .WithHeader("Expires", "0");
        }
    }
}
