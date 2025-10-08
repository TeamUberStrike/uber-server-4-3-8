
using System;
using System.IO;
using System.Text;
using Cmune.Realtime.Photon.Server.Diagnostics;
using Cmune.Util;
using log4net.Config;
using PhotonHostRuntimeInterfaces;

namespace UberStrike.Realtime.Photon.PolicyServer
{
    /// <summary>
    /// Application serving policy files in response of FlashPlayer requests.
    /// </summary>
    public class PolicyServerApplication : IPhotonApplication, IPhotonApplicationControl
    {
        /// <summary>
        /// Name/ID of this CLR application.
        /// </summary>
        private string applicationId;

        private byte[] policyBytesUtf8;

        // usually the Photon.SocketServer.Application provides the path but Policy is not inheriting that
        private string applicationPath;

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                CmuneDebug.LogWarning("Process is terminating.");
            }

            CmuneDebug.Exception(((Exception)e.ExceptionObject).Message);
        }

        #region IPhotonApplicationControl Members

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyServerApplication"/> class.
        /// </summary>
        public PolicyServerApplication()
        {
            this.applicationPath = Environment.CurrentDirectory;

            XmlConfigurator.ConfigureAndWatch(new FileInfo("bin\\log4net.config"));

            CmuneDebug.AddDebugChannel(new DefaultDebug());
            CmuneDebug.AddDebugChannel(new PhotonDebug());

            CmunePerformanceCounter.GetPhysicalMemory();
        }

        /// <summary>
        ///   Called when the application starts.
        /// </summary>
        /// <param name = "instanceName">The instance name.</param>
        /// <param name = "applicationName">Name/ID of the application.</param>
        /// <param name = "sink">The PhotonApplicationSink.</param>
        /// <returns>PhotonApplication object.</returns>
        public IPhotonApplication OnStart(string instanceName, string applicationName, IPhotonApplicationSink sink)
        {
            // this app does not inherit from Photon.SocketServer.Application, so it's missing the ApplicationPath
            // get the ApplicationPath "manually": Environment.CurrentDirectory is overwritten when the next app starts, so copy the value
            this.applicationId = applicationName;
            return this;
        }

        /// <summary>
        ///   Photon is now running.
        /// </summary>
        public void OnPhotonRunning()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            string path = Path.Combine(this.applicationPath, "bin/log4net.config");
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                XmlConfigurator.Configure(fileInfo);
            }

            try
            {
                this.policyBytesUtf8 = ReadPolicyFile(Path.Combine(this.applicationPath, "bin/assets/socket-policy.xml"));
            }
            catch (Exception e)
            {
                throw new CmuneException(e.Message);
            }

            CmuneDebug.Log("Policy Server running!");
        }
        /// <summary>
        /// Called when the application stops.
        /// </summary>
        public void OnStop()
        { }

        #endregion

        private static byte[] ReadPolicyFile(string fileName)
        {
            byte[] policyUtf8Binary;
            string policy;

            if (CmuneDebug.IsDebugEnabled)
            {
                CmuneDebug.Log("Reading policy file: " + fileName);
            }

            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (var sr = new StreamReader(fileName))
            {
                Encoding utf8Encoding = Encoding.UTF8;
                policy = sr.ReadToEnd();

                policyUtf8Binary = utf8Encoding.GetBytes(policy);
            }

            if (CmuneDebug.IsDebugEnabled)
            {
                CmuneDebug.Log("Policy file: \n{0}", policy);
            }

            return policyUtf8Binary;
        }

        public void OnInit(IPhotonPeer peer, byte[] data)
        {
            CmuneDebug.Log("OnInit from IP {0} with data array of length {1}", peer.GetRemoteIP(), (data != null ? data.Length : -1));

            //if (CmuneDebug.IsDebugEnabled)
            //{
            //    CmuneDebug.LogFormat("OnInit - {0}", Encoding.UTF8.GetString(data));
            //}

            if (data.Length >= 22 && data[0] == '<' && data[21] == '>')
            {
                // any other requesting client gets flash response
                peer.Send(this.policyBytesUtf8, MessageReliablity.Reliable, 0);

                if (CmuneDebug.IsDebugEnabled)
                {
                    CmuneDebug.Log("Policy sent.");
                }
            }
            else
            {
                CmuneDebug.Log("OnInit - malformed request from peer {0}", peer.GetRemoteIP());
            }

            peer.DisconnectClient();
        }

        public void OnOperationReceive(IPhotonPeer peer, byte[] data, short invocationId, MessageReliablity reliability, byte channelId)
        {
            if (CmuneDebug.IsDebugEnabled)
            {
                CmuneDebug.Log("OnOperationReceive");
            }
        }

        public void OnDisconnect(IPhotonPeer peer)
        {
            if (CmuneDebug.IsDebugEnabled)
            {
                CmuneDebug.Log("OnDisconnect");
            }
        }

        public void OnStopRequested()
        { }

        public void OnDisconnect(IPhotonPeer peer, object userData)
        { }

        public void OnFlowControlEvent(IPhotonPeer peer, object userData, FlowControlEvent FlowControlEvent)
        { }

        public void OnOutboundConnectionEstablished(IPhotonPeer peer, object userData)
        { }

        public void OnOutboundConnectionFailed(IPhotonPeer peer, object userData, int errorCode, string errorMessage)
        { }

        public void OnReceive(IPhotonPeer peer, object userData, byte[] data, MessageReliablity reliability, byte channelId)
        { }
    }
}