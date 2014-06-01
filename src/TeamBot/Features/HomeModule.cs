using Nancy;

namespace TeamBot.Features
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["", true] = async (ctx, ct) =>
            {
                return View["index"];
            };
        }
    }
}
