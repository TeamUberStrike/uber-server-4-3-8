using System;
using System.Collections.Generic;
using Cmune.Core.Models.Views;

namespace Cmune.DataCenter.Common.Entities
{
    /// <summary>
    /// Defines the client version and the IP and ports of all our server applications
    /// Defines also the release date, expiration date
    /// </summary>
    [Serializable]
    public class ApplicationView
    {
        #region Properties

        public int ApplicationVersionId { get; set; }

        public string Version { get; set; }

        public BuildType Build { get; set; }

        public ChannelType Channel { get; set; }

        public string FileName { get; set; }

        public DateTime ReleaseDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int RemainingTime { get; set; }

        public bool IsCurrent { get; set; }

        public List<PhotonView> Servers { get; set; }

        public string SupportUrl { get; set; }

        public int PhotonGroupId { get; set; }

        public string PhotonGroupName { get; set; }

        #endregion Properties

        #region Constructors

        public ApplicationView()
        {
            this.Servers = new List<PhotonView>();
        }

        public ApplicationView(string version, BuildType build, ChannelType channel)
        {
            this.Version = version;
            this.Build = build;
            this.Channel = channel;
            this.Servers = new List<PhotonView>();
        }

        public ApplicationView(int applicationVersionId, string version, BuildType build, ChannelType channel, string fileName, DateTime releaseDate, DateTime? expirationDate, bool isCurrent, string supportUrl, int photonGroupId, List<PhotonView> servers)
        {
            int remainingTime = -1;
            if (expirationDate != null && releaseDate != null)
            {
                if (expirationDate.HasValue)
                {
                    DateTime expirationDateValue = expirationDate.Value;

                    if (expirationDateValue.CompareTo(DateTime.Now) <= 0)
                    {
                        remainingTime = 0;
                    }
                    else
                    {
                        TimeSpan remainingPeriod = DateTime.Now.Subtract(expirationDateValue);
                        remainingTime = (int)Math.Floor(remainingPeriod.TotalMinutes);
                    }
                }
            }

            SetApplication(applicationVersionId, version, build, channel, fileName, releaseDate, expirationDate, remainingTime, isCurrent, supportUrl, photonGroupId, servers);
        }

        #endregion Constructors

        #region Methods

        private void SetApplication(int applicationVersionID, string version, BuildType build, ChannelType channel, string fileName, DateTime releaseDate, DateTime? expirationDate, int remainingTime, bool isCurrent, string supportUrl, int photonGroupId, List<PhotonView> servers)
        {
            this.ApplicationVersionId = applicationVersionID;
            this.Version = version;
            this.Build = build;
            this.Channel = channel;
            this.FileName = fileName;
            this.ReleaseDate = releaseDate;
            this.ExpirationDate = expirationDate;
            this.RemainingTime = remainingTime;
            this.IsCurrent = isCurrent;
            this.SupportUrl = supportUrl;
            this.PhotonGroupId = photonGroupId;

            if (servers != null)
            {
                this.Servers = servers;
            }
            else
            {
                this.Servers = new List<PhotonView>();
            }
        }

        public override string ToString()
        {
            string applicationDisplay = "[Application: ";
            applicationDisplay += "[ID: " + this.ApplicationVersionId + "][version: " + this.Version + "][Build: " + this.Build + "][Channel: " + this.Channel + "][File name: " + this.FileName + "][Release date: " + this.ReleaseDate + "][Expiration date: " + this.ExpirationDate + "][Remaining time: " + this.RemainingTime + "][Is current: " + this.IsCurrent + "][Support URL: " + this.SupportUrl + "]";
            applicationDisplay += "[Servers]";
            foreach (PhotonView photon in this.Servers)
            {
                applicationDisplay += photon.ToString();
            }
            applicationDisplay += "[/Servers]]";


            applicationDisplay += "]";

            return applicationDisplay;
        }

        #endregion Methods
    }
}