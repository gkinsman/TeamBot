using System;
using Autofac;
using Autofac.Builder;
using NUnit.Framework;
using TeamBot.Tests.AutofacModules;
using TeamBot.Tests.Conventions.Container;

namespace TeamBot.Tests.Conventions
{
    public class VerifyAllTypesRegisteredWithTheContainer
    {
        [Test]
        public void VerifyAllTypesCanBeResolved()
        {
            var assertion = new AutofacContainerAssertion(Filter, IsKnownOffender);
            var container = IoC.LetThereBeIoC(ContainerBuildOptions.IgnoreStartableComponents, builder => builder.RegisterModule<SubstituteModule>());
            assertion.Verify(container);
            container.Dispose();
        }

        [Test]
        public void VerifyAllRegisteredTypesLifetimes()
        {
            var assertion = new AutofacLifetimeAssertion();
            var container = IoC.LetThereBeIoC(ContainerBuildOptions.IgnoreStartableComponents, builder => builder.RegisterModule<SubstituteModule>());
            assertion.Verify(container);
            container.Dispose();
        }

        private bool Filter(Type serviceType)
        {
            return true;
        }

        private bool IsKnownOffender(Type serviceType)
        {
            return false;
        }
    }
}