using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;
using System.Configuration;

namespace Cmune.Instrumentation.DataAccess
{
    public struct InstrumentationAppSettings
    {
        public const string DatabaseDataSourceOverride = "DbAddressInstrumentation";
        public const string DatabaseDataSource = "InstrumentationAPIKey";
    }

    public partial class InstrumentationDataContext
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string instrumenationDbDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(InstrumentationAppSettings.DatabaseDataSourceOverride, false);
                string dataSource = String.Empty;

                if (instrumenationDbDataSourceConfig.IsNullOrFullyEmpty())
                {
                    instrumenationDbDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(InstrumentationAppSettings.DatabaseDataSource);

                    if (!instrumenationDbDataSourceConfig.IsNullOrFullyEmpty())
                    {
                        instrumenationDbDataSourceConfig = instrumenationDbDataSourceConfig.ToLower();

                        if (instrumenationDbDataSourceConfig.Equals(DatabaseDeployment.Dev))
                        {
                            dataSource = "Data Source=DESKTOP-LNSADFU\\MYSECONDSERVER;Initial Catalog=Instrumentation;Persist Security Info=True;User ID=sa;Password=cmune$1";
                        }
                        else if (instrumenationDbDataSourceConfig.Equals(DatabaseDeployment.Staging))
                        {
                            dataSource = "Data Source=75.126.46.229;Initial Catalog=Instrumentation;Persist Security Info=True;User ID=InstrumentationDbo;Password=thunderbus888";
                        }
                        else if (instrumenationDbDataSourceConfig.Equals(DatabaseDeployment.Prod))
                        {
                            // We use the local IP for performance reason, if you want to query the production DB from outside EC2 us-east-1c availability zone you will have have to use the override
                            dataSource = "Data Source=10.116.59.43;Initial Catalog=Instrumentation;Persist Security Info=True;User ID=InstrumentationDbo;Password=thunderbus888";
                        }
                        else
                        {
                            ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + InstrumentationAppSettings.DatabaseDataSource + "\" has an incorrect value (\"" + instrumenationDbDataSourceConfig + "\"): it should be " + DatabaseDeployment.Dev + " or " + DatabaseDeployment.Staging + " or " + DatabaseDeployment.Prod);
                            CmuneLog.LogException(configurationException, String.Empty);
                            throw configurationException;
                        }
                    }
                    else
                    {
                        ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + InstrumentationAppSettings.DatabaseDataSource + "\" is empty and the key \"" + InstrumentationAppSettings.DatabaseDataSourceOverride + "\" is missing.");
                        CmuneLog.LogException(configurationException, String.Empty);
                        throw configurationException;
                    }
                }
                else
                {
                    dataSource = instrumenationDbDataSourceConfig;
                }

                return dataSource;
            }
        }

        /// <summary>
        /// UberstrikeDataContext default constructor
        /// </summary>
        public InstrumentationDataContext() :
            base(global::Cmune.Instrumentation.DataAccess.InstrumentationDataContext.ConnectionString, mappingSource)
		{
			OnCreated();
		}
    }
}
