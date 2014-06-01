using System;
using System.Security.Claims;
using Autofac;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.ErrorHandling;
using Nancy.Responses;
using Nancy.Serialization.JsonNet;
using Serilog;
using TeamBot.Infrastructure.Security;

namespace TeamBot
{
    public class NancyBootstrapper : AutofacNancyBootstrapper
    {
        private IContainer _container;
        private bool _enforceHttps;
        private Func<NancyContext, ClaimsPrincipal> _claimsPrincipalResolver;

        public NancyBootstrapper UseContainer(IContainer containerToUse)
        {
            if (containerToUse == null) 
                throw new ArgumentNullException("containerToUse");

            if (ApplicationContainer != null)
                throw new Exception("The ApplicationContainer already exists! This method should be called before the ApplicationContainer is initialized by Nancy.");

            _container = containerToUse;
            return this;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container ?? base.GetApplicationContainer();
        }

        public NancyBootstrapper EnforceHttps(bool enforce = true)
        {
            _enforceHttps = enforce;
            return this;
        }

        public NancyBootstrapper ResolveClaimsPrincipal(Func<NancyContext, ClaimsPrincipal> resolver)
        {
            if (resolver == null) 
                throw new ArgumentNullException("resolver");

            _claimsPrincipalResolver = resolver;
            return this;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            if (container == null) 
                throw new ArgumentNullException("container");
            
            if (pipelines == null) 
                throw new ArgumentNullException("pipelines");

            pipelines.BeforeRequest +=
                async (ctx, ct) =>
                {
                    Log.Debug("Begin {Method} to {Url}", ctx.Request.Method, ctx.Request.Url);

                    // Require SSL
                    if (_enforceHttps && ctx.Request.Url.IsSecure == false)
                    {
                        var secureUrl = ctx.Request.Url.Clone();
                        secureUrl.Scheme = "https";
                        return new RedirectResponse(secureUrl.ToString(), RedirectResponse.RedirectType.Permanent);
                    }

                    // Resolve and assign the CurrentUser
                    if (_claimsPrincipalResolver != null)
                    {
                        var principal = _claimsPrincipalResolver(ctx);
                        if (principal != null && principal.Identity.IsAuthenticated)
                        {
                            var claimsIdentity = (ClaimsIdentity)principal.Identity;
                            ctx.CurrentUser = container.Resolve<IUserIdentityEnricher>().GetEnrichedUser(claimsIdentity);
                        }
                    }

                    // Otherwise continue with processing this request
                    return null;
                };

            pipelines.AfterRequest +=
                async (ctx, ct) =>
                    Log.Debug("End {Method} to {Url} with response {Response}",
                        ctx.Request.Method, ctx.Request.Url, ctx.Response.StatusCode);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(config =>
                {
                    config.StatusCodeHandlers.Remove(typeof(DefaultStatusCodeHandler));
                    config.Serializers.Insert(0, typeof(JsonNetSerializer));
                });
            }
        }
    }
}