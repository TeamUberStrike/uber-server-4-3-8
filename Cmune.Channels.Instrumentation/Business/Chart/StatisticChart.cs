using System;
using System.Collections.Generic;
using Cmune.Channels.Instrumentation.Extensions;
using Cmune.DataCenter.Common.Utils;
using UberStrike.Core.Types;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class StatisticChart : BaseChart
    {
        #region Properties

        public Dictionary<DateTime, string> ApplicationMilestones { get; private set; }

        #endregion

        #region Constructors

        public StatisticChart(DateTime FromDate, DateTime ToDate)
        {
            this.FromDate = FromDate;
            this.ToDate = ToDate;
            ApplicationMilestones = AdminCache.LoadApplicationMilestones(FromDate, ToDate);
        }

        #endregion

        #region Methods

        #region Bundles

        protected OpenFlashChart.OpenFlashChart DrawPackageContributionByRevenueChart(Dictionary<int, decimal> packageContribution, bool areCreditsBundles)
        {
            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            Dictionary<int, decimal> bundlesPrice = AdminCache.LoadBundlesUsdPrice();
            Dictionary<int, string> bundlesName = new Dictionary<int, string>();

            string chartName = "Credits Distribution by Revenue";

            if (!areCreditsBundles)
            {
                chartName = "Bundle Distribution by Revenue";
                bundlesName = AdminCache.LoadBundlesName();
            }

            foreach (KeyValuePair<int, decimal> kvp in packageContribution)
            {
                string bundleTitle = String.Empty;
                decimal bundlePrice;
                string bundlePriceString;

                if (!bundlesPrice.TryGetValue(kvp.Key, out bundlePrice))
                {
                    bundlePriceString = "Deleted bundle";
                }
                else
                {
                    bundlePriceString = bundlePrice.ToString("N0");
                }

                string bundleName = String.Empty;

                if (!bundlesName.TryGetValue(kvp.Key, out bundleName))
                {
                    bundleName = "Deleted bundle";
                }

                if (areCreditsBundles)
                {
                    bundleTitle = String.Format("US${0}", bundlePriceString);
                }
                else
                {
                    bundleTitle = String.Format("US${0} {1}", bundlePriceString, bundleName);
                }

                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, bundleTitle));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "US$#val# of US$#total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title(chartName);
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        protected OpenFlashChart.OpenFlashChart DrawPackageContributionByVolumeChart(Dictionary<int, int> packageContributionByAmount, bool areCreditsBundles)
        {
            OpenFlashChart.Pie pie = new OpenFlashChart.Pie();
            List<OpenFlashChart.PieValue> pieValues = new List<OpenFlashChart.PieValue>();

            Dictionary<int, decimal> bundlesPrice = AdminCache.LoadBundlesUsdPrice();
            Dictionary<int, string> bundlesName = new Dictionary<int, string>();

            string chartName = "Credits Distribution by Volume";

            if (!areCreditsBundles)
            {
                chartName = "Bundle Distribution by Volume";
                bundlesName = AdminCache.LoadBundlesName();
            }

            foreach (KeyValuePair<int, int> kvp in packageContributionByAmount)
            {
                string bundleTitle = String.Empty;
                decimal bundlePrice;
                string bundlePriceString;

                if (!bundlesPrice.TryGetValue(kvp.Key, out bundlePrice))
                {
                    bundlePriceString = "Deleted bundle";
                }
                else
                {
                    bundlePriceString = bundlePrice.ToString("N0");
                }

                string bundleName = String.Empty;

                if (!bundlesName.TryGetValue(kvp.Key, out bundleName))
                {
                    bundleName = "Deleted bundle";
                }

                if (areCreditsBundles)
                {
                    bundleTitle = String.Format("US${0}", bundlePriceString);
                }
                else
                {
                    bundleTitle = String.Format("US${0} {1}", bundlePriceString, bundleName);
                }

                pieValues.Add(new OpenFlashChart.PieValue((double)kvp.Value, bundleTitle));
            }

            pie.Values = pieValues;
            pie.FontSize = 15;
            pie.Alpha = .5;
            pie.Colours = new string[] { "#F99506", "#7bc90b", "#0000ee", "#ee0000" };
            pie.Tooltip = "#val# of #total# (#percent#)";
            pie.GradientFillMode = true;

            OpenFlashChart.PieAnimationSeries pieAnimationSeries = new OpenFlashChart.PieAnimationSeries();
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation(OpenFlashChart.AnimationType.Fadein, null));
            pieAnimationSeries.Add(new OpenFlashChart.PieAnimation("bounce", 3));
            pie.Animate = pieAnimationSeries;

            OpenFlashChart.OpenFlashChart objectChart = new OpenFlashChart.OpenFlashChart();
            objectChart.Title = new OpenFlashChart.Title(chartName);
            objectChart.Title.Style = "{color:#000000; font-size:14px;}";
            objectChart.AddElement(pie);
            objectChart.Bgcolor = "#FFFFFF";

            return objectChart;
        }

        public OpenFlashChart.OpenFlashChart DrawBundlesSalesChart(Dictionary<DateTime, Dictionary<int, int>> bundleSales, bool areCreditsBundles)
        {
            int max = 0;
            string chartName = "Bundles Sales";

            if (areCreditsBundles)
            {
                chartName = "Credits Sales";
            }

            Dictionary<int, string> bundleNames = AdminCache.LoadBundlesName();
            Dictionary<int, decimal> bundlesUsdPrice = AdminCache.LoadBundlesUsdPrice();

            Dictionary<int, int> bundlesList = new Dictionary<int, int>();

            foreach (DateTime date in bundleSales.Keys)
            {
                foreach (int bundleId in bundleSales[date].Keys)
                {
                    if (!bundlesList.ContainsKey(bundleId))
                    {
                        bundlesList.Add(bundleId, bundleId);
                    }
                }
            }

            Dictionary<int, OpenFlashChart.Area> charts = new Dictionary<int, OpenFlashChart.Area>();

            foreach (int bundleId in bundlesList.Keys)
            {
                charts.Add(bundleId, ChartUtil.CreateDefaultAreaChart(GenerateBundleColor(bundleId)));
            }

            Dictionary<int, List<OpenFlashChart.LineDotValue>> values = new Dictionary<int, List<OpenFlashChart.LineDotValue>>();

            foreach (int bundleId in bundlesList.Keys)
            {
                values.Add(bundleId, new List<OpenFlashChart.LineDotValue>());
            }

            foreach (DateTime day in StatisticsBusiness.GetDaysList(FromDate, ToDate))
            {
                foreach (int bundleId in bundlesList.Keys)
                {
                    int count = 0;

                    if (bundleSales[day].ContainsKey(bundleId))
                    {
                        count = bundleSales[day][bundleId];
                    }

                    if (count > max)
                    {
                        max = count;
                    }

                    decimal bundlePrice;
                    string bundlePriceString;

                    if (!bundlesUsdPrice.TryGetValue(bundleId, out bundlePrice))
                    {
                        bundlePriceString = "Deleted bundle";
                    }
                    else
                    {
                        bundlePriceString = bundlePrice.ToString("C2");
                    }

                    string bundleName = String.Empty;

                    if (!bundleNames.TryGetValue(bundleId, out bundleName))
                    {
                        bundleName = "Deleted bundle";
                    }
                    else
                    {
                        bundleName = String.Format("{0} (US{1})", bundleName, bundlePriceString);
                    }

                    if (areCreditsBundles)
                    {
                        bundleName = String.Format("US{0}", bundlePriceString);
                    }

                    // TODO we need to generate different colors according to the bundleId
                    values[bundleId].Add(new OpenFlashChart.LineDotValue((double)count, String.Format("{0}: {1} Units ({2})", bundleName, count, day.ToChartTooltip()), GenerateBundleColor(bundleId)));
                }
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart(chartName, max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            foreach (int bundleId in bundlesList.Keys)
            {
                charts[bundleId].Values = values[bundleId];
                chartObject.AddElement(charts[bundleId]);
            }

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = max / 5;

            return chartObject;
        }

        private string GenerateBundleColor(int bundleId)
        {
            string color = "#000";

            //Dictionary<int, string> colors = new Dictionary<int, string> { { 1, "#FFFF00" }, { 2, "#F73BDF" }, { 6, "#59A76A" }, { 11, "#171BF1" }, { 12, "#004EC3" }, { 13, "#BCBAB6" }, { 16, "#FF0000" } };

            //if (!colors.TryGetValue(bundleId, out color))
            //{
            //    color = "#000";
            //}

            return color;
        }

        #endregion

        #region Unity conversion funnel

        public OpenFlashChart.OpenFlashChart DrawInstallFlowChart(Dictionary<DateTime, int> trackingInitialState, Dictionary<DateTime, int> trackingClickDownload, Dictionary<DateTime, int> trackingUnityInstalled, Dictionary<DateTime, int> trackingUnityInitialized, Dictionary<DateTime, int> trackingFullGameLoaded, Dictionary<DateTime, int> trackingAccountCreated, Dictionary<DateTime, int> trackingClickCancel, bool hasUnity)
        {
            int max = 0;

            string initialStateColor = "#ff0000";
            string initialStateName = "No Unity";
            string chartName = "Installation Conversion Funnel No Unity";

            if (hasUnity)
            {
                initialStateColor = "#3DFAF8";
                initialStateName = "Has Unity";
                chartName = "Installation Conversion Funnel has Unity";
            }

            OpenFlashChart.Area initialStateChart = ChartUtil.CreateDefaultAreaChart(initialStateColor);
            List<OpenFlashChart.LineDotValue> initialStateValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area clickDownloadChart = ChartUtil.CreateDefaultAreaChart("#000099");
            List<OpenFlashChart.LineDotValue> clickDownloadValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area unityInstalledChart = ChartUtil.CreateDefaultAreaChart("#ff9900");
            List<OpenFlashChart.LineDotValue> unityInstalledValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area unityInitializedChart = ChartUtil.CreateDefaultAreaChart("#ff6c00");
            List<OpenFlashChart.LineDotValue> unityInitializedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area fullGameLoadedChart = ChartUtil.CreateDefaultAreaChart("#009900");
            List<OpenFlashChart.LineDotValue> fullGameLoadedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area accountCreatedChart = ChartUtil.CreateDefaultAreaChart("#0078A4");
            List<OpenFlashChart.LineDotValue> accountCreatedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area clickCancelChart = ChartUtil.CreateDefaultAreaChart("#000000");
            List<OpenFlashChart.LineDotValue> clickCancelValues = new List<OpenFlashChart.LineDotValue>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(FromDate, ToDate))
            {
                int noUnityToday, clickDownloadToday, unityInstalledToday, unityInitializedToday, fullGameLoadedToday, accountCreatedToday, clickCancelToday;

                trackingInitialState.TryGetValue(day, out noUnityToday);

                if (noUnityToday > max)
                {
                    max = noUnityToday;
                }

                trackingClickDownload.TryGetValue(day, out clickDownloadToday);
                trackingUnityInstalled.TryGetValue(day, out unityInstalledToday);
                trackingUnityInitialized.TryGetValue(day, out unityInitializedToday);
                trackingFullGameLoaded.TryGetValue(day, out fullGameLoadedToday);
                trackingAccountCreated.TryGetValue(day, out accountCreatedToday);
                trackingClickCancel.TryGetValue(day, out clickCancelToday);

                initialStateValues.Add(new OpenFlashChart.LineDotValue(noUnityToday, String.Format("{0}: {1} ({2})", initialStateName, noUnityToday, day.ToChartTooltip()), initialStateColor));
                clickDownloadValues.Add(new OpenFlashChart.LineDotValue(clickDownloadToday, String.Format("Clicked Download {0} ({1})", clickDownloadToday, day.ToChartTooltip()), "#000099"));
                unityInstalledValues.Add(new OpenFlashChart.LineDotValue(unityInstalledToday, String.Format("Installed Unity {0} ({1})", unityInstalledToday, day.ToChartTooltip()), "#ff9900"));
                unityInitializedValues.Add(new OpenFlashChart.LineDotValue(unityInitializedToday, String.Format("Unity Initialized {0} ({1})", unityInitializedToday, day.ToChartTooltip()), "#ff6c00"));
                fullGameLoadedValues.Add(new OpenFlashChart.LineDotValue(fullGameLoadedToday, String.Format("Full Game Loaded {0} ({1})", fullGameLoadedToday, day.ToChartTooltip()), "#009900"));
                accountCreatedValues.Add(new OpenFlashChart.LineDotValue(accountCreatedToday, String.Format("Account Created {0} ({1})", accountCreatedToday, day.ToChartTooltip()), "#0078A4"));
                clickCancelValues.Add(new OpenFlashChart.LineDotValue(clickCancelToday, String.Format("Clicked Cancel {0} ({1})", clickCancelToday, day.ToChartTooltip()), "#000000"));
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart(chartName, max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            initialStateChart.Values = initialStateValues;
            clickDownloadChart.Values = clickDownloadValues;
            unityInstalledChart.Values = unityInstalledValues;
            unityInitializedChart.Values = unityInitializedValues;
            fullGameLoadedChart.Values = fullGameLoadedValues;
            accountCreatedChart.Values = accountCreatedValues;
            clickCancelChart.Values = clickCancelValues;

            chartObject.AddElement(initialStateChart);
            chartObject.AddElement(clickDownloadChart);
            chartObject.AddElement(unityInstalledChart);
            chartObject.AddElement(unityInitializedChart);
            chartObject.AddElement(fullGameLoadedChart);
            chartObject.AddElement(accountCreatedChart);
            chartObject.AddElement(clickCancelChart);

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = max / 5;

            return chartObject;
        }

        public OpenFlashChart.OpenFlashChart DrawInstallFlowLineChart(Dictionary<DateTime, int> trackingInitialState, Dictionary<DateTime, int> trackingClickDownload, Dictionary<DateTime, int> trackingUnityInstalled, Dictionary<DateTime, int> trackingUnityInitialized, Dictionary<DateTime, int> trackingFullGameLoaded, Dictionary<DateTime, int> trackingAccountCreated, Dictionary<DateTime, int> trackingClickCancel, bool hasUnity)
        {
            string initialStateColor = "#ff0000";
            string initialStateName = "No Unity";
            string chartName = "Installation Conversion Funnel No Unity - Step Ratio";

            if (hasUnity)
            {
                initialStateColor = "#3DFAF8";
                initialStateName = "Has Unity";
                chartName = "Installation Conversion Funnel has Unity - Step Ratio";
            }

            OpenFlashChart.Area initialStateChart = ChartUtil.CreateDefaultAreaChart(initialStateColor);
            List<OpenFlashChart.LineDotValue> initialStateValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area clickDownloadChart = ChartUtil.CreateDefaultAreaChart("#000099");
            List<OpenFlashChart.LineDotValue> clickDownloadValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area unityInstalledChart = ChartUtil.CreateDefaultAreaChart("#ff9900");
            List<OpenFlashChart.LineDotValue> unityInstalledValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area unityInitializedChart = ChartUtil.CreateDefaultAreaChart("#ff6c00");
            List<OpenFlashChart.LineDotValue> unityInitializedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area fullGameLoadedChart = ChartUtil.CreateDefaultAreaChart("#009900");
            List<OpenFlashChart.LineDotValue> fullGameLoadedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area accountCreatedChart = ChartUtil.CreateDefaultAreaChart("#0078A4");
            List<OpenFlashChart.LineDotValue> accountCreatedValues = new List<OpenFlashChart.LineDotValue>();
            OpenFlashChart.Area clickCancelChart = ChartUtil.CreateDefaultAreaChart("#000000");
            List<OpenFlashChart.LineDotValue> clickCancelValues = new List<OpenFlashChart.LineDotValue>();

            foreach (DateTime day in StatisticsBusiness.GetDaysList(FromDate, ToDate))
            {
                int initialStateToday, clickDownloadToday, unityInstalledToday, unityInitializedToday, fullGameLoadedToday, accountCreatedToday, clickCancelToday;
                float clickDownloadRatio = 0, unityInstalledRatio = 0, unityInitializedRatio = 0, fullGameLoadedRatio = 0, accountCreatedRatio = 0, clickCancelRatio = 0;

                trackingInitialState.TryGetValue(day, out initialStateToday);
                trackingClickDownload.TryGetValue(day, out clickDownloadToday);
                trackingUnityInstalled.TryGetValue(day, out unityInstalledToday);
                trackingUnityInitialized.TryGetValue(day, out unityInitializedToday);
                trackingFullGameLoaded.TryGetValue(day, out fullGameLoadedToday);
                trackingAccountCreated.TryGetValue(day, out accountCreatedToday);
                trackingClickCancel.TryGetValue(day, out clickCancelToday);

                // Find ratios
                if (initialStateToday > 0)
                {
                    clickDownloadRatio = ((float)clickDownloadToday / (float)initialStateToday) * 100;
                    unityInstalledRatio = ((float)unityInstalledToday / (float)initialStateToday) * 100;
                    unityInitializedRatio = ((float)unityInitializedToday / (float)initialStateToday) * 100;
                    fullGameLoadedRatio = ((float)fullGameLoadedToday / (float)initialStateToday) * 100;
                    accountCreatedRatio = ((float)accountCreatedToday / (float)initialStateToday) * 100;
                    clickCancelRatio = ((float)clickCancelToday / (float)initialStateToday) * 100;
                }

                initialStateValues.Add(new OpenFlashChart.LineDotValue(100, String.Format("{0}% {1} ({2}) - {3}", 100, initialStateName, initialStateToday, day.ToChartTooltip()), initialStateColor));
                clickDownloadValues.Add(new OpenFlashChart.LineDotValue(clickDownloadRatio, String.Format("{0}% Clicked Download ({1}) - {2}", clickDownloadRatio.ToString("F2"), clickDownloadToday, day.ToChartTooltip()), "#000099"));
                unityInstalledValues.Add(new OpenFlashChart.LineDotValue(unityInstalledRatio, String.Format("{0}% Installed Unity ({1}) - {2}", unityInstalledRatio.ToString("F2"), unityInstalledToday, day.ToChartTooltip()), "#ff9900"));
                unityInitializedValues.Add(new OpenFlashChart.LineDotValue(unityInitializedRatio, String.Format("{0}% Unity Initialized ({1}) - {2}", unityInitializedRatio.ToString("F2"), unityInitializedToday, day.ToChartTooltip()), "#ff6c00"));
                fullGameLoadedValues.Add(new OpenFlashChart.LineDotValue(fullGameLoadedRatio, String.Format("{0}% Full Game Loaded ({1}) - {2}", fullGameLoadedRatio.ToString("F2"), fullGameLoadedToday, day.ToChartTooltip()), "#009900"));
                accountCreatedValues.Add(new OpenFlashChart.LineDotValue(accountCreatedRatio, String.Format("{0}% Account Created ({1}) - {2}", accountCreatedRatio.ToString("F2"), accountCreatedToday, day.ToChartTooltip()), "#0078A4"));
                clickCancelValues.Add(new OpenFlashChart.LineDotValue(clickCancelRatio, String.Format("{0}% Clicked Cancel ({1}) - {2}", clickCancelRatio.ToString("F2"), clickCancelToday, day.ToChartTooltip()), "#000000"));
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart(chartName, 100);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(100, ApplicationMilestones));

            initialStateChart.Values = initialStateValues;
            clickDownloadChart.Values = clickDownloadValues;
            unityInstalledChart.Values = unityInstalledValues;
            unityInitializedChart.Values = unityInitializedValues;
            fullGameLoadedChart.Values = fullGameLoadedValues;
            accountCreatedChart.Values = accountCreatedValues;
            clickCancelChart.Values = clickCancelValues;

            chartObject.AddElement(initialStateChart);
            chartObject.AddElement(clickDownloadChart);
            chartObject.AddElement(unityInstalledChart);
            chartObject.AddElement(unityInitializedChart);
            chartObject.AddElement(fullGameLoadedChart);
            chartObject.AddElement(accountCreatedChart);
            chartObject.AddElement(clickCancelChart);

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = 10;

            return chartObject;
        }

        #endregion

        #region Tutorial

        static string GetTutorialStepTypeColor(TutorialStepType stepId)
        {
            string color = "#ff0000";

            switch (stepId)
            {
                case TutorialStepType.TutorialStart:
                    color = "#FF0000";
                    break;
                case TutorialStepType.KeyboardMove:
                    color = "#6066FF";
                    break;
                case TutorialStepType.MouseLook:
                    color = "#60C2FF";
                    break;
                case TutorialStepType.NameSelection:
                    color = "#FFE960";
                    break;
                case TutorialStepType.PickUpWeapon:
                    color = "#FFB560";
                    break;
                case TutorialStepType.ShootFirstGroup:
                    color = "#FF6B60";
                    break;
                case TutorialStepType.ShootSecondGroup:
                    color = "#FF60B5";
                    break;
                case TutorialStepType.TutorialComplete:
                    color = "#000000";
                    break;
                case TutorialStepType.WalkToArmory:
                    color = "#A060FF";
                    break;
                default:
                    throw new NotImplementedException(String.Format("This stepId ({0}) is not supported yet.", stepId));
            }

            return color;
        }

        public OpenFlashChart.OpenFlashChart DrawTutorialChart(Dictionary<DateTime, Dictionary<TutorialStepType, int>> tutorialSteps)
        {
            int max = 0;

            string chartName = "Tutorial";
            List<TutorialStepType> stepsIds = EnumUtilities.IterateEnum<TutorialStepType>();

            Dictionary<TutorialStepType, OpenFlashChart.Area> charts = new Dictionary<TutorialStepType, OpenFlashChart.Area>();
            Dictionary<TutorialStepType, List<OpenFlashChart.LineDotValue>> values = new Dictionary<TutorialStepType, List<OpenFlashChart.LineDotValue>>();

            foreach (TutorialStepType stepId in stepsIds)
            {
                charts.Add(stepId, ChartUtil.CreateDefaultAreaChart(GetTutorialStepTypeColor(stepId)));
                values.Add(stepId, new List<OpenFlashChart.LineDotValue>());
            }

            foreach (DateTime day in StatisticsBusiness.GetDaysList(FromDate, ToDate))
            {
                if (tutorialSteps[day][TutorialStepType.TutorialStart] > max)
                {
                    max = tutorialSteps[day][TutorialStepType.TutorialStart];
                }

                foreach (TutorialStepType stepId in stepsIds)
                {
                    values[stepId].Add(new OpenFlashChart.LineDotValue(tutorialSteps[day][stepId], String.Format("{0}: {1} ({2})", stepId, tutorialSteps[day][stepId], day.ToChartTooltip()), GetTutorialStepTypeColor(stepId)));
                }
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart(chartName, max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            foreach (TutorialStepType stepId in stepsIds)
            {
                charts[stepId].Values = values[stepId];
                chartObject.AddElement(charts[stepId]);
            }

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = max / 5;

            return chartObject;
        }

        public OpenFlashChart.OpenFlashChart DrawTutorialLineChart(Dictionary<DateTime, Dictionary<TutorialStepType, int>> tutorialSteps)
        {
            string chartName = "Tutorial - Step Ratio";
            List<TutorialStepType> stepsIds = EnumUtilities.IterateEnum<TutorialStepType>();

            Dictionary<TutorialStepType, OpenFlashChart.Area> charts = new Dictionary<TutorialStepType, OpenFlashChart.Area>();
            Dictionary<TutorialStepType, List<OpenFlashChart.LineDotValue>> values = new Dictionary<TutorialStepType, List<OpenFlashChart.LineDotValue>>();

            foreach (TutorialStepType stepId in stepsIds)
            {
                charts.Add(stepId, ChartUtil.CreateDefaultAreaChart(GetTutorialStepTypeColor(stepId)));
                values.Add(stepId, new List<OpenFlashChart.LineDotValue>());
            }

            float min = 100;

            foreach (DateTime day in StatisticsBusiness.GetDaysList(FromDate, ToDate))
            {
                foreach (TutorialStepType stepId in stepsIds)
                {
                    float ratio = 0;

                    if (tutorialSteps[day][TutorialStepType.TutorialStart] != 0)
                    {
                        ratio = ((float)tutorialSteps[day][stepId] / (float)tutorialSteps[day][TutorialStepType.TutorialStart]) * 100;
                    }

                    if (ratio < min)
                    {
                        min = ratio;
                    }

                    values[stepId].Add(new OpenFlashChart.LineDotValue(ratio, String.Format("{0}% {1} ({2}) - {3}", ratio.ToString("F2"), stepId, tutorialSteps[day][stepId], day.ToChartTooltip()), GetTutorialStepTypeColor(stepId)));
                }
            }

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart(chartName, 100);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(100, ApplicationMilestones));

            foreach (TutorialStepType stepId in stepsIds)
            {
                charts[stepId].Values = values[stepId];
                chartObject.AddElement(charts[stepId]);
            }

            min = (int) min - ((int)min % 5);

            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = 5;
            chartObject.Y_Axis.Min = min;

            return chartObject;
        }

        #endregion

        protected OpenFlashChart.OpenFlashChart DrawNewVersusReturningChart(Dictionary<DateTime, IntPair> newVersusReturning)
        {
            string newMembersDotColor = "#0078a4";
            string returningMembersdotColor = "#ffbd16";

            OpenFlashChart.Area newMembersLineChart = ChartUtil.CreateDefaultAreaChart(newMembersDotColor);
            OpenFlashChart.Area returningMembersLineChart2 = ChartUtil.CreateDefaultAreaChart(returningMembersdotColor);
            List<OpenFlashChart.LineDotValue> newMembersLineChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> returningMembersLineChartValues2 = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, IntPair> kvp in newVersusReturning)
            {
                float total = (float)kvp.Value.Int1 + (float)kvp.Value.Int2;
                float newPercent = (total > 0) ? ((float)kvp.Value.Int1 / total) : 0.0f;
                float returningPercent = (total > 0) ? ((float)kvp.Value.Int2 / total) : 0.0f;

                newMembersLineChartValues.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int1, string.Format("{0} New Users ({1}) {2}", kvp.Value.Int1.ToString("N0"), newPercent.ToString("P2"), kvp.Key.ToChartTooltip()), newMembersDotColor));
                returningMembersLineChartValues2.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int2, string.Format("{0} Returning Users ({1}) {2}", kvp.Value.Int2.ToString("N0"), returningPercent.ToString("P2"), kvp.Key.ToChartTooltip()), returningMembersdotColor));

                if (kvp.Value.Int1 > max)
                {
                    max = kvp.Value.Int1;
                }

                if (kvp.Value.Int2 > max)
                {
                    max = kvp.Value.Int2;
                }
            }

            newMembersLineChart.Values = newMembersLineChartValues;
            returningMembersLineChart2.Values = returningMembersLineChartValues2;

            OpenFlashChart.OpenFlashChart nvsrChart = ChartUtil.CreateDefaultChart("New vs Returning Users", max);
            nvsrChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            nvsrChart.AddElement(returningMembersLineChart2);
            nvsrChart.AddElement(newMembersLineChart);
            nvsrChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));

            return nvsrChart;
        }

        public OpenFlashChart.OpenFlashChart DrawDailyTransactionsChart(Dictionary<DateTime, IntPair> dailyTransactions)
        {
            string dotColorUnique = "#0078a4";
            string dotColorTotal = "#ffbd16";

            OpenFlashChart.Area dailyConvLineChartUnique = ChartUtil.CreateDefaultAreaChart(dotColorUnique);
            OpenFlashChart.Area dailyConvLineChartTotal = ChartUtil.CreateDefaultAreaChart(dotColorTotal);

            List<OpenFlashChart.LineDotValue> dailyConvLineChartValuesUnique = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> dailyConvLineChartValuesTotal = new List<OpenFlashChart.LineDotValue>();

            int max = 0;

            foreach (KeyValuePair<DateTime, IntPair> kvp in dailyTransactions)
            {
                dailyConvLineChartValuesUnique.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int1, string.Format("{0} Unique ({1})", kvp.Value.Int1.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorUnique));
                dailyConvLineChartValuesTotal.Add(new OpenFlashChart.LineDotValue(kvp.Value.Int2, string.Format("{0} Total ({1})", kvp.Value.Int2.ToString("N0"), kvp.Key.ToChartTooltip()), dotColorTotal));
                if (kvp.Value.Int1 > max) max = kvp.Value.Int1;
                if (kvp.Value.Int2 > max) max = kvp.Value.Int2;
            }

            dailyConvLineChartUnique.Values = dailyConvLineChartValuesUnique;
            dailyConvLineChartTotal.Values = dailyConvLineChartValuesTotal;

            OpenFlashChart.OpenFlashChart dailyRevChart = ChartUtil.CreateDefaultChart("Daily Transactions", max);
            dailyRevChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));

            dailyRevChart.AddElement(dailyConvLineChartTotal);
            dailyRevChart.AddElement(dailyConvLineChartUnique);

            dailyRevChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            dailyRevChart.Y_Axis.Steps = max / 10;

            return dailyRevChart;
        }

        public OpenFlashChart.OpenFlashChart DrawDailyPlayRateChart(Dictionary<DateTime, float> dailyPlayRate)
        {
            string dotColor = "#f16406";
            List<OpenFlashChart.LineDotValue> drLineChartValues = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            foreach (KeyValuePair<DateTime, float> kvp in dailyPlayRate)
            {
                float playRate = kvp.Value * 100;
                drLineChartValues.Add(new OpenFlashChart.LineDotValue(playRate, string.Format("{0} ({1})", kvp.Value.ToString("P2"), kvp.Key.ToChartTooltip()), dotColor));
                if (playRate > max) max = playRate;
            }

            OpenFlashChart.Area drLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            drLineChart.Values = drLineChartValues;

            OpenFlashChart.OpenFlashChart drChart = ChartUtil.CreateDefaultChart("Play Rate", max);
            drChart.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            drChart.AddElement(drLineChart);
            drChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            drChart.Y_Axis.Steps = 5;
            return drChart;
        }

        public OpenFlashChart.OpenFlashChart DrawDailyConversionToPayingChart(Dictionary<DateTime, FloatPair> dailyConversionToPaying)
        {
            string dotColor = "#7bc90b";
            string dotColor2 = "#ffbd16";
            OpenFlashChart.Area areaChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            OpenFlashChart.Area areaChart2 = ChartUtil.CreateDefaultAreaChart(dotColor2);
            List<OpenFlashChart.LineDotValue> lineChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> lineChartValues2 = new List<OpenFlashChart.LineDotValue>();

            float max = 0;

            foreach (KeyValuePair<DateTime, FloatPair> kvp in dailyConversionToPaying)
            {
                lineChartValues.Add(new OpenFlashChart.LineDotValue(kvp.Value.Float1 * 1000, string.Format("{0} Unique ({1})", kvp.Value.Float1.ToString("P3"), kvp.Key.ToChartTooltip()), dotColor));
                lineChartValues2.Add(new OpenFlashChart.LineDotValue(kvp.Value.Float2 * 1000, string.Format("{0} Total ({1})", kvp.Value.Float2.ToString("P3"), kvp.Key.ToChartTooltip()), dotColor));
                if (kvp.Value.Float1 > max) max = kvp.Value.Float1;
                if (kvp.Value.Float2 > max) max = kvp.Value.Float2;
            }

            areaChart.Values = lineChartValues;
            areaChart2.Values = lineChartValues2;

            // OFC can't display properly chart with small values
            max *= 1000;

            OpenFlashChart.OpenFlashChart chartObject = ChartUtil.CreateDefaultChart("Daily Conversion To Paying", max);
            chartObject.AddElement(ChartUtil.CreateMilestoneChart(max, ApplicationMilestones));
            chartObject.AddElement(areaChart2);
            chartObject.AddElement(areaChart);
            chartObject.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            chartObject.Y_Axis.Steps = (int)(max / 10);

            return chartObject;
        }

        public OpenFlashChart.OpenFlashChart DrawDarpuChart(Dictionary<DateTime, DecimalPair> darpu)
        {
            return DrawDarpuChart(darpu, "DARPU (Daily Average Revenue Per User)");
        }

        public OpenFlashChart.OpenFlashChart DrawDarpuChart(Dictionary<DateTime, DecimalPair> darpu, string title)
        {
            string dotColor = "#7bc90b";
            List<OpenFlashChart.LineDotValue> darpuLineChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 0;

            foreach (KeyValuePair<DateTime, DecimalPair> kvp in darpu)
            {
                decimal currentDarpu = kvp.Value.Decimal2 * 100;
                darpuLineChartValues.Add(new OpenFlashChart.LineDotValue((double)currentDarpu, string.Format("US${0} Total US${1} ({2})", kvp.Value.Decimal2.ToString("N3"), kvp.Value.Decimal1.ToString("N2"), kvp.Key.ToChartTooltip()), dotColor));
                if (currentDarpu > max) max = currentDarpu;
            }

            OpenFlashChart.Area darpuLineChart = ChartUtil.CreateDefaultAreaChart(dotColor);
            darpuLineChart.Values = darpuLineChartValues;

            OpenFlashChart.OpenFlashChart darpuChart = ChartUtil.CreateDefaultChart("DARPU (Daily Average Revenue Per User)", (double)max);
            darpuChart.AddElement(ChartUtil.CreateMilestoneChart((double)max, ApplicationMilestones));
            darpuChart.AddElement(darpuLineChart);
            darpuChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            darpuChart.Y_Axis.Labels.Steps = (int)(max / 10);

            return darpuChart;
        }

        public OpenFlashChart.OpenFlashChart DrawRetentionCohortsChart(Dictionary<DateTime, Dictionary<int, decimal>> cohorts, string title)
        {
            string oneDayRetentionColor = "#F06F30";
            string threeDaysRetentionColor = "#63BE6C";
            string sevenDaysRetentionColor = "#78B3E5";

            List<OpenFlashChart.LineDotValue> oneDayRetentionChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> threeDaysRetentionChartValues = new List<OpenFlashChart.LineDotValue>();
            List<OpenFlashChart.LineDotValue> sevenDaysRetentionChartValues = new List<OpenFlashChart.LineDotValue>();

            decimal max = 100;

            foreach (KeyValuePair<DateTime, Dictionary<int, decimal>> kvp in cohorts)
            {
                oneDayRetentionChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value[1] * 100, string.Format("One day retention {0} ({1})", kvp.Value[1].ToString("P2"), kvp.Key.ToChartTooltip()), oneDayRetentionColor));
                threeDaysRetentionChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value[3] * 100, string.Format("Three days retention {0} ({1})", kvp.Value[3].ToString("P2"), kvp.Key.ToChartTooltip()), threeDaysRetentionColor));
                sevenDaysRetentionChartValues.Add(new OpenFlashChart.LineDotValue((double)kvp.Value[7] * 100, string.Format("Seven days retention {0} ({1})", kvp.Value[7].ToString("P2"), kvp.Key.ToChartTooltip()), sevenDaysRetentionColor));
            }

            OpenFlashChart.Area oneDayRetentionLineChart = ChartUtil.CreateDefaultAreaChart(oneDayRetentionColor);
            oneDayRetentionLineChart.Values = oneDayRetentionChartValues;

            OpenFlashChart.Area threeDaysRetentionLineChart = ChartUtil.CreateDefaultAreaChart(threeDaysRetentionColor);
            threeDaysRetentionLineChart.Values = threeDaysRetentionChartValues;

            OpenFlashChart.Area sevenDaysRetentionLineChart = ChartUtil.CreateDefaultAreaChart(sevenDaysRetentionColor);
            sevenDaysRetentionLineChart.Values = sevenDaysRetentionChartValues;

            OpenFlashChart.OpenFlashChart cohortsChart = ChartUtil.CreateDefaultChart(title, (double)max);
            cohortsChart.AddElement(ChartUtil.CreateMilestoneChart((double)max, ApplicationMilestones));

            cohortsChart.AddElement(oneDayRetentionLineChart);
            cohortsChart.AddElement(threeDaysRetentionLineChart);
            cohortsChart.AddElement(sevenDaysRetentionLineChart);

            cohortsChart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
            cohortsChart.Y_Axis.Steps = 10;

            return cohortsChart;
        }

        #endregion
    }
}