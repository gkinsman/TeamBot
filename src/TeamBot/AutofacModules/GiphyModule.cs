using Autofac;
using TeamBot.Features.Giphy;

namespace TeamBot.AutofacModules
{
    public class GiphyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GiphyClient>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
