using Autofac;
using TeamBot.Infrastructure.Slack;

namespace TeamBot.AutofacModules
{
    public class SlackModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SlackClient>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
