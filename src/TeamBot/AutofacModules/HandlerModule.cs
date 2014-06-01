using Autofac;
using TeamBot.Infrastructure.Messages;

namespace TeamBot.AutofacModules
{
    public class HandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof (IoC).Assembly)
                .Where(t => t.IsAssignableTo<IHandleMessage>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}