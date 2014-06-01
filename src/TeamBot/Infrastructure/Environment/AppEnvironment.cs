using System;

namespace TeamBot.Infrastructure.Environment
{
    public static class AppEnvironment
    {
        public const string EnvironmentTypeAppSettingKey = "EnvironmentType";
        public const string EnvironmentNameAppSettingKey = "EnvironmentName";

        private static bool _isInitialized;
        private static bool _isSimulating;

        private static string _machineName;
        private static EnvironmentType? _environmentType;
        private static EnvironmentName? _environmentName;

        private static string _simulatedMachineName;
        private static EnvironmentType? _simulatedEnvironmentType;
        private static EnvironmentName? _simulatedEnvironmentName;

        public static string MachineName
        {
            get
            {
                if (_isInitialized == false) 
                    Initialize();
                
                return _isSimulating ? _simulatedMachineName : _machineName;
            }
        }

        public static EnvironmentType EnvironmentType
        {
            get
            {
                if (_isInitialized == false) 
                    Initialize();

                return _isSimulating ? _simulatedEnvironmentType.Value : _environmentType.Value;
            }
        }

        public static EnvironmentName EnvironmentName
        {
            get
            {
                if (_isInitialized == false) 
                    Initialize();

                return _isSimulating ? _simulatedEnvironmentName.Value : _environmentName.Value;
            }
        }

        public static bool IsLocal()
        {
            return EnvironmentType == EnvironmentType.Local;
        }

        public static bool IsProduction()
        {
            return EnvironmentType == EnvironmentType.Production;
        }

        public static bool IsTest()
        {
            return EnvironmentType == EnvironmentType.Test;
        }

        public static void Simulate(string machineName, EnvironmentType? type, EnvironmentName? name)
        {
            _simulatedMachineName = machineName;
            _simulatedEnvironmentType = type;
            _simulatedEnvironmentName = name;
            _isSimulating = true;
        }

        private static void Initialize()
        {
            // Skip initialization if we are simulating
            if (_isSimulating) return;

            _machineName = System.Environment.MachineName;

            var environmentTypeFromConfig = System.Configuration.ConfigurationManager.AppSettings[EnvironmentTypeAppSettingKey];
            EnvironmentType type;
            if (Enum.TryParse(environmentTypeFromConfig, true, out type) == false)
                throw new Exception("The environment type was not one of the expected values: " + type +
                                    ".  Acceptable values are: " +
                                    string.Join(",", Enum.GetNames(typeof (EnvironmentType))));

            _environmentType = type;

            var environmentNameFromConfig = System.Configuration.ConfigurationManager.AppSettings[EnvironmentNameAppSettingKey];
            EnvironmentName name;
            if (Enum.TryParse(environmentNameFromConfig, true, out name) == false)
                throw new Exception("The environment name was not one of the expected values: " + name +
                                    ".  Acceptable values are: " +
                                    string.Join(",", Enum.GetNames(typeof(EnvironmentName))));

            _environmentName = name;

            _isInitialized = true;
        }

        public static void Reset()
        {
            _isInitialized = false;
            _isSimulating = false;
            _machineName = null;
            _simulatedMachineName = null;
            _environmentType = null;
            _simulatedEnvironmentType = null;
            _environmentName = null;
            _simulatedEnvironmentName = null;
        }
    }
}