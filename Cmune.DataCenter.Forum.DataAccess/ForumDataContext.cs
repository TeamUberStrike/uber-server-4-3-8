using System;
using System.Configuration;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;

namespace Cmune.DataCenter.Forum.DataAccess
{
    /// <summary>
    /// ForumDataContext
    /// </summary>
    public partial class ForumDataContext
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string forumDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseForumDataSourceOverride, false);
                string dataSource = String.Empty;

                if (forumDBDataSourceConfig.IsNullOrFullyEmpty())
                {
                    forumDBDataSourceConfig = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.DatabaseForumDataSource);

                    if (!forumDBDataSourceConfig.IsNullOrFullyEmpty())
                    {
                        forumDBDataSourceConfig = forumDBDataSourceConfig.ToLower();

                        if (forumDBDataSourceConfig.Equals(DatabaseDeployment.Dev))
                        {
                            dataSource = "Data Source=192.168.1.200;Initial Catalog=MvParadisePaintballForum;Persist Security Info=True;User ID=sa;Password=cmune$1";
                        }
                        else if (forumDBDataSourceConfig.Equals(DatabaseDeployment.Staging))
                        {
                            dataSource = "Data Source=75.126.46.229;Initial Catalog=MvParadisePaintballForum;Persist Security Info=True;User ID=MvParadisePaintballDbo;Password=thunderbus888";
                        }
                        else if (forumDBDataSourceConfig.Equals(DatabaseDeployment.Prod))
                        {
                            // We use the local IP for performance reason, if you want to query the production DB from outside EC2 us-east-1 availability zone you will have have to use the override
                            dataSource = "Data Source=10.116.59.43;Initial Catalog=MvParadisePaintballForum;Persist Security Info=True;User ID=MvParadisePaintballDbo;Password=thunderbus888";
                        }
                        else
                        {
                            ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseForumDataSource + "\" has an incorrect value (\"" + forumDBDataSourceConfig + "\"): it should be " + DatabaseDeployment.Dev + " or " + DatabaseDeployment.Staging + " or " + DatabaseDeployment.Prod);
                            CmuneLog.LogException(configurationException, String.Empty);
                            throw configurationException;
                        }
                    }
                    else
                    {
                        ConfigurationErrorsException configurationException = new ConfigurationErrorsException("The config key \"" + CommonAppSettings.DatabaseForumDataSource + "\" is empty and the key \"" + CommonAppSettings.DatabaseForumDataSourceOverride + "\" is missing.");
                        CmuneLog.LogException(configurationException, String.Empty);
                        throw configurationException;
                    }
                }
                else
                {
                    dataSource = forumDBDataSourceConfig;
                }

                return dataSource;
            }
        }

        /// <summary>
        /// ForumDataContext default constructor
        /// </summary>
        public ForumDataContext() :
            base(global::Cmune.DataCenter.Forum.DataAccess.ForumDataContext.ConnectionString, mappingSource)
		{
			OnCreated();
		}
    }
}