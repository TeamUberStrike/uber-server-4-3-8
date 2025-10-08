using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.DataCenter.Utils;
using Cmune.Instrumentation.Monitoring.Common.Entities;
using Cmune.Instrumentation.Monitoring.Business;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class PhotonsChart
    {
        #region Properties

        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public Dictionary<DateTime, int> ConcurrentUsers { get; private set; }
        public Dictionary<DateTime, PhotonMonitoringDetailView> PhotonHealth { get; private set; }
        public Dictionary<int, string> InstancesName { get; private set; }

        #endregion

        #region Constructors

        public PhotonsChart(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        #endregion

        #region Methods

        protected void InitCcuChart(string versionNumber)
        {
            ConcurrentUsers = AdminCache.LoadConcurrentUsers(versionNumber, FromDate, ToDate);
        }

        public OpenFlashChart.OpenFlashChart DrawCcuChart(string versionNumber)
        {
            InitCcuChart(versionNumber);

            string dotColor = "#F06F30";

            List<OpenFlashChart.LineDotValue> lineValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, int> kvp in ConcurrentUsers)
            {
                lineValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value, string.Format("{0} CCU ({1})", kvp.Value.ToString("N0"), kvp.Key.ToSamplesTimeString()), dotColor));
                if (kvp.Value > max) max = kvp.Value;
            }

            OpenFlashChart.Area areaChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotColor);
            areaChart.Values = lineValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart ccuChart = ChartUtil.CreateDefaultChart("Concurrent users", (double)max);
            ccuChart.Y_Axis.Steps = (int)max / 5;

            ccuChart.AddElement(areaChart);

            // TODO only start of the day have the date, all the other have the time only (also display only some of them)
            ccuChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate).ToSamplesTimeLabelString().ToArray());
            ccuChart.X_Axis.Set3D(20);

            return ccuChart;
        }

        protected void InitPhotonHealthChart(int instanceId, string version)
        {
            PhotonHealth = AdminCache.LoadPhotonHealth(instanceId, FromDate, ToDate);

            InstancesName = AdminCache.LoadInstancesName(version);
        }

        public OpenFlashChart.OpenFlashChart DrawPhotonHealthChart(int instanceId, string version)
        {
            InitPhotonHealthChart(instanceId, version);

            string dotCpuUtilizationColor = "#F06F30";
            string dotRamUtilizationColor = "#F06F30";
            string dotInstanceRamMbColor = "#F06F30";
            string dotPeerCountColor = "#F06F30";

            List<OpenFlashChart.LineDotValue> lineCpuUtilizationValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineRamUtilizationValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineInstanceRamMbValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> linePeerCountValues = new List<OpenFlashChart.LineDotValue>();

            string instanceName = String.Empty;

            if (!InstancesName.TryGetValue(instanceId, out instanceName))
            {
                instanceName = "No name";
            }

            decimal max = 0;

            foreach (KeyValuePair<DateTime, PhotonMonitoringDetailView> kvp in PhotonHealth)
            {
                lineCpuUtilizationValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value.CpuUtilization, string.Format("{0} CPU utilization ({1})", kvp.Value.CpuUtilization.ToString("N0"), kvp.Key.ToSamplesTimeString()), dotCpuUtilizationColor));
                lineRamUtilizationValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value.RamUtilization, string.Format("{0} RAM utilization ({1})", kvp.Value.RamUtilization.ToString("N0"), kvp.Key.ToSamplesTimeString()), dotRamUtilizationColor));
                lineInstanceRamMbValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value.InstanceRamMb, string.Format("{0} Instance RAM ({1})", kvp.Value.InstanceRamMb.ToString("N0"), kvp.Key.ToSamplesTimeString()), dotInstanceRamMbColor));
                linePeerCountValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value.PeerCount, string.Format("{0} Peers ({1})", kvp.Value.PeerCount.ToString("N0"), kvp.Key.ToSamplesTimeString()), dotPeerCountColor));
                
                if (kvp.Value.CpuUtilization > max) max = kvp.Value.CpuUtilization;
                if (kvp.Value.RamUtilization > max) max = kvp.Value.RamUtilization;
                if (kvp.Value.InstanceRamMb > max) max = kvp.Value.InstanceRamMb;
                if (kvp.Value.PeerCount > max) max = kvp.Value.PeerCount;
            }

            OpenFlashChart.Area areaCpuUtilizationChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotCpuUtilizationColor);
            areaCpuUtilizationChart.Values = lineCpuUtilizationValues;

            OpenFlashChart.Area areaRamUtilizationChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotRamUtilizationColor);
            areaRamUtilizationChart.Values = lineRamUtilizationValues;

            OpenFlashChart.Area areaInstanceRamMbChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotInstanceRamMbColor);
            areaInstanceRamMbChart.Values = lineInstanceRamMbValues;

            OpenFlashChart.Area areaPeerCountChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultAreaChart(dotPeerCountColor);
            areaPeerCountChart.Values = linePeerCountValues;

            // Create the Chart

            OpenFlashChart.OpenFlashChart photonHealthChart = ChartUtil.CreateDefaultChart(instanceName, (double)max);
            photonHealthChart.Y_Axis.Steps = (int)max / 5;

            photonHealthChart.AddElement(areaCpuUtilizationChart);
            photonHealthChart.AddElement(areaRamUtilizationChart);
            photonHealthChart.AddElement(areaInstanceRamMbChart);
            photonHealthChart.AddElement(areaPeerCountChart);

            // TODO only start of the day have the date, all the other have the time only (also display only some of them)
            photonHealthChart.X_Axis.SetLabels(PhotonMonitoring.GetPhotonIntervalList(FromDate, ToDate).ToSamplesTimeLabelString().ToArray());
            //photonHealthChart.X_Axis.Steps = 6;
            //photonHealthChart.X_Axis.Set3D(80);

            return photonHealthChart;
        }

        #endregion
    }

    public static class PhotonDateTimeExtensions
    {
        public static string ToSamplesTimeString(this DateTime value)
        {
            return value.ToString("MM-dd HH:mm");
        }

        public static List<string> ToSamplesTimeLabelString(this List<DateTime> values)
        {
            List<string> legends = new List<string>();

            int firstMinute = 0;

            if (values.Count > 0)
            {
                firstMinute = values[0].Minute;
            }

            foreach (DateTime value in values)
            {
                if (value.Minute == firstMinute)
                {
                    legends.Add(value.ToString("HH:mm"));
                }
                else
                {
                    legends.Add(String.Empty);
                }
            }

            return legends;
        }
    }
}