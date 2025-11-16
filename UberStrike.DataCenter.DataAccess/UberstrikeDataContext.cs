using System;
using System.Configuration;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using UberStrike.DataCenter.Common.Entities;

namespace UberStrike.DataCenter.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    public partial class UberstrikeDataContext
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string paradiseDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.DatabaseDataSourceOverride, false);
                string dataSource = String.Empty;

                if (paradiseDBDataSourceConfig.IsNullOrFullyEmpty())
                {
                    paradiseDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(UberstrikeAppSettings.DatabaseDataSource);

                    if (!paradiseDBDataSourceConfig.IsNullOrFullyEmpty())
                    {
                        paradiseDBDataSourceConfig = paradiseDBDataSourceConfig.ToLower();

                        if (paradiseDBDataSourceConfig.Equals(DatabaseDeployment.Dev))
                        {
                            dataSource = "Data Source=DESKTOP-LNSADFU\\MYSECONDSERVER;Initial Catalog=MvParadisePaintball;Persist Security Info=True;User ID=sa;Password=cmune$1";
                        }
                        else if (paradiseDBDataSourceConfig.Equals(DatabaseDeployment.Staging))
                        {
                            dataSource = "Data Source=75.126.46.229;Initial Catalog=MvParadisePaintball;Persist Security Info=True;User ID=MvParadisePaintballDbo;Password=thunderbus888";
                        }
                        else if (paradiseDBDataSourceConfig.Equals(DatabaseDeployment.Prod))
                        {
                            // We use the local IP for performance reason, if you want to query the production DB from outside EC2 us-east-1c availability zone you will have have to use the override
                            dataSource = "Data Source=10.116.59.43;Initial Catalog=MvParadisePaintball;Persist Security Info=True;User ID=MvParadisePaintballDbo;Password=thunderbus888";
                        }
                        else
                        {
                            ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + UberstrikeAppSettings.DatabaseDataSource + "\" has an incorrect value (\"" + paradiseDBDataSourceConfig + "\"): it should be " + DatabaseDeployment.Dev + " or " + DatabaseDeployment.Staging + " or " + DatabaseDeployment.Prod);
                            CmuneLog.LogException(configurationException, String.Empty);
                            throw configurationException;
                        }
                    }
                    else
                    {
                        ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + UberstrikeAppSettings.DatabaseDataSource + "\" is empty and the key \"" + UberstrikeAppSettings.DatabaseDataSourceOverride + "\" is missing.");
                        CmuneLog.LogException(configurationException, String.Empty);
                        throw configurationException;
                    }
                }
                else
                {
                    dataSource = paradiseDBDataSourceConfig;
                }

                return dataSource;
            }
        }

        /// <summary>
        /// UberstrikeDataContext default constructor
        /// </summary>
        public UberstrikeDataContext() :
            base(global::UberStrike.DataCenter.DataAccess.UberstrikeDataContext.ConnectionString, mappingSource)
		{
			OnCreated();
		}
    }
}