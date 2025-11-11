using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Caching;
using Cmune.Core.Models.Views;
using Cmune.DataCenter.Business;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Business;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Types;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Business;
using UberStrike.DataCenter.Common.Entities;
using UberStrike.DataCenter.Utils;
using UberStrike.DataCenter.WebService.Interfaces;

namespace UberStrike.DataCenter.WebService
{
    public class ApplicationWebService : IApplicationWebService
    {
        /// <summary>
        /// <returns></returns> 
        /// NEW : Authentication for version 4.3.4
        /// </summary>
        /// <param name="version"></param>
        /// <param name="channel"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public AuthenticateApplicationView AuthenticateApplication(string version, ChannelType channel, string publicKey)
        {
            string cacheName = UberStrikeCacheKeys.CheckApplicationVersionViewParameters + CmuneCacheKeys.Separator + version + CmuneCacheKeys.Separator + ((int)channel);

            AuthenticateApplicationView authenticationApplicationView = null;

            if (HttpRuntime.Cache[cacheName] != null)
            {
                authenticationApplicationView = (AuthenticateApplicationView)HttpRuntime.Cache[cacheName];
            }
            else
            {
                ApplicationVersionViewModel currentApplication = ApplicationVersionService.GetApplicationVersion(version, channel);

                if (currentApplication != null)
                {
                    authenticationApplicationView = ApplicationVersionService.GetAuthenticationApplicationView(currentApplication);
                    HttpRuntime.Cache.Add(cacheName, authenticationApplicationView, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                }
            }

            if (authenticationApplicationView != null)
            {
                authenticationApplicationView.EncryptionInitVector = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.InitVector);
                authenticationApplicationView.EncryptionPassPhrase = ConfigurationUtilities.ReadConfigurationManager(CommonAppSettings.PassPhrase);
            }

            return authenticationApplicationView;
        }

        public RegisterClientApplicationViewModel RegisterClientApplication(int cmid, string hashCode, ChannelType channel, int applicationId)
        {
            Dictionary<int, int> itemsAttributedOrdered = new Dictionary<int, int>();
            var registerClientApplication = new RegisterClientApplicationViewModel()
            {
                Result = ClientApplication.RegisterClientApplication(cmid, hashCode, channel, applicationId, out itemsAttributedOrdered)
            };

            if (registerClientApplication.Result == ApplicationRegistrationResult.Ok)
            {
                registerClientApplication.ItemsAttributed = itemsAttributedOrdered.Keys;
            }
            return registerClientApplication;
        }

        public List<PhotonView> GetPhotonServers(ApplicationView applicationView)
        {
            return Games.GetAllPhotonServerViews(applicationView.Version, applicationView.Channel);
        }

        public List<LiveFeedView> GetLiveFeed()
        {
            return Games.GetLiveFeedView();
        }

        public List<MapView> GetMaps(string appVersion, LocaleType locale, MapType mapType)
        {
            List<MapView> maps = Games.GetMaps(appVersion, mapType);

            //TODO: here we need DB backed data
            foreach (var m in maps)
            {
                Maps.UpdateMapSettings(m);
            }

            return maps;
        }

        public void SetLevelVersion(int id, int version, string md5Hash)
        {
            //TODO: save levelID along with version and hash in DB
        }

        public bool ReportBug(BugView bugView)
        {
            UberStrikeMail.BugReport(bugView.Content, bugView.Subject, ConfigurationUtilities.ReadConfigurationManager("bugReportEmail"));

            return true;
        }

        public void RecordTutorialStep(int cmid, TutorialStepType step)
        {
            Tracking.RecordTutorialStep(cmid, step);
        }

        public void RecordException(int cmid, BuildType buildType, ChannelType channelType, string buildNumber, string logString, string stackTrace, string exceptionData)
        {
            if (IsExceptionTrackingActivated())
            {
                try
                {
                    string[] exceptionArray = logString.Split(':');
                    string exceptionType = exceptionArray[0];
                    string exceptionMessage = String.Empty;
                    for (int i = 1; i < exceptionArray.Length; i++)
                    {
                        exceptionMessage += exceptionArray[i];
                    }
                    UnityExceptionsService.RecordException(exceptionType, exceptionMessage, stackTrace, buildType, channelType, buildNumber, cmid, exceptionData);
                }
                catch (SqlException)
                {
                    StopExceptionTracking();
                    throw;
                }
            }
        }

        public void RecordExceptionUnencrypted(BuildType buildType, ChannelType channelType, string buildNumber, string errorType, string errorMessage)
        {
            if (IsExceptionTrackingActivated())
            {
                try
                {
                    UnityExceptionsService.RecordException(String.Empty, errorMessage, errorType, buildType, channelType, buildNumber, 0, String.Empty);
                }
                catch (SqlException)
                {
                    StopExceptionTracking();
                    throw;
                }
            }
        }

        public string GetPhotonServerName(string applicationVersion, string ipAddress, int port)
        {
            return Games.GetPhotonServerName(applicationVersion, ipAddress, port);
        }

        public string GetMyIP()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties messageProperties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpointProperty.Address;
        }

        public string IsAlive()
        {
            return "yes";
        }

        #region Private Helpers

        private void StopExceptionTracking()
        {
            if (HttpRuntime.Cache["WebServicesClass.Application.IsTrackingActivated"] != null && (bool)HttpRuntime.Cache["WebServicesClass.Application.IsTrackingActivated"])
            {
                HttpRuntime.Cache.Remove("WebServicesClass.Application.IsTrackingActivated");
                HttpRuntime.Cache.Add("WebServicesClass.Application.IsTrackingActivated", false, null, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);

                CmuneMail.SendEmail("noreply@cmune.com", "Cmune devteam", "logger@cmune.com", "Cmune Logger", "WCF stopped recording the Unity exceptions", "Please help me!", "Please help me!");
            }
        }

        private bool IsExceptionTrackingActivated()
        {
            bool isTrackingActivated = false;

            if (HttpRuntime.Cache["WebServicesClass.Application.IsTrackingActivated"] != null)
            {
                isTrackingActivated = (bool)HttpRuntime.Cache["WebServicesClass.Application.IsTrackingActivated"];
            }
            else
            {
                isTrackingActivated = true;
                HttpRuntime.Cache.Add("WebServicesClass.Application.IsTrackingActivated", isTrackingActivated, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }

            return isTrackingActivated;
        }

        #endregion

        public string ExceptionString()
        {
            throw new System.Exception("This Webservice throws an Exception");
        }

        public void ExceptionVoid()
        {
            throw new System.Exception("This Webservice throws an Exception");
        }

        public int Sleep(int milliseconds)
        {
            System.Threading.Thread.Sleep(milliseconds);
            return 0;
        }
    }
}
