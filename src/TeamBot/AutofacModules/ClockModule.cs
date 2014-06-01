using Autofac;
using TeamBot.Infrastructure.Clock;

namespace TeamBot.AutofacModules
{
    public class ClockModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SystemClock>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}