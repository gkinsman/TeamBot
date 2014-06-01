using Autofac;
using TeamBot.Infrastructure.Messages;

namespace TeamBot.AutofacModules
{
    public class MessageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageProcessor>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}