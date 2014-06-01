using Autofac;
using Raven.Client;
using TeamBot.Infrastructure.Raven;

namespace TeamBot.AutofacModules
{
    public class RavenModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => DocumentStoreBuilder.DocumentStore.Value)
                .As<IDocumentStore>()
                .SingleInstance();
        }
    }
}