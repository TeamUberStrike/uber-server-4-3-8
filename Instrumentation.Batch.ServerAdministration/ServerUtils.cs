using System;
using System.Collections.Generic;
using System.Text;
using Cmune.DataCenter.Utils;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace Cmune.Instrumentation.Batch.ServerAdministration
{
    public static class ServerUtils
    {
        public const int DaysToKeepMin = 2;
        // 1Go = 1073741824 bytes (2^30 bytes), current limit is 5Go (5368709120 bytes)
        /// <summary>
        /// In bytes
        /// </summary>
        public static long HardDriveAvailableSpaceLowLimit = 5368709120;

        /// <summary>
        /// Archive IIS logs
        /// </summary>
        /// <param name="debug"></param>
        public static void ArchiveLogs(bool debug)
        {
            try
            {
                // Configuration reading

                string logDirectory = ConfigurationUtilities.ReadConfigurationManager("LogDirectory");
                string backupDirectory = ConfigurationUtilities.ReadConfigurationManager("BackupDirectory");
                string sevenZipDirectory = ConfigurationUtilities.ReadConfigurationManager("SevenZipDirectory");
                int daysToKeep = ConfigurationUtilities.ReadConfigurationManagerInt("DaysToKeep");

                if (daysToKeep < DaysToKeepMin)
                {
                    daysToKeep = DaysToKeepMin;
                }

                // We need to get the file name of all the logs we need to backup

                DateTime now = DateTime.Now;
                DateTime backupTime = now.AddDays(-daysToKeep);
                backupTime = new DateTime(backupTime.Year, backupTime.Month, backupTime.Day, 0, 0, 0);

                // TODO -> We should explore each sub directory to archive all the IIS logs

                DirectoryInfo dinfo = new DirectoryInfo(logDirectory);
                FileInfo[] filesToBackup = dinfo.GetFiles("*.log").Where(f => f.LastWriteTime < backupTime).ToArray();
                List<string> filesNameToBackup = filesToBackup.Select(f => f.FullName).ToList();

                // Now we need to 7zip the files that we'll backup

                string archiveName = backupTime.ToString("YYMMdd") + ".7z ";

                foreach (string fileNameToBackup in filesNameToBackup)
                {
                    string archiveCommand = sevenZipDirectory + "7za a -t7z u_ex" + archiveName + fileNameToBackup;
                    Process.Start(archiveCommand);
                }

                // Then we delete the files we just backup (after ensuring that the archive is existing)
                FileInfo[] backupArchive = dinfo.GetFiles(archiveName);

                if (backupArchive.Length == 1)
                {
                    foreach (FileInfo fileToBackup in filesToBackup)
                    {
                        fileToBackup.Delete();
                    }

                    // Then we copy the backup to the backup directory

                    backupArchive[0].MoveTo(backupDirectory);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Alerts if the available space is under a critical level
        /// </summary>
        /// <param name="debug"></param>
        public static void AnalyzeDiskSpace(bool debug)
        {
            try
            {
                string systemDrivename = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
                DriveInfo driveInfo = new DriveInfo(systemDrivename);
                long freeSpace = driveInfo.AvailableFreeSpace;

                if (freeSpace < HardDriveAvailableSpaceLowLimit)
                {
                    string serverName = ConfigurationUtilities.ReadConfigurationManager("ServerName");
                    string serverPublicIp = ConfigurationUtilities.ReadConfigurationManager("ServerPublicIp");

                    double availableGbs = ((double) freeSpace) / Math.Pow(2, 30);

                    CmuneMail.SendEmail(ConfigurationUtilities.ReadConfigurationManager("EmailAlertFrom"),
                                        ConfigurationUtilities.ReadConfigurationManager("EmailAlertFromName"),
                                        ConfigurationUtilities.ReadConfigurationManager("EmailAlertTo"),
                                        ConfigurationUtilities.ReadConfigurationManager("EmailAlertToName"),
                                        String.Format("[LOW DISK SPACE][{0}][{1}]", serverName, serverPublicIp),
                                        String.Format("<p>Please help me, my system disk is having {0} bytes ({1} Gb) available.<ul><li><b>My name</b>: {2}</b><li><b>My public IP</b>: {3}</li></ul></p>", freeSpace.ToString(), availableGbs.ToString("N2"), serverName, serverPublicIp),
                                        String.Format("Please help me, my system disk is having {0} bytes ({1} Gb) available.\n\nMy name: {2}\nMy public IP: {3}", freeSpace.ToString(), availableGbs.ToString("N2"), serverName, serverPublicIp));
                }
            }
            catch (Exception ex)
            {
                CmuneLog.LogException(ex, String.Empty);
            }
        }
    }
}