using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.Instrumentation.Monitoring.Business;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class ServersMonitoringChart : StatisticChart
    {
        #region Constructors

        public ServersMonitoringChart(DateTime FromDate, DateTime ToDate)
            : base(FromDate, ToDate)
        {
        }

        #endregion

        public OpenFlashChart.OpenFlashChart DrawCpuUsage(int managedServerId)
        {
            List<OpenFlashChart.LineDotValue> dCPUUsageChartValues = new List<OpenFlashChart.LineDotValue>();
            var cpuUsage = ServersMonitoringCache.LoadCPUUsageOverTime(this.FromDate, this.ToDate, managedServerId);

            int max = 100;
            foreach (var value in cpuUsage)
            {
                dCPUUsageChartValues.Add(new OpenFlashChart.LineDotValue(value.Value, string.Format("CPU Usage {0}% {1}", value.Value.ToString("N0"), value.Key), AdminConfig.ChartColorCpuUsage));
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(AdminConfig.ChartColorCpuUsage);
            areaChart.Values = dCPUUsageChartValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart cpuUsageChart = ChartUtil.CreateDefaultChart("CPU Usage", max);
            cpuUsageChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            cpuUsageChart.AddElement(areaChart);
            cpuUsageChart.Y_Axis.Steps = 10;
            cpuUsageChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate, 5).ToSamplesTimeLabelString().ToArray());


            return cpuUsageChart;
        }

        public OpenFlashChart.OpenFlashChart DrawRamUsage(int managedServerId)
        {
            List<OpenFlashChart.LineDotValue> dRamUsageChartValues = new List<OpenFlashChart.LineDotValue>();
            var RamUsage = ServersMonitoringCache.LoadRamUsageOverTime(this.FromDate, this.ToDate, managedServerId);

            int max = 100;
            foreach (var value in RamUsage)
            {
                dRamUsageChartValues.Add(new OpenFlashChart.LineDotValue(value.Value, string.Format("Ram Usage {0}% {1}", value.Value.ToString("N0"), value.Key), AdminConfig.ChartColorRamUsage));
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(AdminConfig.ChartColorRamUsage);
            areaChart.Values = dRamUsageChartValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart RamUsageChart = ChartUtil.CreateDefaultChart("Ram Usage", max);
            RamUsageChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            RamUsageChart.AddElement(areaChart);
            RamUsageChart.Y_Axis.Steps = 10;
            RamUsageChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate, 5).ToSamplesTimeLabelString().ToArray());


            return RamUsageChart;
        }

        public OpenFlashChart.OpenFlashChart DrawBandwidthUsage(int managedServerId)
        {
            List<OpenFlashChart.LineDotValue> BandwidthUsageChartValues = new List<OpenFlashChart.LineDotValue>();
            var BandwidthUsage = ServersMonitoringCache.LoadBandwidthUsageOverTime(this.FromDate, this.ToDate, managedServerId);

            int max = 10000;
            foreach (var value in BandwidthUsage)
            {
                BandwidthUsageChartValues.Add(new OpenFlashChart.LineDotValue(value.Value, string.Format("Bandwidth Usage {0}Bytes/s {1}", value.Value.ToString("N0"), value.Key), AdminConfig.ChartColorBandwidthUsage));
                max = value.Value > max ? value.Value : max;
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(AdminConfig.ChartColorBandwidthUsage);
            areaChart.Values = BandwidthUsageChartValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart BandwidthUsageChart = ChartUtil.CreateDefaultChart("Bandwidth Usage", max);
            BandwidthUsageChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            BandwidthUsageChart.AddElement(areaChart);
            BandwidthUsageChart.Y_Axis.Steps = 1000;
            BandwidthUsageChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate, 5).ToSamplesTimeLabelString().ToArray());


            return BandwidthUsageChart;
        }

        public OpenFlashChart.OpenFlashChart DrawDiskSpaceUsage(int managedServerId)
        {
            List<OpenFlashChart.LineDotValue> DiskSpaceUsageChartValues = new List<OpenFlashChart.LineDotValue>();
            var DiskSpaceUsage = ServersMonitoringCache.LoadDiskSpaceUsageOverTime(this.FromDate, this.ToDate, managedServerId);

            int max = 100;
            foreach (var value in DiskSpaceUsage)
            {
                DiskSpaceUsageChartValues.Add(new OpenFlashChart.LineDotValue(value.Value, string.Format("DiskSpace Usage {0}% {1}", value.Value.ToString("N0"), value.Key), AdminConfig.ChartColorDiskSpaceUsage));
            }

            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(AdminConfig.ChartColorDiskSpaceUsage);
            areaChart.Values = DiskSpaceUsageChartValues;

            // Create the Chart
            OpenFlashChart.OpenFlashChart DiskSpaceUsageChart = ChartUtil.CreateDefaultChart("DiskSpace Usage", max);
            DiskSpaceUsageChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            DiskSpaceUsageChart.AddElement(areaChart);
            DiskSpaceUsageChart.Y_Axis.Steps = 10;
            DiskSpaceUsageChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate, 5).ToSamplesTimeLabelString().ToArray());


            return DiskSpaceUsageChart;
        }
    }
}