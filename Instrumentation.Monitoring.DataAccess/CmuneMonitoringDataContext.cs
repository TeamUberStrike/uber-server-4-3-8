using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Cmune.DataCenter.Utils;
using Cmune.DataCenter.Common.Entities;

namespace Cmune.Instrumentation.Monitoring.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CmuneMonitoringDataContext
    {
        /// <summary>
        /// DatabaseDataSourceOverride config name
        /// </summary>
        public const string DatabaseDataSourceOverride = "DbAddressMonitoring";

        /// <summary>
        /// ConnectionString Getter
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string monitoringDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(DatabaseDataSourceOverride, false);
                string dataSource = String.Empty;

                if (monitoringDBDataSourceConfig.IsNullOrFullyEmpty())
                {
                    monitoringDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSource);

                    if (!monitoringDBDataSourceConfig.IsNullOrFullyEmpty())
                    {
                        monitoringDBDataSourceConfig = monitoringDBDataSourceConfig.ToLower();

                        if (monitoringDBDataSourceConfig.Equals(DatabaseDeployment.Dev))
                        {
                            dataSource = "Data Source=DESKTOP-LNSADFU\\MYSECONDSERVER;Initial Catalog=CmuneMonitoring;Persist Security Info=True;User ID=sa;Password=cmune$1";
                        }
                        else if (monitoringDBDataSourceConfig.Equals(DatabaseDeployment.Staging))
                        {
                            dataSource = "Data Source=75.126.46.229;Initial Catalog=CmuneMonitoring;Persist Security Info=True;User ID=CmuneMonitoringDbo;Password=thunderbus888";
                        }
                        else if (monitoringDBDataSourceConfig.Equals(DatabaseDeployment.Prod))
                        {
                            // We use the local IP for performance reason, if you want to query the production DB from outside EC2 us-east-1 availability zone you will have have to use the override
                            dataSource = "Data Source=10.116.59.43;Initial Catalog=CmuneMonitoring;Persist Security Info=True;User ID=CmuneMonitoringDbo;Password=thunderbus888";
                        }
                        else
                        {
                            ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseDataSource + "\" has an incorrect value (\"" + monitoringDBDataSourceConfig + "\"): it should be " + DatabaseDeployment.Dev + " or " + DatabaseDeployment.Staging + " or " + DatabaseDeployment.Prod);
                            CmuneLog.LogException(configurationException, String.Empty);
                            throw configurationException;
                        }
                    }
                    else
                    {
                        ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseDataSource + "\" is empty and the key \"" + DatabaseDataSourceOverride + "\" is missing.");
                        CmuneLog.LogException(configurationException, String.Empty);
                        throw configurationException;
                    }
                }
                else
                {
                    dataSource = monitoringDBDataSourceConfig;
                }

                return dataSource;
            }
        }

        /// <summary>
        /// Override of the default constructor
        /// </summary>
        public CmuneMonitoringDataContext() :
            base(global::Cmune.Instrumentation.Monitoring.DataAccess.CmuneMonitoringDataContext.ConnectionString, mappingSource)
		{
			OnCreated();
		}
    }
}
