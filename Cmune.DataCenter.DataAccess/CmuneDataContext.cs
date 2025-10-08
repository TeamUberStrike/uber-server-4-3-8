using System;
using System.Collections.Generic;
using System.Configuration;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
    partial class CmuneDataContext
    {
        /// <summary>
        /// Get the Esns handles matching with a cmid
        /// </summary>
        /// <param name="cmid"></param>
        /// <returns></returns>
        public IEnumerable<ESNSIdentity> GetEsnsHandles(int cmid)
        {
            return ExecuteQuery<ESNSIdentity>(@"select * from ESNSIdentities AS EId 
                                            join ESNSHandles AS Handle on Handle.ESNSID = EId.ESNSID
                                            where EId.CMID = {0}", cmid);
        }

        /// <summary>
        /// ConnectionString
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string cmuneDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSourceOverride, false);
                string dataSource = String.Empty;

                if (cmuneDBDataSourceConfig.IsNullOrFullyEmpty())
                {
                    cmuneDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseDataSource);

                    if (!cmuneDBDataSourceConfig.IsNullOrFullyEmpty())
                    {
                        cmuneDBDataSourceConfig = cmuneDBDataSourceConfig.ToLower();

                        if (cmuneDBDataSourceConfig.Equals(DatabaseDeployment.Dev))
                        {
                            dataSource = "Data Source=192.168.1.200;Initial Catalog=Cmune;Persist Security Info=True;User ID=sa;Password=cmune$1";
                        }
                        else if (cmuneDBDataSourceConfig.Equals(DatabaseDeployment.Staging))
                        {
                            dataSource = "Data Source=75.126.46.229;Initial Catalog=Cmune;Persist Security Info=True;User ID=CoreDbo;Password=thunderbus888";
                        }
                        else if (cmuneDBDataSourceConfig.Equals(DatabaseDeployment.Prod))
                        {
                            // We use the local IP for performance reason, if you want to query the production DB from outside EC2 us-east-1 availability zone you will have have to use the override
                            dataSource = "Data Source=10.116.59.43;Initial Catalog=Cmune;Persist Security Info=True;User ID=CoreDbo;Password=thunderbus888";
                        }
                        else
                        {
                            ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseDataSource + "\" has an incorrect value (\"" + cmuneDBDataSourceConfig + "\"): it should be " + DatabaseDeployment.Dev + " or " + DatabaseDeployment.Staging + " or " + DatabaseDeployment.Prod);
                            CmuneLog.LogException(configurationException, String.Empty);
                            throw configurationException;
                        }
                    }
                    else
                    {
                        ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseDataSource + "\" is empty and the key \"" + CommonAppSettings.DatabaseDataSourceOverride + "\" is missing.");
                        CmuneLog.LogException(configurationException, String.Empty);
                        throw configurationException;
                    }
                }
                else
                {
                    dataSource = cmuneDBDataSourceConfig;
                }

                return dataSource;
            }
        }

        /// <summary>
        /// CmuneDataContext default constructor
        /// </summary>
        public CmuneDataContext() :
            base(global::Cmune.DataCenter.DataAccess.CmuneDataContext.ConnectionString, mappingSource)
		{
			OnCreated();
		}
    }
}