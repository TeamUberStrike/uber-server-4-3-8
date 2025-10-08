using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cmune.Channels.Instrumentation.Extensions;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public static class ChartUtil
    {
        public static OpenFlashChart.Area CreateDefaultAreaChart(string lineColor)
        {
            OpenFlashChart.Area chartObject = new OpenFlashChart.Area();
            chartObject.Colour = lineColor;
            chartObject.DotStyleType.DotSize = 3;
            return chartObject;
        }

        public static OpenFlashChart.Line CreateDefaultLineChart(string lineColor)
        {
            OpenFlashChart.Line lineChart = new OpenFlashChart.Line();
            lineChart.Colour = lineColor;
            lineChart.Width = 2;
            lineChart.HaloSize = 0;
            lineChart.FillAlpha = 0;
            lineChart.Alpha = 0;
            lineChart.Loop = false;
            lineChart.DotStyleType.DotSize = 3;
            lineChart.FontSize = 12;
            return lineChart;
        }

        private static double ComputeMax(double max)
        {
            max = Math.Ceiling(max);

            if (max < 10)
            {
                max += 1;
            }
            else if (max < 100)
            {
                max *= 1.10;
            }
            else
            {
                max *= 1.03;
            }

            return max;
        }

        public static OpenFlashChart.OpenFlashChart CreateDefaultChart(string title, double max)
        {
            max = ComputeMax(max);

            OpenFlashChart.OpenFlashChart chartObject = new OpenFlashChart.OpenFlashChart();
            chartObject.Title = new OpenFlashChart.Title(title);
            chartObject.Title.Style = "{color:#555555; font-family:Verdana,Arial,Helvetica,sans-serif; font-size:14px; font-weight:bold; font background-color: #FFFFFF}";
            chartObject.Bgcolor = "#FCFBFB";
            chartObject.X_Axis.SetColors("#FFFFFF", "#FCFBFB");
            chartObject.Y_Axis.SetColors("#FFFFFF", "#dbdbdb");
            chartObject.X_Axis.Labels.Color = "#555555";
            chartObject.Y_Axis.Labels.Color = "#555555";
            chartObject.X_Axis.Labels.FontSize = 10;
            chartObject.X_Axis.Labels.Steps = 10;
            chartObject.Y_Axis.Labels.Steps = 10;
            chartObject.Y_Axis.SetRange(0, max);
            //chartObject.X_Axis.Labels.Rotate = "-45";
            chartObject.Tooltip = new OpenFlashChart.ToolTip("#val#");
            chartObject.Tooltip.Shadow = true;
            chartObject.Tooltip.Colour = AdminConfig.ChartToolTipBorderColor;
            chartObject.Tooltip.BackgroundColor = AdminConfig.ChartToolTipColor;
            chartObject.Tooltip.Rounded = 4;
            chartObject.Tooltip.Stroke = 2;
            chartObject.Tooltip.BodyStyle = "color: #FFFFFF; font-size:10px; font-family:Helvetica;";
            chartObject.Tooltip.MouseStyle = OpenFlashChart.ToolTipStyle.CLOSEST;
            return chartObject;
        }

        public static OpenFlashChart.Line CreateMilestoneChart(double max, Dictionary<DateTime, string> applicationMilestones)
        {
            max = ComputeMax(max);

            //Create milestones values
            string milestoneColor = "#FF0000";
            List<OpenFlashChart.LineDotValue> msLineChartValues = new List<OpenFlashChart.LineDotValue>();
            foreach (KeyValuePair<DateTime, string> kvp in applicationMilestones)
            {
                if (string.IsNullOrEmpty(kvp.Value))
                    msLineChartValues.Add(null);
                else
                    msLineChartValues.Add(new OpenFlashChart.LineDotValue(max, kvp.Value + " (" + kvp.Key.ToShortDateString() + ")", milestoneColor));
            }

            OpenFlashChart.Line msLineChart = Cmune.Channels.Instrumentation.Business.Chart.ChartUtil.CreateDefaultLineChart(milestoneColor);
            msLineChart.Values = msLineChartValues;
            msLineChart.Colour = "#FFFFFF";
            msLineChart.Width = 0;
            msLineChart.Alpha = 0;
            return msLineChart;
        }

        public static OpenFlashChart.OpenFlashChart CreateDefaultBarChart(string title, int maxValue, string tooltipBorderColor, string tooltipColor)
        {
            OpenFlashChart.OpenFlashChart chartObject = new OpenFlashChart.OpenFlashChart();
            chartObject.Title = new OpenFlashChart.Title(title);
            chartObject.Title.Style = "{color:#555555; font-family:Verdana,Arial,Helvetica,sans-serif; font-size:14px; font-weight:bold; font background-color: #FFFFFF}";
            chartObject.Bgcolor = "#FCFBFB";
            chartObject.X_Axis.SetColors("#FFFFFF", "#FCFBFB");
            chartObject.Y_Axis.SetColors("#FFFFFF", "#dbdbdb");
            chartObject.X_Axis.Labels.Color = "#555555";
            chartObject.Y_Axis.Labels.Color = "#555555";
            chartObject.X_Axis.Labels.Steps = 10;
            chartObject.Y_Axis.Labels.Steps = maxValue / 10;
            chartObject.Y_Axis.SetRange(0, maxValue);
            chartObject.X_Axis.Labels.FontSize = 10;
            chartObject.Tooltip = new OpenFlashChart.ToolTip("#val#");
            chartObject.Tooltip.Shadow = true;
            chartObject.Tooltip.Colour = tooltipBorderColor;
            chartObject.Tooltip.BackgroundColor = tooltipColor;
            chartObject.Tooltip.Rounded = 4;
            chartObject.Tooltip.Stroke = 2;
            chartObject.Tooltip.BodyStyle = "color: #FFFFFF; font-size:10px; font-family:Helvetica;";
            chartObject.Tooltip.MouseStyle = OpenFlashChart.ToolTipStyle.CLOSEST;
            return chartObject;
        }

        public static double Round(double value, int digits)
        {
            if ((digits < -15) || (digits > 15))
                throw new ArgumentOutOfRangeException("digits", "Rounding digits must be between -15 and 15, inclusive.");

            if (digits >= 0)
                return Math.Round(value, digits);

            double n = Math.Pow(10, -digits);
            return Math.Round(value / n, 0) * n;
        }

        public static IList<string> GetXAxisLabels(DateTime fromDate, DateTime toDate)
        {
            return StatisticsBusiness.GetDaysList(fromDate, toDate).ConvertAll(t => t.ToChartAxisLabel()).ToArray();
        }

        public static IList<string> GetXAxisLabelsHours(DateTime fromDate, DateTime toDate)
        {
            return StatisticsBusiness.GetHoursList(fromDate, toDate).ConvertAll(t => t.ToChartLabelDateHour()).ToArray();
        }
    }
}