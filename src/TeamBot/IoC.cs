using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Builder;

namespace TeamBot
{
    public class IoC
    {
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Io")]
        public static IContainer LetThereBeIoC(ContainerBuildOptions containerBuildOptions = ContainerBuildOptions.None, Action<ContainerBuilder> preBuildHook = null)
        {
            var builder = new ContainerBuilder();

            var thisAssembly = typeof(IoC).Assembly;

            builder.RegisterAssemblyModules(thisAssembly);

            if (preBuildHook != null) 
                preBuildHook(builder);

            return builder.Build(containerBuildOptions);
        }
    }
}