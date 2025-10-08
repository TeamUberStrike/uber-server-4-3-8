using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Cmune.DataCenter.Common.Entities;
using Cmune.DataCenter.Common.Utils;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.DataAccess;
using Cmune.Instrumentation.Monitoring.Common.Entities;

namespace Cmune.Instrumentation.Monitoring.Business
{
    /// <summary>
    /// Allows us to monitor our servers
    /// </summary>
    public static class ServersMonitoring
    {
        /// <summary>
        /// Testing interval
        /// </summary>
        public static readonly int TestingInterval = 900; // seconds
        /// <summary>
        /// Testing loop count
        /// </summary>
        public static readonly int TestingLoopCount = 4;

        public static ManagedServerModel ToManagedServerModel(this ManagedServer managedServer)
        {
            var managedServerModel = new ManagedServerModel();

            managedServerModel.ManagedServerId2 = managedServer.ManagedServerID;
            managedServerModel.ServerName = managedServer.ServerName;
            managedServerModel.ServerIp = managedServer.ServerIP;
            managedServerModel.NextPollTime = managedServer.NextPollTime;
            managedServerModel.DeploymentTime = managedServer.DeploymentTime;
            managedServerModel.IsDisable = managedServer.IsDisable;
            managedServerModel.ServerIDC = managedServer.ServerIDC;
            managedServerModel.Region = managedServer.Region;
            managedServerModel.CPUModel = managedServer.CPUModel;
            managedServerModel.CPUSpeed = managedServer.CPUSpeed;
            managedServerModel.CPUCore = managedServer.CPUCore;
            managedServerModel.CPUs = managedServer.CPUs;
            managedServerModel.RAM = managedServer.RAM;
            managedServerModel.AllowedBandwidth = managedServer.AllowedBandwidth;
            managedServerModel.Note = managedServer.Note;
            return managedServerModel;
        }

        public static ManagedServer ToManagedServer(this ManagedServerModel managedServerModel)
        {
            var managedServer = new ManagedServer();

            managedServer.ManagedServerID = managedServerModel.ManagedServerId2;
            managedServer.ServerName = managedServerModel.ServerName;
            managedServer.ServerIP = managedServerModel.ServerIp;
            managedServer.NextPollTime = managedServerModel.NextPollTime;
            managedServer.DeploymentTime = managedServerModel.DeploymentTime;
            managedServer.IsDisable = managedServerModel.IsDisable;
            managedServer.ServerIDC = managedServerModel.ServerIDC;
            managedServer.Region = managedServerModel.Region;
            managedServer.CPUModel = managedServerModel.CPUModel;
            managedServer.CPUSpeed = managedServerModel.CPUSpeed;
            managedServer.CPUCore = managedServerModel.CPUCore;
            managedServer.CPUs = managedServer.CPUs;
            managedServer.RAM = managedServerModel.RAM;
            managedServer.AllowedBandwidth = managedServerModel.AllowedBandwidth;
            managedServer.Note = managedServerModel.Note;
            return managedServer;
        }

        public static void CopyFromManagedServerModel(this ManagedServer managedServer, ManagedServerModel managedServerModel)
        {
            managedServer.ServerName = ValidationUtilities.StandardizeManagedServerName(managedServerModel.ServerName);
            managedServer.ServerIP = managedServerModel.ServerIp;
            managedServer.NextPollTime = managedServerModel.NextPollTime;
            managedServer.DeploymentTime = managedServerModel.DeploymentTime;
            managedServer.IsDisable = managedServerModel.IsDisable;
            managedServer.ServerIDC = managedServerModel.ServerIDC;
            managedServer.Region = managedServerModel.Region;
            managedServer.CPUModel = managedServerModel.CPUModel;
            managedServer.CPUSpeed = managedServerModel.CPUSpeed;
            managedServer.CPUCore = managedServerModel.CPUCore;
            managedServer.CPUs = managedServerModel.CPUs;
            managedServer.RAM = managedServerModel.RAM;
            managedServer.AllowedBandwidth = managedServerModel.AllowedBandwidth;
            managedServer.Note = managedServerModel.Note;
        }

        public static IQueryable<ManagedServerModel> ToManagedServerModelQueryable(this IQueryable<ManagedServer> managedServerList)
        {
            var managedserverModelList = from managedServerModel in managedServerList select managedServerModel.ToManagedServerModel();
            return managedserverModelList;
        }

        /// <summary>
        /// Get a specific managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static ManagedServerModel GetManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);
                if(managedServer != null)
                    return managedServer.ToManagedServerModel();
                return null;
            }
        }


        /// <summary>
        /// Delete a specific managed server and its test histories
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <returns></returns>
        public static bool DeleteManagedServer(int managedServerId)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                var managedServer = monitoringDB.ManagedServers.SingleOrDefault(d => d.ManagedServerID == managedServerId);
                if (managedServer != null)
                {
                    monitoringDB.ManagedServers.DeleteOnSubmit(managedServer);


                    var databaseTests = monitoringDB.DatabaseTests.Where(dbTest => dbTest.ManagedServerID == managedServerId);
                    // delete database test histories
                    monitoringDB.DatabaseTestHistories.DeleteAllOnSubmit(monitoringDB.DatabaseTestHistories.Where(dbTestH => databaseTests.Select(dbTest2 => dbTest2.DatabaseTestID).Contains(dbTestH.DatabaseTestID)));
                    // delete database test 
                    monitoringDB.DatabaseTests.DeleteAllOnSubmit(databaseTests);

                    var photonTests = monitoringDB.PhotonTests.Where(photonTest => photonTest.ManagedServerID == managedServerId);
                    // delete photon test histories
                    monitoringDB.PhotonTestHistories.DeleteAllOnSubmit(monitoringDB.PhotonTestHistories.Where(photonTestH => photonTests.Select(photoTest2 => photoTest2.PhotonTestID).Contains(photonTestH.PhotonTestID)));
                    // delete photon test
                    monitoringDB.PhotonTests.DeleteAllOnSubmit(monitoringDB.PhotonTests.Where(photTest => photTest.ManagedServerID == managedServerId));


                    var websiteTests = monitoringDB.WebsiteTests.Where(websiteTest => websiteTest.ManagedServerID == managedServerId);
                    // delete website test histories
                    monitoringDB.WebsiteTestHistories.DeleteAllOnSubmit(monitoringDB.WebsiteTestHistories.Where(websiteTestH => websiteTests.Select(websiteTest2 => websiteTest2.WebsiteTestID).Contains(websiteTestH.WebsiteTestID)));
                    // delete website test
                    monitoringDB.WebsiteTests.DeleteAllOnSubmit(monitoringDB.WebsiteTests.Where(websiteTest => websiteTest.ManagedServerID == managedServerId));

                    var webserviceTests = monitoringDB.WebserviceTests.Where(webserviceTest => webserviceTest.ManagedServerID == managedServerId);
                    // delete webservice test histories
                    monitoringDB.WebserviceTestHistories.DeleteAllOnSubmit(monitoringDB.WebserviceTestHistories.Where(webserviceTestH => webserviceTests.Select(webserviceTest2 => webserviceTest2.WebserviceTestID).Contains(webserviceTestH.WebserviceTestID)));
                    // delete webservice test
                    monitoringDB.WebserviceTests.DeleteAllOnSubmit(monitoringDB.WebserviceTests.Where(webserviceTest => webserviceTest.ManagedServerID == managedServerId));

                    var pollHistories = monitoringDB.PollHistories.Where(ph => ph.ManagedServerID == managedServerId);
                    monitoringDB.LastPolls.DeleteAllOnSubmit(monitoringDB.LastPolls.Where(lp => pollHistories.Select(ph2 => ph2.PollHistoryID).Contains(lp.PollHistoryID)));
                    monitoringDB.PollHistories.DeleteAllOnSubmit(pollHistories);
                    monitoringDB.SubmitChanges();

                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Get all the servers managed by this system
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServerModel> GetManagedServers()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServerModel> managedServerModelList = monitoringDB.ManagedServers.ToManagedServerModelQueryable().ToList();

                return managedServerModelList;
            }
        }

        /// <summary>
        /// Get the servers that are enabled
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServer> GetMonitoredServers()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServer> managedServers = monitoringDB.ManagedServers.Where(mS => mS.IsDisable == false).ToList();

                return managedServers;
            }
        }

        /// <summary>
        /// Get the servers to test now
        /// </summary>
        /// <returns></returns>
        public static List<ManagedServer> GetManagedServersToTest()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<ManagedServer> managedServersToTest = monitoringDB.ManagedServers.Where(mS => mS.NextPollTime < DateTime.Now && mS.IsDisable == false).ToList();

                return managedServersToTest;
            }
        }

        /// <summary>
        /// Get the servers ID to test now
        /// </summary>
        /// <returns></returns>
        public static List<int> GetManagedServersIDsToTest()
        {
            return GetManagedServersToTest().Select(mS => mS.ManagedServerID).ToList();
        }

        /// <summary>
        /// Get all the tests indexed by server ID
        /// </summary>
        /// <returns></returns>
        public static List<TestsPoll> GetPollsToTest()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                Dictionary<int, TestsPoll> tests = new Dictionary<int, TestsPoll>();

                List<ManagedServer> serversToTest = GetManagedServersToTest();
                List<int> serversIDsToTest = serversToTest.Select(mS => mS.ManagedServerID).ToList();
                Dictionary<int, string> serversName = serversToTest.ToDictionary(sTT => sTT.ManagedServerID, sTT => sTT.ServerName);

                List<PhotonTest> photonTests = GetPhotonsToTest(serversIDsToTest);
                List<WebserviceTest> webserviceTests = GetWebServicesToTest(serversIDsToTest);
                List<WebsiteTest> websiteTests = GetWebsitesToTest(serversIDsToTest);
                List<DatabaseTest> databaseTests = GetDatabasesToTest(serversIDsToTest);

                foreach (PhotonTest photonTest in photonTests)
                {
                    if (tests.ContainsKey(photonTest.ManagedServerID))
                    {
                        tests[photonTest.ManagedServerID].AddPhotonTest(photonTest);
                    }
                    else
                    {
                        TestsPoll testsPoll = new TestsPoll(photonTest.ManagedServerID, serversName[photonTest.ManagedServerID]);
                        testsPoll.AddPhotonTest(photonTest);
                        tests.Add(photonTest.ManagedServerID, testsPoll);
                    }
                }

                foreach (WebserviceTest webserviceTest in webserviceTests)
                {
                    if (tests.ContainsKey(webserviceTest.ManagedServerID))
                    {
                        tests[webserviceTest.ManagedServerID].AddWebserviceTest(webserviceTest);
                    }
                    else
                    {
                        TestsPoll testsPoll = new TestsPoll(webserviceTest.ManagedServerID, serversName[webserviceTest.ManagedServerID]);
                        testsPoll.AddWebserviceTest(webserviceTest);
                        tests.Add(webserviceTest.ManagedServerID, testsPoll);
                    }
                }

                foreach (WebsiteTest websiteTest in websiteTests)
                {
                    if (tests.ContainsKey(websiteTest.ManagedServerID))
                    {
                        tests[websiteTest.ManagedServerID].AddWebsiteTest(websiteTest);
                    }
                    else
                    {
                        TestsPoll testsPoll = new TestsPoll(websiteTest.ManagedServerID, serversName[websiteTest.ManagedServerID]);
                        testsPoll.AddWebsiteTest(websiteTest);
                        tests.Add(websiteTest.ManagedServerID, testsPoll);
                    }
                }

                foreach (DatabaseTest databaseTest in databaseTests)
                {
                    if (tests.ContainsKey(databaseTest.ManagedServerID))
                    {
                        tests[databaseTest.ManagedServerID].AddDatabaseTest(databaseTest);
                    }
                    else
                    {
                        TestsPoll testsPoll = new TestsPoll(databaseTest.ManagedServerID, serversName[databaseTest.ManagedServerID]);
                        testsPoll.AddDatabaseTest(databaseTest);
                        tests.Add(databaseTest.ManagedServerID, testsPoll);
                    }
                }

                return tests.Values.ToList();
            }
        }

        /// <summary>
        /// Get all the photon tests
        /// </summary>
        /// <returns></returns>
        public static List<PhotonTest> GetPhotonTests()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PhotonTest> photonTests = monitoringDB.PhotonTests.ToList();

                return photonTests;
            }
        }

        /// <summary>
        /// Get the photon tests on a specific server
        /// </summary>
        /// <param name="serversToTestID"></param>
        /// <returns></returns>
        public static List<PhotonTest> GetPhotonsTests(int serversToTestID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PhotonTest> photonTests = monitoringDB.PhotonTests.Where(pT => pT.ManagedServerID == serversToTestID).ToList();

                return photonTests;
            }
        }

        /// <summary>
        /// Get the photons to test on specific servers
        /// </summary>
        /// <param name="serversToTestIDs"></param>
        /// <returns></returns>
        public static List<PhotonTest> GetPhotonsToTest(List<int> serversToTestIDs)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PhotonTest> photonTests = monitoringDB.PhotonTests.Where(pT => serversToTestIDs.Contains(pT.ManagedServerID) && pT.IsDisable == false).ToList();

                return photonTests;
            }
        }

        /// <summary>
        /// Get all the web service tests
        /// </summary>
        /// <returns></returns>
        public static List<WebserviceTest> GetWebServiceTests()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebserviceTest> webServiceTests = monitoringDB.WebserviceTests.ToList();

                return webServiceTests;
            }
        }

        /// <summary>
        /// Get the web service tests on a specific server
        /// </summary>
        /// <param name="serversToTestID"></param>
        /// <returns></returns>
        public static List<WebserviceTest> GetWebServiceTests(int serversToTestID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebserviceTest> webServiceTests = monitoringDB.WebserviceTests.Where(wT => wT.ManagedServerID == serversToTestID).ToList();

                return webServiceTests;
            }
        }

        /// <summary>
        /// Get the web services to test on specific servers
        /// </summary>
        /// <param name="serversToTestIDs"></param>
        /// <returns></returns>
        public static List<WebserviceTest> GetWebServicesToTest(List<int> serversToTestIDs)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebserviceTest> webServiceTests = monitoringDB.WebserviceTests.Where(wT => serversToTestIDs.Contains(wT.ManagedServerID) && wT.IsDisable == false).ToList();

                return webServiceTests;
            }
        }

        /// <summary>
        /// Get all the website tests
        /// </summary>
        /// <returns></returns>
        public static List<WebsiteTest> GetWebsiteTests()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebsiteTest> websiteTests = monitoringDB.WebsiteTests.ToList();

                return websiteTests;
            }
        }

        /// <summary>
        /// Get the website tests on a specific server
        /// </summary>
        /// <param name="serversToTestID"></param>
        /// <returns></returns>
        public static List<WebsiteTest> GetWebsiteTests(int serversToTestID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebsiteTest> websiteTests = monitoringDB.WebsiteTests.Where(wT => wT.ManagedServerID == serversToTestID).ToList();

                return websiteTests;
            }
        }

        /// <summary>
        /// Get the website to test on specific servers
        /// </summary>
        /// <param name="serversToTestIDs"></param>
        /// <returns></returns>
        public static List<WebsiteTest> GetWebsitesToTest(List<int> serversToTestIDs)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebsiteTest> websiteTests = monitoringDB.WebsiteTests.Where(wT => serversToTestIDs.Contains(wT.ManagedServerID) && wT.IsDisable == false).ToList();

                return websiteTests;
            }
        }

        /// <summary>
        /// Get all the database tests
        /// </summary>
        /// <returns></returns>
        public static List<DatabaseTest> GetDatabaseTests()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<DatabaseTest> databasesTests = monitoringDB.DatabaseTests.ToList();

                return databasesTests;
            }
        }

        /// <summary>
        /// Get the database tests on a specific server
        /// </summary>
        /// <param name="serversToTestID"></param>
        /// <returns></returns>
        public static List<DatabaseTest> GetDatabaseTests(int serversToTestID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<DatabaseTest> databasesTests = monitoringDB.DatabaseTests.Where(dT => dT.ManagedServerID == serversToTestID).ToList();

                return databasesTests;
            }
        }

        /// <summary>
        /// Get the database to test on specific servers
        /// </summary>
        /// <param name="serversToTestIDs"></param>
        /// <returns></returns>
        public static List<DatabaseTest> GetDatabasesToTest(List<int> serversToTestIDs)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<DatabaseTest> databasesTests = monitoringDB.DatabaseTests.Where(dT => serversToTestIDs.Contains(dT.ManagedServerID) && dT.IsDisable == false).ToList();

                return databasesTests;
            }
        }

        /// <summary>
        /// Get the database tests history for a specific poll history
        /// </summary>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        public static List<DatabaseTestHistory> GetDatabaseTestHistory(int pollHistoryID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<DatabaseTestHistory> databaseTestHistory = monitoringDB.DatabaseTestHistories.Where(dTH => dTH.PollHistoryID == pollHistoryID).ToList();

                return databaseTestHistory;
            }
        }

        /// <summary>
        /// Get the photon tests history for a specific poll history
        /// </summary>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        public static List<PhotonTestHistory> GetPhotonTestHistory(int pollHistoryID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PhotonTestHistory> photonTestHistory = monitoringDB.PhotonTestHistories.Where(pTH => pTH.PollHistoryID == pollHistoryID).ToList();

                return photonTestHistory;
            }
        }

        /// <summary>
        /// Get the web service tests history for a specific poll history
        /// </summary>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        public static List<WebserviceTestHistory> GetWebserviceTestHistory(int pollHistoryID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebserviceTestHistory> webserviceTestHistory = monitoringDB.WebserviceTestHistories.Where(wTH => wTH.PollHistoryID == pollHistoryID).ToList();

                return webserviceTestHistory;
            }
        }

        /// <summary>
        /// Get the website tests history for a specific poll history
        /// </summary>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        public static List<WebsiteTestHistory> GetWebsiteTestHistory(int pollHistoryID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<WebsiteTestHistory> websiteTestHistory = monitoringDB.WebsiteTestHistories.Where(wTH => wTH.PollHistoryID == pollHistoryID).ToList();

                return websiteTestHistory;
            }
        }

        /// <summary>
        /// Run all the tests contained in the polls
        /// </summary>
        /// <param name="testsPolls"></param>
        public static void TestPolls(List<TestsPoll> testsPolls)
        {
            if (testsPolls != null)
            {
                foreach (TestsPoll testPoll in testsPolls)
                {
                    TestPoll(testPoll);
                }
            }
        }

        /// <summary>
        /// Run all the tests for all the polls
        /// </summary>
        public static void TestPolls()
        {
            List<TestsPoll> tetsPolls = GetPollsToTest();
            TestPolls(tetsPolls);
        }

        /// <summary>
        /// Run all the tests of a specific poll
        /// </summary>
        /// <param name="testsPoll"></param>
        public static void TestPoll(TestsPoll testsPoll)
        {
            if (testsPoll != null && testsPoll.CountTests() > 0)
            {
                //StringBuilder logOuput = new StringBuilder();
                long processMemory = 0;
                long garbageCollectorMemory = 0;

                //logOuput.Append("[Server name:");
                //logOuput.Append(testsPoll.ManagedServerName);
                //logOuput.Append("]");

                Process MyProcess = System.Diagnostics.Process.GetCurrentProcess();

                // Measure starting point memory use
                garbageCollectorMemory = System.GC.GetTotalMemory(true);
                processMemory = MyProcess.PrivateMemorySize64;

                //logOuput.Append("[Memory:");
                //logOuput.Append(processMemory);
                //logOuput.Append("]");
                //logOuput.Append("[GC memory:");
                //logOuput.Append(garbageCollectorMemory);
                //logOuput.Append("]");

                string alertMessageTemplate = "SERVER DOWN: {0} - {1} ({2})";

                using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
                {
                    PollHistory pollHistory = new PollHistory();
                    pollHistory.IsAlive = true;
                    pollHistory.ManagedServerID = testsPoll.ManagedServerID;
                    pollHistory.PollTime = DateTime.Now;
                    pollHistory.ServerName = testsPoll.ManagedServerName;

                    monitoringDB.PollHistories.InsertOnSubmit(pollHistory);
                    monitoringDB.SubmitChanges();

                    // In case of exception we write the next poll date now

                    ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == pollHistory.ManagedServerID);

                    if (managedServer != null)
                    {
                        managedServer.NextPollTime = DateTime.Now.AddSeconds(TestingInterval);
                    }

                    monitoringDB.SubmitChanges();

                    bool isServerAlive = true;
                    bool shouldAlert = false;
                    string alertMessage = String.Empty;

                    // To factor in methods maybe
                    List<PhotonTestHistory> photonTestHistories = new List<PhotonTestHistory>();

                    foreach (PhotonTest photonTest in testsPoll.PhotonTests)
                    {
                        //logOuput.Append("[Photon test:");
                        //logOuput.Append(photonTest.TestName);
                        //logOuput.Append("]");

                        int failCount = 0;
                        bool isAlive = true;

                        for (int i = 0; i < TestingLoopCount; i++)
                        {
                            if (!IsPhotonAlive(photonTest.Socket, photonTest.TestName))
                            {
                                failCount++;

                                DateTime now = DateTime.Now;

                                while (DateTime.Now.Subtract(now).TotalSeconds < 3) { }
                            }

                            if (failCount > TestingLoopCount / 2)
                            {
                                isAlive = false;
                                i = TestingLoopCount;
                            }
                        }

                        PhotonTestHistory photonTestHistory = new PhotonTestHistory();
                        photonTestHistory.IsAlive = isAlive;
                        photonTestHistory.PhotonTestID = photonTest.PhotonTestID;
                        photonTestHistory.PollHistoryID = pollHistory.PollHistoryID;
                        photonTestHistory.TestName = photonTest.TestName;

                        photonTestHistories.Add(photonTestHistory);

                        if (!isAlive)
                        {
                            isServerAlive = false;

                            if (alertMessage.IsNullOrFullyEmpty())
                            {
                                alertMessage = String.Format(alertMessageTemplate, "Photon", photonTest.Socket, testsPoll.ManagedServerName);
                            }
                        }

                        garbageCollectorMemory = System.GC.GetTotalMemory(true);
                        processMemory = MyProcess.PrivateMemorySize64;

                        //logOuput.Append("[Memory:");
                        //logOuput.Append(processMemory);
                        //logOuput.Append("]");
                        //logOuput.Append("[GC memory:");
                        //logOuput.Append(garbageCollectorMemory);
                        //logOuput.Append("]");
                    }

                    monitoringDB.PhotonTestHistories.InsertAllOnSubmit(photonTestHistories);

                    List<DatabaseTestHistory> databaseTestHistories = new List<DatabaseTestHistory>();

                    foreach (DatabaseTest databaseTest in testsPoll.DatabaseTests)
                    {
                        //logOuput.Append("[DB test:");
                        //logOuput.Append(databaseTest.TestName);
                        //logOuput.Append("]");

                        int failCount = 0;
                        bool isAlive = true;

                        for (int i = 0; i < TestingLoopCount; i++)
                        {
                            if (!IsDatabaseAlive(databaseTest.ConnectionString, databaseTest.SqlQuery))
                            {
                                failCount++;
                            }

                            if (failCount > TestingLoopCount / 2)
                            {
                                isAlive = false;
                                i = TestingLoopCount;
                            }
                        }

                        DatabaseTestHistory databaseTestHistory = new DatabaseTestHistory();
                        databaseTestHistory.DatabaseTestID = databaseTest.DatabaseTestID;
                        databaseTestHistory.IsAlive = isAlive;
                        databaseTestHistory.PollHistoryID = pollHistory.PollHistoryID;
                        databaseTestHistory.TestName = databaseTest.TestName;

                        databaseTestHistories.Add(databaseTestHistory);

                        if (!isAlive)
                        {
                            isServerAlive = false;

                            if (alertMessage.IsNullOrFullyEmpty())
                            {
                                alertMessage = String.Format(alertMessageTemplate, "Database", databaseTest.TestName, testsPoll.ManagedServerName);
                            }
                        }

                        //logOuput.Append("[Memory:");
                        //logOuput.Append(processMemory);
                        //logOuput.Append("]");
                        //logOuput.Append("[GC memory:");
                        //logOuput.Append(garbageCollectorMemory);
                        //logOuput.Append("]");
                    }

                    monitoringDB.DatabaseTestHistories.InsertAllOnSubmit(databaseTestHistories);

                    List<WebserviceTestHistory> webserviceTestHistories = new List<WebserviceTestHistory>();

                    foreach (WebserviceTest webServiceTest in testsPoll.WebServiceTests)
                    {
                        //logOuput.Append("[WCF test:");
                        //logOuput.Append(webServiceTest.TestName);
                        //logOuput.Append("]");

                        int failCount = 0;
                        bool isAlive = true;

                        for (int i = 0; i < TestingLoopCount; i++)
                        {
                            if (!IsWebServiceAlive(webServiceTest.WebserviceUrl, webServiceTest.PassPhrase, webServiceTest.InitVector, webServiceTest.HttpPostFieldName, webServiceTest.HttpPostContent))
                            {
                                failCount++;
                            }

                            if (failCount > TestingLoopCount / 2)
                            {
                                isAlive = false;
                                i = TestingLoopCount;
                            }
                        }

                        WebserviceTestHistory webserviceTestHistory = new WebserviceTestHistory();
                        webserviceTestHistory.IsAlive = isAlive;
                        webserviceTestHistory.PollHistoryID = pollHistory.PollHistoryID;
                        webserviceTestHistory.WebserviceTestID = webServiceTest.WebserviceTestID;
                        webserviceTestHistory.TestName = webServiceTest.TestName;

                        webserviceTestHistories.Add(webserviceTestHistory);

                        if (!isAlive)
                        {
                            isServerAlive = false;

                            if (alertMessage.IsNullOrFullyEmpty())
                            {
                                alertMessage = String.Format(alertMessageTemplate, "Web Services", webServiceTest.TestName, testsPoll.ManagedServerName);
                            }
                        }

                        //logOuput.Append("[Memory:");
                        //logOuput.Append(processMemory);
                        //logOuput.Append("]");
                        //logOuput.Append("[GC memory:");
                        //logOuput.Append(garbageCollectorMemory);
                        //logOuput.Append("]");
                    }

                    monitoringDB.WebserviceTestHistories.InsertAllOnSubmit(webserviceTestHistories);

                    List<WebsiteTestHistory> websiteTestHistories = new List<WebsiteTestHistory>();

                    foreach (WebsiteTest websiteTest in testsPoll.WebsiteTests)
                    {
                        //logOuput.Append("[Website test:");
                        //logOuput.Append(websiteTest.TestName);
                        //logOuput.Append("]");

                        int failCount = 0;
                        bool isAlive = true;

                        for (int i = 0; i < TestingLoopCount; i++)
                        {
                            if (!IsWebsiteAlive(websiteTest.WebsiteUrl, websiteTest.ExpectedResponseUrl))
                            {
                                failCount++;
                            }

                            if (failCount > TestingLoopCount / 2)
                            {
                                isAlive = false;
                                i = TestingLoopCount;
                            }
                        }

                        WebsiteTestHistory websiteTestHistory = new WebsiteTestHistory();
                        websiteTestHistory.IsAlive = isAlive;
                        websiteTestHistory.PollHistoryID = pollHistory.PollHistoryID;
                        websiteTestHistory.WebsiteTestID = websiteTest.WebsiteTestID;
                        websiteTestHistory.TestName = websiteTest.TestName;

                        websiteTestHistories.Add(websiteTestHistory);

                        if (!isAlive)
                        {
                            isServerAlive = false;

                            if (alertMessage.IsNullOrFullyEmpty())
                            {
                                alertMessage = String.Format(alertMessageTemplate, "Website", websiteTest.TestName, testsPoll.ManagedServerName);
                            }
                        }

                        //logOuput.Append("[Memory:");
                        //logOuput.Append(processMemory);
                        //logOuput.Append("]");
                        //logOuput.Append("[GC memory:");
                        //logOuput.Append(garbageCollectorMemory);
                        //logOuput.Append("]");
                    }

                    monitoringDB.WebsiteTestHistories.InsertAllOnSubmit(websiteTestHistories);

                    pollHistory.IsAlive = isServerAlive;

                    LastPoll lastPoll = monitoringDB.LastPolls.SingleOrDefault(lP => lP.ManagedServerID == pollHistory.ManagedServerID);

                    if (lastPoll != null)
                    {
                        if (lastPoll.IsAlive && !isServerAlive)
                        {
                            shouldAlert = true;
                        }

                        lastPoll.IsAlive = pollHistory.IsAlive;
                        lastPoll.PollTime = pollHistory.PollTime;
                        lastPoll.PollHistoryID = pollHistory.PollHistoryID;
                    }
                    else
                    {
                        if (!isServerAlive)
                        {
                            shouldAlert = true;
                        }

                        lastPoll = new LastPoll();
                        lastPoll.IsAlive = pollHistory.IsAlive;
                        lastPoll.ManagedServerID = pollHistory.ManagedServerID;
                        lastPoll.PollTime = pollHistory.PollTime;
                        lastPoll.ServerName = pollHistory.ServerName;
                        lastPoll.PollHistoryID = pollHistory.PollHistoryID;

                        monitoringDB.LastPolls.InsertOnSubmit(lastPoll);
                    }

                    if (shouldAlert)
                    {
                        SendAlert(alertMessage);
                    }

                    monitoringDB.SubmitChanges();
                }

                //logOuput.Append("[Memory:");
                //logOuput.Append(processMemory);
                //logOuput.Append("]");
                //logOuput.Append("[GC memory:");
                //logOuput.Append(garbageCollectorMemory);
                //logOuput.Append("]");

                //CmuneLog.CustomLogToDefaultPath("Cmune.Instrumentation.Monitoring.log", logOuput.ToString());
            }
        }

        /// <summary>
        /// Sends SMS and email alerts
        /// <param name="alertMessage"></param>
        /// </summary>
        public static void SendAlert(string alertMessage)
        {
            List<RotationMember> membersToAlert = MonitoringRotation.GetMembersToAlert();

            string smsIntervallsConfig = ConfigurationUtilities.ReadConfigurationManager("SmsAlertIntervalls");

            List<int> smsIntervalls = new List<int>();

            if (!smsIntervallsConfig.IsNullOrFullyEmpty())
            {
                string[] merde = smsIntervallsConfig.Split('|');
                smsIntervalls = Array.ConvertAll(merde, s => Int32.Parse(s)).ToList();
            }

            foreach (RotationMember memberToAlert in membersToAlert)
            {
                CmuneMail.SendAlertEmail(memberToAlert.MemberEmail, memberToAlert.MemberName, alertMessage);
                CmuneSms cmuneSms = new CmuneSms();
                cmuneSms.SendMonitoringAlertSms(memberToAlert.MemberCellNumber, alertMessage, smsIntervalls);
            }
        }

        /// <summary>
        /// Test whether a website is live or not
        /// To be called when the excpected returned URL should be the same
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsWebsiteAlive(string url)
        {
            return IsWebsiteAlive(url, url);
        }

        /// <summary>
        /// Test whether a website is live or not
        /// To be called when the excpected returned URL will be different (redirection)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="expectedResponseUrl"></param>
        /// <returns></returns>
        public static bool IsWebsiteAlive(string url, string expectedResponseUrl)
        {
            bool isAlive = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 3000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if ((request.HaveResponse) && (response.StatusCode == HttpStatusCode.OK) && expectedResponseUrl.Equals(response.ResponseUri.ToString()))
                    {
                        isAlive = true;
                    }
                    else
                    {
                        isAlive = false;
                    }
                }
            }
            catch
            {
                isAlive = false;
            }

            return isAlive;
        }

        /// <summary>
        /// Tries to connect to a Photon instance
        /// </summary>
        /// <param name="photonAddress"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsPhotonAlive(string photonAddress, string name)
        {
            System.Diagnostics.Debug.Print("IsPhotonAlive {0}", photonAddress);

            PhotonConnection c = new PhotonConnection(name);
            bool runQuery = true;
            int retries = 3;
            try
            {
                while (runQuery)
                {
                    c.ConnectToServer(photonAddress);
                    while (!c.IsDone) { ; }

                    switch (c.State)
                    {
                        case PhotonConnection.PhotonState.Live:
                            runQuery = false;
                            break;
                        default:
                            retries--;
                            runQuery = retries > 0;

                            if (!runQuery)
                            {
                                CmuneMail.SendEmail("noreply@cmune.com", "Photon Monitor", "thomas@cmune.com", "Photon Dudes", c.Debug, c.Stats, false);
                            }
                            break;
                    }
                }
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.Print("SocketException: {0}", ex.Message);
            }
            finally
            {
                c.Close();
            }

            return c.State == PhotonConnection.PhotonState.Live;
        }

        /// <summary>
        /// Test whether a web service is live or not
        /// </summary>
        /// <param name="url"></param>
        /// <param name="passPhrase"></param>
        /// <param name="initVector"></param>
        /// <param name="httpPostFieldName"></param>
        /// <param name="httpPostContent"></param>
        /// <returns></returns>
        public static bool IsWebServiceAlive(string url, string passPhrase, string initVector, string httpPostFieldName, string httpPostContent)
        {
            bool isAlive = false;

            try
            {
                WebPostRequest myPost = new WebPostRequest(url);
                myPost.Add(httpPostFieldName, httpPostContent);
                string response = myPost.GetResponse();

                string exceptionStringStart = "<";

                if (!response.StartsWith(exceptionStringStart))
                {
                    isAlive = true;
                }
            }
            catch
            {
                isAlive = false;
            }

            return isAlive;
        }

        /// <summary>
        /// Tries to connect to a database and to issue a SELECT statement.
        /// Note: The SELECT should have at least one result for the test to succeed
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery">SELECT statement only</param>
        /// <returns></returns>
        public static bool IsDatabaseAlive(string connectionString, string sqlQuery)
        {
            // TODO Timeout is not working

            bool isAlive = false;

            try
            {
                using (SqlConnection myConnection = new SqlConnection(connectionString))
                {
                    myConnection.Open();
                    // TODO Do we need to protect for SQL injection?
                    SqlCommand myCommand = new SqlCommand(sqlQuery, myConnection);
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        if (myReader.HasRows)
                        {
                            isAlive = true;
                        }
                    }
                }
            }
            catch
            {
                isAlive = false;
            }

            return isAlive;
        }

        /// <summary>
        /// Get the last poll of a specific server
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static LastPoll GetLastPoll(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                LastPoll lastPoll = monitoringDB.LastPolls.SingleOrDefault(lP => lP.ManagedServerID == managedServerID);

                return lastPoll;
            }
        }

        /// <summary>
        /// Get the last polls
        /// </summary>
        /// <returns></returns>
        public static List<LastPoll> GetLastPolls()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<LastPoll> lastPolls = monitoringDB.LastPolls.ToList();

                return lastPolls;
            }
        }

        /// <summary>
        /// Get the last polls for specific servers
        /// </summary>
        /// <param name="monitoredServersIDs"></param>
        /// <returns></returns>
        public static List<LastPoll> GetLastPolls(List<int> monitoredServersIDs)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<LastPoll> lastPolls = monitoringDB.LastPolls.Where(lP => monitoredServersIDs.Contains(lP.ManagedServerID)).ToList();

                return lastPolls;
            }
        }

        /// <summary>
        /// Get the last polls for specific servers
        /// </summary>
        /// <param name="monitoredServers"></param>
        /// <returns></returns>
        public static List<LastPoll> GetLastPolls(List<ManagedServerModel> monitoredServers)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<int> monitoredServersIDs = monitoredServers.Select(mS => mS.ManagedServerId2).ToList();
                List<LastPoll> lastPolls = monitoringDB.LastPolls.Where(lP => monitoredServersIDs.Contains(lP.ManagedServerID)).ToList();

                return lastPolls;
            }
        }

        /// <summary>
        /// Get the servers summary
        /// </summary>
        /// <returns></returns>
        public static List<ServerSummaryView> GetServersSummary()
        {
            List<ServerSummaryView> serverSummary = new List<ServerSummaryView>();

            List<ManagedServerModel> managedServers = GetManagedServers();
            Dictionary<int, ManagedServerModel> managedSerdersOrdered = managedServers.ToDictionary(mS => mS.ManagedServerId2);
            List<ManagedServerModel> monitoredServers = new List<ManagedServerModel>();
            List<ManagedServerModel> disabledServers = new List<ManagedServerModel>();

            SeparateMonitoredAndDisabledServers(managedServers, out monitoredServers, out disabledServers);

            serverSummary.AddRange(disabledServers.ConvertAll<ServerSummaryView>(new Converter<ManagedServerModel, ServerSummaryView>(mS => ConvertToServerSummaryView(DateTime.MinValue, mS))));

            List<LastPoll> lastPolls = GetLastPolls(monitoredServers);

            serverSummary.AddRange(lastPolls.ConvertAll<ServerSummaryView>(new Converter<LastPoll, ServerSummaryView>(lP => ConvertToServerSummaryView(lP, managedSerdersOrdered[lP.ManagedServerID].NextPollTime))));
            Dictionary<int, int> lastPollsServerIDs = lastPolls.ToDictionary(lP => lP.ManagedServerID, lP => lP.ManagedServerID);

            List<ManagedServerModel> newMonitoredServers = new List<ManagedServerModel>();

            foreach (ManagedServerModel monitoredServer in monitoredServers)
            {
                if (!lastPollsServerIDs.ContainsKey(monitoredServer.ManagedServerId2))
                {
                    newMonitoredServers.Add(monitoredServer);
                }
            }

            serverSummary.AddRange(newMonitoredServers.ConvertAll<ServerSummaryView>(new Converter<ManagedServerModel, ServerSummaryView>(mS => ConvertToServerSummaryView(DateTime.MinValue, mS))));

            serverSummary.Sort();

            return serverSummary;
        }

        /// <summary>
        /// Separates a list of managed servers into monitored servers (we're still testing them) and disabled servers (we're not testing them anymore)
        /// </summary>
        /// <param name="managedServers"></param>
        /// <param name="monitoredServers"></param>
        /// <param name="disabledServers"></param>
        public static void SeparateMonitoredAndDisabledServers(List<ManagedServerModel> managedServers, out List<ManagedServerModel> monitoredServers, out List<ManagedServerModel> disabledServers)
        {
            monitoredServers = new List<ManagedServerModel>();
            disabledServers = new List<ManagedServerModel>();

            foreach (ManagedServerModel managedServer in managedServers)
            {
                if (managedServer.IsDisable)
                {
                    disabledServers.Add(managedServer);
                }
                else
                {
                    monitoredServers.Add(managedServer);
                }
            }
        }

        /// <summary>
        /// Separates a list of tests into monitored tests (we're still testing them) and disabled tests (we're not testing them anymore)
        /// </summary>
        /// <param name="tests"></param>
        /// <param name="monitoredTests"></param>
        /// <param name="disabledTests"></param>
        public static void SeparateMonitoredAndDisabledTests(List<ServerTestView> tests, out List<ServerTestView> monitoredTests, out List<ServerTestView> disabledTests)
        {
            monitoredTests = new List<ServerTestView>();
            disabledTests = new List<ServerTestView>();

            foreach (ServerTestView test in tests)
            {
                if (test.Status.Equals(MonitoringTestStatus.Disable))
                {
                    disabledTests.Add(test);
                }
                else
                {
                    monitoredTests.Add(test);
                }
            }
        }

        /// <summary>
        /// Get the last poll history
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static ServerPollView GetLastPollHistory(int managedServerID)
        {
            LastPoll lastPoll = GetLastPoll(managedServerID);
            ServerPollView serverPollView = null;

            if (lastPoll != null)
            {
                serverPollView = GetPollHistory(ConvertToServerSummaryView(lastPoll, DateTime.MinValue), lastPoll.PollHistoryID);
            }
            else
            {
                ManagedServerModel managedServer = GetManagedServer(managedServerID);

                if (managedServer != null)
                {
                    serverPollView = GetPollHistory(ConvertToServerSummaryView(DateTime.MinValue, managedServer), 0);
                }
            }

            return serverPollView;
        }

        /// <summary>
        /// Get a poll history
        /// </summary>
        /// <param name="serverView"></param>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        private static ServerPollView GetPollHistory(ServerSummaryView serverView, int pollHistoryID)
        {
            ServerPollView serverPoll = null;
            List<ServerTestView> serverTests = new List<ServerTestView>();

            List<DatabaseTest> databaseTests = GetDatabaseTests(serverView.ServerId);
            List<PhotonTest> photonsTests = GetPhotonsTests(serverView.ServerId);
            List<WebserviceTest> webserviceTests = GetWebServiceTests(serverView.ServerId);
            List<WebsiteTest> websiteTests = GetWebsiteTests(serverView.ServerId);

            List<DatabaseTestHistory> databaseTestHistory = GetDatabaseTestHistory(pollHistoryID);
            List<PhotonTestHistory> photonTestHistory = GetPhotonTestHistory(pollHistoryID);
            List<WebserviceTestHistory> webserviceTestHistory = GetWebserviceTestHistory(pollHistoryID);
            List<WebsiteTestHistory> websiteTestHistory = GetWebsiteTestHistory(pollHistoryID);

            List<ServerTestView> testsView = databaseTests.ConvertAll<ServerTestView>(new Converter<DatabaseTest, ServerTestView>(ConvertToServerTestView));
            testsView.AddRange(photonsTests.ConvertAll<ServerTestView>(new Converter<PhotonTest, ServerTestView>(ConvertToServerTestView)));
            testsView.AddRange(webserviceTests.ConvertAll<ServerTestView>(new Converter<WebserviceTest, ServerTestView>(ConvertToServerTestView)));
            testsView.AddRange(websiteTests.ConvertAll<ServerTestView>(new Converter<WebsiteTest, ServerTestView>(ConvertToServerTestView)));

            List<ServerTestView> monitoredTests = new List<ServerTestView>();
            List<ServerTestView> disabledTests = new List<ServerTestView>();

            SeparateMonitoredAndDisabledTests(testsView, out monitoredTests, out disabledTests);
            serverTests.AddRange(disabledTests);

            List<ServerTestView> testHistoryView = databaseTestHistory.ConvertAll<ServerTestView>(new Converter<DatabaseTestHistory, ServerTestView>(ConvertToServerTestView));
            testHistoryView.AddRange(photonTestHistory.ConvertAll<ServerTestView>(new Converter<PhotonTestHistory, ServerTestView>(ConvertToServerTestView)));
            testHistoryView.AddRange(webserviceTestHistory.ConvertAll<ServerTestView>(new Converter<WebserviceTestHistory, ServerTestView>(ConvertToServerTestView)));
            testHistoryView.AddRange(websiteTestHistory.ConvertAll<ServerTestView>(new Converter<WebsiteTestHistory, ServerTestView>(ConvertToServerTestView)));

            serverTests.AddRange(testHistoryView);

            Dictionary<string, string> lastTestIDsAndTypes = testHistoryView.ToDictionary(tHV => tHV.GetUniqueKey(), tHV => tHV.GetUniqueKey());

            List<ServerTestView> newMonitoredTests = new List<ServerTestView>();

            foreach (ServerTestView monitoredTest in monitoredTests)
            {
                if (!lastTestIDsAndTypes.ContainsKey(monitoredTest.GetUniqueKey()))
                {
                    newMonitoredTests.Add(monitoredTest);
                }
            }

            serverTests.AddRange(newMonitoredTests);

            serverTests.Sort();
            serverPoll = new ServerPollView(serverView, serverTests);

            return serverPoll;
        }

        /// <summary>
        /// Get a poll history
        /// </summary>
        /// <param name="pollHistoryID"></param>
        /// <returns></returns>
        public static ServerPollView GetPollHistory(int pollHistoryID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                PollHistory pollHistory = monitoringDB.PollHistories.SingleOrDefault(pH => pH.PollHistoryID == pollHistoryID);
                ServerPollView serverPollView = null;

                if (pollHistory != null)
                {
                    serverPollView = GetPollHistory(ConvertToServerSummaryView(pollHistory, DateTime.MinValue), pollHistoryID);
                }

                return serverPollView;
            }
        }

        /// <summary>
        /// Get a lit of poll history
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <param name="range"></param>
        /// <param name="pollHistoryCount"></param>
        /// <returns></returns>
        public static List<PollHistory> GetPollHistory(int managedServerId, int range, int pollHistoryCount)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PollHistory> pollHistory = new List<PollHistory>();
                int pollHistoryToSkipCount = range * pollHistoryCount;

                pollHistory = monitoringDB.PollHistories.Where(pH => pH.ManagedServerID == managedServerId).OrderByDescending(pH => pH.PollHistoryID).Skip(pollHistoryToSkipCount).Take(pollHistoryCount).ToList();

                return pollHistory;
            }
        }

        /// <summary>
        /// Disable a managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        public static void DisableManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);

                if (managedServer != null)
                {
                    if (!managedServer.IsDisable)
                    {
                        managedServer.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a managed server
        /// </summary>
        /// <param name="managedServerID"></param>
        public static void EnableManagedServer(int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerID);

                if (managedServer != null)
                {
                    if (managedServer.IsDisable)
                    {
                        managedServer.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the name is duplicated
        /// </summary>
        /// <param name="serverName"></param>
        /// <returns></returns>
        public static bool IsManagedServerNameDuplicated(string serverName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerName(serverName))
                {
                    serverName = ValidationUtilities.StandardizeManagedServerName(serverName);

                    int duplicatedNames = monitoringDB.ManagedServers.Count(mS => mS.ServerName == serverName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether the name is duplicated except for a specific server
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="managedServerID"></param>
        /// <returns></returns>
        public static bool IsManagedServerNameDuplicated(string serverName, int managedServerID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerName(serverName))
                {
                    serverName = ValidationUtilities.StandardizeManagedServerName(serverName);

                    int duplicatedNames = monitoringDB.ManagedServers.Count(mS => mS.ServerName == serverName && mS.ManagedServerID != managedServerID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Creates a new managed server
        /// </summary>
        /// <param name="managedServerModel"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateManagedServer(ManagedServerModel managedServerModel)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerName(managedServerModel.ServerName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidIPAddress(managedServerModel.ServerIp))
                {
                    ret = MonitoringOperationResult.InvalidIP;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsManagedServerNameDuplicated(managedServerModel.ServerName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    managedServerModel.NextPollTime = DateTime.Now;
                    managedServerModel.ServerName = ValidationUtilities.StandardizeManagedServerName(managedServerModel.ServerName);

                    monitoringDB.ManagedServers.InsertOnSubmit(managedServerModel.ToManagedServer());
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a managed server
        /// </summary>
        /// <param name="managedServerModel"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditManagedServer(ManagedServerModel managedServerModel)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidIPAddress(managedServerModel.ServerIp))
                {
                    ret = MonitoringOperationResult.InvalidIP;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsManagedServerNameDuplicated(managedServerModel.ServerName, managedServerModel.ManagedServerId2))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    ManagedServer managedServer = monitoringDB.ManagedServers.SingleOrDefault(mS => mS.ManagedServerID == managedServerModel.ManagedServerId2);

                    if (managedServer != null)
                    {
                        managedServer.CopyFromManagedServerModel(managedServerModel);
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.ServerNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Disable a database test
        /// </summary>
        /// <param name="testID"></param>
        public static void DisableDatabaseTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                DatabaseTest test = monitoringDB.DatabaseTests.SingleOrDefault(dT => dT.DatabaseTestID == testID);

                if (test != null)
                {
                    if (!test.IsDisable)
                    {
                        test.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a database test
        /// </summary>
        /// <param name="testID"></param>
        public static void EnableDatabaseTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                DatabaseTest test = monitoringDB.DatabaseTests.SingleOrDefault(dT => dT.DatabaseTestID == testID);

                if (test != null)
                {
                    if (test.IsDisable)
                    {
                        test.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Disable a photon test
        /// </summary>
        /// <param name="testID"></param>
        public static void DisablePhotonTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                PhotonTest test = monitoringDB.PhotonTests.SingleOrDefault(pT => pT.PhotonTestID == testID);

                if (test != null)
                {
                    if (!test.IsDisable)
                    {
                        test.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a photon test
        /// </summary>
        /// <param name="testID"></param>
        public static void EnablePhotonTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                PhotonTest test = monitoringDB.PhotonTests.SingleOrDefault(pT => pT.PhotonTestID == testID);

                if (test != null)
                {
                    if (test.IsDisable)
                    {
                        test.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Disable a web service test
        /// </summary>
        /// <param name="testID"></param>
        public static void DisableWebserviceTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                WebserviceTest test = monitoringDB.WebserviceTests.SingleOrDefault(wT => wT.WebserviceTestID == testID);

                if (test != null)
                {
                    if (!test.IsDisable)
                    {
                        test.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a web service test
        /// </summary>
        /// <param name="testID"></param>
        public static void EnableWebServiceTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                WebserviceTest test = monitoringDB.WebserviceTests.SingleOrDefault(wT => wT.WebserviceTestID == testID);

                if (test != null)
                {
                    if (test.IsDisable)
                    {
                        test.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Disable a web site test
        /// </summary>
        /// <param name="testID"></param>
        public static void DisableWebsiteTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                WebsiteTest test = monitoringDB.WebsiteTests.SingleOrDefault(wT => wT.WebsiteTestID == testID);

                if (test != null)
                {
                    if (!test.IsDisable)
                    {
                        test.IsDisable = true;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Enable a web site test
        /// </summary>
        /// <param name="testID"></param>
        public static void EnableWebsiteTest(int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                WebsiteTest test = monitoringDB.WebsiteTests.SingleOrDefault(wT => wT.WebsiteTestID == testID);

                if (test != null)
                {
                    if (test.IsDisable)
                    {
                        test.IsDisable = false;

                        monitoringDB.SubmitChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether a database test name is duplicated
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsDatabaseTestNameDuplicated(string testName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.DatabaseTests.Count(dT => dT.TestName == testName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a database test name is duplicated except for a specific test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testID"></param>
        /// <returns></returns>
        public static bool IsDatabaseTestNameDuplicated(string testName, int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.DatabaseTests.Count(dT => dT.TestName == testName && dT.DatabaseTestID != testID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a photon test name is duplicated
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsPhotonTestNameDuplicated(string testName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.PhotonTests.Count(pT => pT.TestName == testName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a photon test name is duplicated except for a specific test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testID"></param>
        /// <returns></returns>
        public static bool IsPhotonTestNameDuplicated(string testName, int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.PhotonTests.Count(pT => pT.TestName == testName && pT.PhotonTestID != testID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a web service test name is duplicated
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsWebserviceTestNameDuplicated(string testName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.WebserviceTests.Count(wT => wT.TestName == testName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a web service test name is duplicated except for a specific test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testID"></param>
        /// <returns></returns>
        public static bool IsWebserviceTestNameDuplicated(string testName, int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.WebserviceTests.Count(wT => wT.TestName == testName && wT.WebserviceTestID != testID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a web site test name is duplicated
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        public static bool IsWebsiteTestNameDuplicated(string testName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.WebsiteTests.Count(wT => wT.TestName == testName);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Checks whether a web site test name is duplicated except for a specific test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="testID"></param>
        /// <returns></returns>
        public static bool IsWebsiteTestNameDuplicated(string testName, int testID)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                bool isDuplicated = false;

                if (ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    testName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    int duplicatedNames = monitoringDB.WebsiteTests.Count(wT => wT.TestName == testName && wT.WebsiteTestID != testID);

                    if (duplicatedNames > 0)
                    {
                        isDuplicated = true;
                    }
                }

                return isDuplicated;
            }
        }

        /// <summary>
        /// Creates a new database test
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateDatabaseTest(int managedServerID, string testName, string connectionString, string sqlQuery)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && connectionString.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidConnectionString;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && sqlQuery.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidSqlQuery;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    DatabaseTest test = new DatabaseTest();
                    test.ConnectionString = connectionString.Trim();
                    test.IsDisable = false;
                    test.SqlQuery = sqlQuery.Trim();
                    test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);
                    test.ManagedServerID = managedServerID;

                    monitoringDB.DatabaseTests.InsertOnSubmit(test);
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a database test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="connectionString"></param>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditDatabaseTest(int testID, int managedServerID, string testName, string connectionString, string sqlQuery)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName, testID))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && connectionString.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidConnectionString;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && sqlQuery.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidSqlQuery;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    DatabaseTest test = monitoringDB.DatabaseTests.SingleOrDefault(dT => dT.DatabaseTestID == testID);

                    if (test != null)
                    {
                        test.ConnectionString = connectionString.Trim();
                        test.ManagedServerID = managedServerID;
                        test.SqlQuery = sqlQuery.Trim();
                        test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Creates a new photon test
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreatePhotonTest(int managedServerID, string testName, string socket)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidSocket(socket))
                {
                    ret = MonitoringOperationResult.TestInvalidSocket;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    PhotonTest test = new PhotonTest();
                    test.Socket = socket.Trim();
                    test.IsDisable = false;
                    test.ManagedServerID = managedServerID;
                    test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                    monitoringDB.PhotonTests.InsertOnSubmit(test);
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a photon test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditPhotonTest(int testID, int managedServerID, string testName, string socket)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName, testID))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && !ValidationUtilities.IsValidSocket(socket))
                {
                    ret = MonitoringOperationResult.TestInvalidSocket;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    PhotonTest test = monitoringDB.PhotonTests.SingleOrDefault(pT => pT.PhotonTestID == testID);

                    if (test != null)
                    {
                        test.Socket = socket.Trim();
                        test.ManagedServerID = managedServerID;
                        test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);

                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Creates a new web service test
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="url"></param>
        /// <param name="passPhrase"></param>
        /// <param name="initVector"></param>
        /// <param name="postContent"></param>
        /// <param name="postField"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateWebserviceTest(int managedServerID, string testName, string url, string passPhrase, string initVector, string postContent, string postField)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsWebserviceTestNameDuplicated(testName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && url.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && passPhrase.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPassPhrase;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && initVector.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidInitVector;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && postContent.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPostContent;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && postField.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPostField;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebserviceTest test = new WebserviceTest();
                    test.HttpPostContent = postContent.Trim();
                    test.HttpPostFieldName = postField.Trim();
                    test.InitVector = initVector.Trim();
                    test.IsDisable = false;
                    test.ManagedServerID = managedServerID;
                    test.PassPhrase = passPhrase.Trim();
                    test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);
                    test.WebserviceUrl = url.Trim();

                    monitoringDB.WebserviceTests.InsertOnSubmit(test);
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a web service test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="url"></param>
        /// <param name="passPhrase"></param>
        /// <param name="initVector"></param>
        /// <param name="postContent"></param>
        /// <param name="postField"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditWebserviceTest(int testID, int managedServerID, string testName, string url, string passPhrase, string initVector, string postContent, string postField)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName, testID))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && url.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && passPhrase.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPassPhrase;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && initVector.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidInitVector;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && postContent.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPostContent;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && postField.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidPostField;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebserviceTest test = monitoringDB.WebserviceTests.SingleOrDefault(wT => wT.WebserviceTestID == testID);

                    if (test != null)
                    {
                        test.HttpPostContent = postContent.Trim();
                        test.HttpPostFieldName = postField.Trim();
                        test.InitVector = initVector.Trim();
                        test.ManagedServerID = managedServerID;
                        test.PassPhrase = passPhrase.Trim();
                        test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);
                        test.WebserviceUrl = url.Trim();

                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Creates a new web site test
        /// </summary>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="url"></param>
        /// <param name="expectedResponseUrl"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CreateWebsiteTest(int managedServerID, string testName, string url, string expectedResponseUrl)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && url.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && expectedResponseUrl.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebsiteTest test = new WebsiteTest();
                    test.IsDisable = false;
                    test.ManagedServerID = managedServerID;
                    test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);
                    test.ExpectedResponseUrl = expectedResponseUrl.Trim();
                    test.WebsiteUrl = url.Trim();

                    monitoringDB.WebsiteTests.InsertOnSubmit(test);
                    monitoringDB.SubmitChanges();
                }

                return ret;
            }
        }

        /// <summary>
        /// Edits a web site test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="managedServerID"></param>
        /// <param name="testName"></param>
        /// <param name="url"></param>
        /// <param name="expectedResponseUrl"></param>
        /// <returns></returns>
        public static MonitoringOperationResult EditWebsiteTest(int testID, int managedServerID, string testName, string url, string expectedResponseUrl)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(testName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(testName, testID))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && url.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && expectedResponseUrl.IsNullOrFullyEmpty())
                {
                    ret = MonitoringOperationResult.TestInvalidUrl;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebsiteTest test = monitoringDB.WebsiteTests.SingleOrDefault(wT => wT.WebsiteTestID == testID);

                    if (test != null)
                    {
                        test.ManagedServerID = managedServerID;
                        test.TestName = ValidationUtilities.StandardizeManagedServerTestName(testName);
                        test.ExpectedResponseUrl = expectedResponseUrl.Trim();
                        test.WebsiteUrl = url.Trim();

                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Copy a database test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="newManagedServerID"></param>
        /// <param name="newTestName"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CopyDatabaseTest(int testID, int newManagedServerID, string newTestName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(newTestName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(newTestName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    DatabaseTest test = monitoringDB.DatabaseTests.SingleOrDefault(dT => dT.DatabaseTestID == testID);

                    if (test != null)
                    {
                        DatabaseTest newTest = new DatabaseTest();
                        newTest.ConnectionString = test.ConnectionString;
                        newTest.IsDisable = test.IsDisable;
                        newTest.ManagedServerID = newManagedServerID;
                        newTest.SqlQuery = test.SqlQuery;
                        newTest.TestName = ValidationUtilities.StandardizeManagedServerTestName(newTestName);

                        monitoringDB.DatabaseTests.InsertOnSubmit(newTest);
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Copy a photon test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="newManagedServerID"></param>
        /// <param name="newTestName"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CopyPhotonTest(int testID, int newManagedServerID, string newTestName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(newTestName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsPhotonTestNameDuplicated(newTestName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    PhotonTest test = monitoringDB.PhotonTests.SingleOrDefault(pT => pT.PhotonTestID == testID);

                    if (test != null)
                    {
                        PhotonTest newTest = new PhotonTest();
                        newTest.Socket = test.Socket;
                        newTest.IsDisable = test.IsDisable;
                        newTest.ManagedServerID = newManagedServerID;
                        newTest.TestName = ValidationUtilities.StandardizeManagedServerTestName(newTestName);

                        monitoringDB.PhotonTests.InsertOnSubmit(newTest);
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Copy a web service test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="newManagedServerID"></param>
        /// <param name="newTestName"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CopyWebserviceTest(int testID, int newManagedServerID, string newTestName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(newTestName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(newTestName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebserviceTest test = monitoringDB.WebserviceTests.SingleOrDefault(wT => wT.WebserviceTestID == testID);

                    if (test != null)
                    {
                        WebserviceTest newTest = new WebserviceTest();
                        newTest.HttpPostContent = test.HttpPostContent;
                        newTest.HttpPostFieldName = test.HttpPostFieldName;
                        newTest.InitVector = test.InitVector;
                        newTest.IsDisable = test.IsDisable;
                        newTest.ManagedServerID = newManagedServerID;
                        newTest.PassPhrase = test.PassPhrase;
                        newTest.TestName = ValidationUtilities.StandardizeManagedServerTestName(newTestName);
                        newTest.WebserviceUrl = test.WebserviceUrl;

                        monitoringDB.WebserviceTests.InsertOnSubmit(newTest);
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Copy a website test
        /// </summary>
        /// <param name="testID"></param>
        /// <param name="newManagedServerID"></param>
        /// <param name="newTestName"></param>
        /// <returns></returns>
        public static MonitoringOperationResult CopyWebsiteTest(int testID, int newManagedServerID, string newTestName)
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                MonitoringOperationResult ret = MonitoringOperationResult.Ok;

                if (!ValidationUtilities.IsValidManagedServerTestName(newTestName))
                {
                    ret = MonitoringOperationResult.InvalidName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok) && IsDatabaseTestNameDuplicated(newTestName))
                {
                    ret = MonitoringOperationResult.DuplicateName;
                }

                if (ret.Equals(MonitoringOperationResult.Ok))
                {
                    WebsiteTest test = monitoringDB.WebsiteTests.SingleOrDefault(wT => wT.WebsiteTestID == testID);

                    if (test != null)
                    {
                        WebsiteTest newTest = new WebsiteTest();
                        newTest.IsDisable = test.IsDisable;
                        newTest.ManagedServerID = newManagedServerID;
                        newTest.TestName = ValidationUtilities.StandardizeManagedServerTestName(newTestName);
                        newTest.ExpectedResponseUrl = test.ExpectedResponseUrl;
                        newTest.WebsiteUrl = test.WebsiteUrl;

                        monitoringDB.WebsiteTests.InsertOnSubmit(newTest);
                        monitoringDB.SubmitChanges();
                    }
                    else
                    {
                        ret = MonitoringOperationResult.TestNotFound;
                    }
                }

                return ret;
            }
        }

        /// <summary>
        /// Get the last fail poll
        /// </summary>
        /// <returns></returns>
        public static PollHistory GetLastFailPoll()
        {
            using (CmuneMonitoringDataContext monitoringDB = new CmuneMonitoringDataContext())
            {
                List<PollHistory> lastFailPolls = monitoringDB.PollHistories.Where(pH => pH.IsAlive == false).OrderByDescending(pH => pH.PollHistoryID).Take(1).ToList();

                PollHistory lastFailPoll = null;

                if (lastFailPolls.Count > 0)
                {
                    lastFailPoll = lastFailPolls[0];
                }

                return lastFailPoll;
            }
        }

        #region ConvertAll
        // To move to some Utils probably

        /// <summary>
        /// Convert to ServerSummaryView
        /// </summary>
        /// <param name="lastPollTime"></param>
        /// <param name="managedServer"></param>
        /// <returns></returns>
        public static ServerSummaryView ConvertToServerSummaryView(DateTime lastPollTime, ManagedServerModel managedServer)
        {
            ServerSummaryView serverSummary = null;

            if (managedServer != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (managedServer.IsDisable)
                {
                    monitoringStatus = MonitoringTestStatus.Disable;
                }

                serverSummary = new ServerSummaryView(monitoringStatus, managedServer.ManagedServerId2, managedServer.ServerName, lastPollTime, managedServer.NextPollTime);
            }

            return serverSummary;
        }

        /// <summary>
        /// Convert to ServerSummaryView
        /// </summary>
        /// <param name="lastPoll"></param>
        /// <param name="nextPollTime"></param>
        /// <returns></returns>
        public static ServerSummaryView ConvertToServerSummaryView(LastPoll lastPoll, DateTime nextPollTime)
        {
            ServerSummaryView serverSummary = null;

            if (lastPoll != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (lastPoll.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverSummary = new ServerSummaryView(monitoringStatus, lastPoll.ManagedServerID, lastPoll.ServerName, lastPoll.PollTime, nextPollTime);
            }

            return serverSummary;
        }

        /// <summary>
        /// Convert to ServerSummaryView
        /// </summary>
        /// <param name="pollHistory"></param>
        /// <param name="nextPollTime"></param>
        /// <returns></returns>
        public static ServerSummaryView ConvertToServerSummaryView(PollHistory pollHistory, DateTime nextPollTime)
        {
            ServerSummaryView serverSummary = null;

            if (pollHistory != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (pollHistory.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverSummary = new ServerSummaryView(monitoringStatus, pollHistory.ManagedServerID, pollHistory.ServerName, pollHistory.PollTime, nextPollTime);
            }

            return serverSummary;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="databaseTest"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(DatabaseTest databaseTest)
        {
            ServerTestView serverTest = null;

            if (databaseTest != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (databaseTest.IsDisable)
                {
                    monitoringStatus = MonitoringTestStatus.Disable;
                }

                serverTest = new ServerTestView(monitoringStatus, databaseTest.DatabaseTestID, databaseTest.TestName, MonitoringTestType.Database);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="photonTest"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(PhotonTest photonTest)
        {
            ServerTestView serverTest = null;

            if (photonTest != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (photonTest.IsDisable)
                {
                    monitoringStatus = MonitoringTestStatus.Disable;
                }

                serverTest = new ServerTestView(monitoringStatus, photonTest.PhotonTestID, photonTest.TestName, MonitoringTestType.Photon);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="webserviceTest"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(WebserviceTest webserviceTest)
        {
            ServerTestView serverTest = null;

            if (webserviceTest != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (webserviceTest.IsDisable)
                {
                    monitoringStatus = MonitoringTestStatus.Disable;
                }

                serverTest = new ServerTestView(monitoringStatus, webserviceTest.WebserviceTestID, webserviceTest.TestName, MonitoringTestType.WebService);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="websiteTest"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(WebsiteTest websiteTest)
        {
            ServerTestView serverTest = null;

            if (websiteTest != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (websiteTest.IsDisable)
                {
                    monitoringStatus = MonitoringTestStatus.Disable;
                }

                serverTest = new ServerTestView(monitoringStatus, websiteTest.WebsiteTestID, websiteTest.TestName, MonitoringTestType.Website);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="databaseTestHistory"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(DatabaseTestHistory databaseTestHistory)
        {
            ServerTestView serverTest = null;

            if (databaseTestHistory != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (databaseTestHistory.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverTest = new ServerTestView(monitoringStatus, databaseTestHistory.DatabaseTestID, databaseTestHistory.TestName, MonitoringTestType.Database);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="photonTestHistory"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(PhotonTestHistory photonTestHistory)
        {
            ServerTestView serverTest = null;

            if (photonTestHistory != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (photonTestHistory.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverTest = new ServerTestView(monitoringStatus, photonTestHistory.PhotonTestID, photonTestHistory.TestName, MonitoringTestType.Photon);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="webserviceTestHistory"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(WebserviceTestHistory webserviceTestHistory)
        {
            ServerTestView serverTest = null;

            if (webserviceTestHistory != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (webserviceTestHistory.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverTest = new ServerTestView(monitoringStatus, webserviceTestHistory.WebserviceTestID, webserviceTestHistory.TestName, MonitoringTestType.WebService);
            }

            return serverTest;
        }

        /// <summary>
        /// Convert to ServerTestView
        /// </summary>
        /// <param name="websiteTestHistory"></param>
        /// <returns></returns>
        public static ServerTestView ConvertToServerTestView(WebsiteTestHistory websiteTestHistory)
        {
            ServerTestView serverTest = null;

            if (websiteTestHistory != null)
            {
                MonitoringTestStatus monitoringStatus = MonitoringTestStatus.DidNotRun;

                if (websiteTestHistory.IsAlive)
                {
                    monitoringStatus = MonitoringTestStatus.Success;
                }
                else
                {
                    monitoringStatus = MonitoringTestStatus.Failure;
                }

                serverTest = new ServerTestView(monitoringStatus, websiteTestHistory.WebsiteTestID, websiteTestHistory.TestName, MonitoringTestType.Website);
            }

            return serverTest;
        }

        #endregion
    }

    /// <summary>
    /// Test poll
    /// </summary>
    public class TestsPoll
    {
        #region Fields

        private List<PhotonTest> _photonTests;
        private List<WebserviceTest> _webServiceTests;
        private List<WebsiteTest> _websiteTests;
        private List<DatabaseTest> _databaseTests;
        private int _managedServerID;
        private string _managedServerName;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Photon tests
        /// </summary>
        public List<PhotonTest> PhotonTests
        {
            get { return _photonTests; }
            private set { _photonTests = value; }
        }

        /// <summary>
        /// Web Service tests
        /// </summary>
        public List<WebserviceTest> WebServiceTests
        {
            get { return _webServiceTests; }
            private set { _webServiceTests = value; }
        }

        /// <summary>
        /// Website tests
        /// </summary>
        public List<WebsiteTest> WebsiteTests
        {
            get { return _websiteTests; }
            private set { _websiteTests = value; }
        }

        /// <summary>
        /// Database tests
        /// </summary>
        public List<DatabaseTest> DatabaseTests
        {
            get { return _databaseTests; }
            private set { _databaseTests = value; }
        }

        /// <summary>
        /// Managed Server Id
        /// </summary>
        public int ManagedServerID
        {
            get { return _managedServerID; }
            private set { _managedServerID = value; }
        }

        /// <summary>
        /// Managed Server Name
        /// </summary>
        public string ManagedServerName
        {
            get { return _managedServerName; }
            private set { _managedServerName = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Simple constructor
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <param name="managedServerName"></param>
        public TestsPoll(int managedServerId, string managedServerName)
        {
            this.ManagedServerID = managedServerId;
            this.ManagedServerName = managedServerName;
            this.PhotonTests = new List<PhotonTest>();
            this.WebServiceTests = new List<WebserviceTest>();
            this.WebsiteTests = new List<WebsiteTest>();
            this.DatabaseTests = new List<DatabaseTest>();
        }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="managedServerId"></param>
        /// <param name="managedServerName"></param>
        /// <param name="photonTests"></param>
        /// <param name="webServiceTests"></param>
        /// <param name="websiteTests"></param>
        /// <param name="databaseTests"></param>
        public TestsPoll(int managedServerId, string managedServerName, List<PhotonTest> photonTests, List<WebserviceTest> webServiceTests, List<WebsiteTest> websiteTests, List<DatabaseTest> databaseTests)
        {
            this.ManagedServerID = managedServerId;
            this.ManagedServerName = managedServerName;
            this.PhotonTests = new List<PhotonTest>();

            if (photonTests != null)
            {
                this.PhotonTests = photonTests;
            }

            this.WebServiceTests = new List<WebserviceTest>();

            if (webServiceTests != null)
            {
                this.WebServiceTests = new List<WebserviceTest>();
            }

            this.WebsiteTests = new List<WebsiteTest>();

            if (websiteTests != null)
            {
                this.WebsiteTests = websiteTests;
            }

            this.DatabaseTests = new List<DatabaseTest>();

            if (databaseTests != null)
            {
                this.DatabaseTests = databaseTests;
            }
        }

        #endregion Constructors

        /// <summary>
        /// Add a Photon test
        /// </summary>
        /// <param name="photonTest"></param>
        public void AddPhotonTest(PhotonTest photonTest)
        {
            if (photonTest != null)
            {
                this.PhotonTests.Add(photonTest);
            }
        }

        /// <summary>
        /// Add a web service test
        /// </summary>
        /// <param name="webserviceTest"></param>
        public void AddWebserviceTest(WebserviceTest webserviceTest)
        {
            if (webserviceTest != null)
            {
                this.WebServiceTests.Add(webserviceTest);
            }
        }

        /// <summary>
        /// Add a website test
        /// </summary>
        /// <param name="websiteTest"></param>
        public void AddWebsiteTest(WebsiteTest websiteTest)
        {
            if (websiteTest != null)
            {
                this.WebsiteTests.Add(websiteTest);
            }
        }

        /// <summary>
        /// Add a database test
        /// </summary>
        /// <param name="databaseTest"></param>
        public void AddDatabaseTest(DatabaseTest databaseTest)
        {
            if (databaseTest != null)
            {
                this.DatabaseTests.Add(databaseTest);
            }
        }

        /// <summary>
        /// Counts the number of tests in this poll
        /// </summary>
        /// <returns></returns>
        public int CountTests()
        {
            int testsCount = 0;

            testsCount += this.DatabaseTests.Count;
            testsCount += this.PhotonTests.Count;
            testsCount += this.WebServiceTests.Count;
            testsCount += this.WebsiteTests.Count;

            return testsCount;
        }
    }
}