using Autofac;
using TeamBot.Features.TeamCity;

namespace TeamBot.AutofacModules
{
    public class TeamCityModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TeamCityClient>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}