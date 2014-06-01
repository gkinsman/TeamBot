using Autofac;
using Microsoft.Owin;
using NSubstitute;

namespace TeamBot.Tests.AutofacModules
{
    public class SubstituteModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(context => Substitute.For<IOwinContext>())
                .As<IOwinContext>()
                .SingleInstance();
        }
    }
}