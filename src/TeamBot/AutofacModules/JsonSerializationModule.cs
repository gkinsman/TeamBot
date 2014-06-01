using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TeamBot.AutofacModules
{
    public class JsonSerializationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters =
                {
                    new StringEnumConverter()
                }
            }).As<JsonSerializer>();
        }
    }
}