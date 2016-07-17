using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Nancy;
using Nancy.Owin;
using Owin;
using Seq;
using Serilog;
using Serilog.Events;
using TeamBot.Infrastructure.Environment;
using TeamBot.Infrastructure.ErrorHandling.Owin;

namespace TeamBot
{
    public class Startup
    {
        private const string ApplicationName = "TeamBot.Web";
         
        public void Configuration(IAppBuilder app)
        {
            if (app == null) 
                throw new ArgumentNullException("app");

            ConfigureLogging();
            HookUpToUnhandledExceptionsInTheAppDomain();
            HookUpToUnobservedTaskExceptions();

            var container = IoC.LetThereBeIoC();

            app.UseSerilogErrorHandling();
            UseNancy(app, container);
        }

        private static void HookUpToUnhandledExceptionsInTheAppDomain()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                try
                {
                    Log.Error((Exception)args.ExceptionObject, "Unhandled Exception: {Message}",
                        ((Exception)args.ExceptionObject).Message);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception e)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            };
        }

        private static void HookUpToUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                try
                {
                    Log.Error(args.Exception, "Unhandled Exception: {Message}",
                        (args.Exception).Message);
                }
                // ReSharper disable once UnusedVariable
                catch (Exception e)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                }
            };
        }

        private static void ConfigureLogging()
        {
            var logLocation = string.Format(@"TeamBotLog-{0}-{{Date}}.txt",
                AppEnvironment.EnvironmentName);
             
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Trace()
                .WriteTo.RollingFile(logLocation)
                .WriteTo.Seq(System.Configuration.ConfigurationManager.AppSettings["SeqServerUri"])
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .Enrich.With(new AppEnvironmentEnricher(ApplicationName, typeof(Startup).Assembly.GetName().Version.ToString()))
                .Destructure.ByTransforming<Request>(r => new { r.Method, Url = r.Url.ToString(), Headers = r.Headers.ToDictionary(x => x.Key, x => x.Value), Cookies = r.Cookies.ToDictionary(x => x.Key, x => x.Value), r.Form, r.UserHostAddress })
                .MinimumLevel.Is((LogEventLevel)Enum.Parse(typeof(LogEventLevel), System.Configuration.ConfigurationManager.AppSettings["MinimumLogLevel"]));

            Log.Logger = loggerConfig.CreateLogger();
        }

        private void UseNancy(IAppBuilder app, IContainer container)
        {
            if (app == null) 
                throw new ArgumentNullException("app");

            if (container == null)
                throw new ArgumentNullException("container");

            app.UseNancy(options =>
            {
                var bootstrapper = new NancyBootstrapper()
                    .UseContainer(container)
                    //.EnforceHttps(!AppEnvironment.IsLocal())
                    .ResolveClaimsPrincipal(ResolveClaimsPrincipal);

                options.Bootstrapper = bootstrapper;
            });
        }

        private ClaimsPrincipal ResolveClaimsPrincipal(NancyContext ctx)
        {
            if (ctx == null) 
                throw new ArgumentNullException("ctx");

            object owinEnvironmentObject;
            if (ctx.Items.TryGetValue(NancyOwinHost.RequestEnvironmentKey, out owinEnvironmentObject))
            {
                var owinEnvironment = (IDictionary<string, object>)owinEnvironmentObject;
                object userObject;
                if (owinEnvironment != null && owinEnvironment.TryGetValue("server.User", out userObject))
                {
                    return (ClaimsPrincipal)userObject;
                }
            }

            return null;
        }
    }
}