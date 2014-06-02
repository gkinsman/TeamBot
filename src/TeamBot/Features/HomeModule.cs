using System.Configuration;
using Nancy;

namespace TeamBot.Features
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["", true] = async (ctx, ct) =>
            {
                var model = new
                {
                    EnvironmentName = ConfigurationManager.AppSettings["EnvironmentName"],
                    Version = typeof(IoC).Assembly.GetName().Version.ToString(3)
                };

                return View["index", model];
            };
        }
    }
}
