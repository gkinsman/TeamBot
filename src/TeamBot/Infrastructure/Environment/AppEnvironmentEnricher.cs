using System;
using Serilog.Core;
using Serilog.Events;

namespace TeamBot.Infrastructure.Environment
{
    /// <summary>
    /// Enriches log events with the AppEnvironment details.
    /// </summary>
    public class AppEnvironmentEnricher : ILogEventEnricher
    {
        private const string MachineNamePropertyName = "MachineName";
        private const string EnvironmentTypePropertyName = "EnvironmentType";
        private const string EnvironmentNamePropertyName = "EnvironmentName";
        private const string ApplicationNamePropertyName = "ApplicationName";
        private const string ApplicationVersionPropertyName = "ApplicationVersion";

        readonly string _applicationName;
        readonly string _applicationVersion;
        private LogEventProperty[] _cachedProperties;

        public AppEnvironmentEnricher(string applicationName, string applicationVersion)
        {
            if (applicationName == null) 
                throw new ArgumentNullException("applicationName");

            if (applicationVersion == null) 
                throw new ArgumentNullException("applicationVersion");

            _applicationName = applicationName;
            _applicationVersion = applicationVersion;
        }

        /// <summary>
        /// Enrich the log event.
        /// 
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param><param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent == null) 
                throw new ArgumentNullException("logEvent");
            
            if (propertyFactory == null) 
                throw new ArgumentNullException("propertyFactory");

            if (_cachedProperties == null)
            {
                _cachedProperties = new []
                {
                    propertyFactory.CreateProperty(MachineNamePropertyName, AppEnvironment.MachineName, destructureObjects: false),
                    propertyFactory.CreateProperty(EnvironmentTypePropertyName, AppEnvironment.EnvironmentType, destructureObjects: false),
                    propertyFactory.CreateProperty(EnvironmentNamePropertyName, AppEnvironment.EnvironmentName, destructureObjects: false),
                    propertyFactory.CreateProperty(ApplicationNamePropertyName, _applicationName, destructureObjects: false),
                    propertyFactory.CreateProperty(ApplicationVersionPropertyName, _applicationVersion, destructureObjects: false),
                };
            }

            foreach (var property in _cachedProperties)
            {
                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}