using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cmune.Channels.Instrumentation.Business.Chart
{
    public class BaseChart
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        protected void SetXAxis(OpenFlashChart.OpenFlashChart chart)
        {
            if (chart != null)
            {
                chart.X_Axis.SetLabels(ChartUtil.GetXAxisLabels(FromDate, ToDate));
                chart.X_Axis.Steps = 3;
            }
        }
    }
}