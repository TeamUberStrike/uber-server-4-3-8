using System;
using System.Web.Mvc;
using System.Text;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class OpenFlashChartHelper
    {
        #region OpenFlashChart

        public static string RenderChart(this HtmlHelper helper,
          string chartContainer, //This is the id of the html container where the chart is going to be rendered preferably a div.
          string chartData, //This is the route to the chart JSON formatted data
          int width, //This is the chart width
          int height //This is the chart height
          , UrlHelper url)
        {
            //This is chart's the loading text
            string loadingText = "Loading data...";
            var chart = new StringBuilder();

            //Creating the script tag
            var script = new TagBuilder("script");

            //Adding the type attribute
            script.Attributes.Add("type", "text/javascript");

            //Setting the swf object definition for the open flash chart
            string swfobject = "swfobject.embedSWF(" +
                        "\"" + url.Content("~/Content/swf/open-flash-chart.swf") + "\"," + //This can be set from a config file or w/e
                        "\"" + chartContainer + "\"," +
                        "\"" + width + "\"," +
                        "\"" + height + "\"," +
                        "\"9.0.0\"," +
                        "\"" + url.Content("~/Content/swf/expressInstall.swf") + "\"," +
                        "{ \"data-file\": \"" + Uri.EscapeDataString(chartData) + "\", \"loading\":\"" + loadingText + "\" }" +
                    ");";

            //Setting the inner text of the script tag without HTML encoding
            script.InnerHtml = swfobject;

            //Appending the script tag to the helper result
            chart.Append(script);

            //Returning the open flash chart client code implementation
            return chart.ToString();
        }
        #endregion
    }
}