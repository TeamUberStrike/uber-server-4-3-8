using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Mvc;

namespace Cmune.Channels.Instrumentation.Helper
{
    public static class ComparerHelper
    {
        /// <summary>
        /// Compare 2 values and get a html tag class property
        /// </summary>
        /// <returns></returns>
        public static string CompareValues(this HtmlHelper html, int value, int value2)
        {
            return value == value2 ? "" : "highlight";
        }

        /// <summary>
        /// Compare 2 values and get a html tag class property
        /// </summary>
        /// <returns></returns>
        public static string CompareValues(this HtmlHelper html, bool value, bool value2)
        {
            return value == value2 ? "" : "highlight";
        }

        /// <summary>
        /// Compare 2 values and get a html tag class property
        /// </summary>
        /// <returns></returns>
        public static string CompareValues(this HtmlHelper html, string value, string value2)
        {
            return value == value2 ? "" : "highlight";
        }

        public static HtmlString CompareItemValues(this HtmlHelper html, UrlHelper urlHelper, string htmlContainerTag, string title, string value, string value2)
        {
            bool booleanResult = false;
            
            string element = "<" + htmlContainerTag + " ";
            element += "class=\"" + html.CompareValues(value, value2) + "\"";
            element += " title=\"" + title + "\"";
            element += ">";;

            if (bool.TryParse(value, out booleanResult))
                element += booleanResult ? html.TrueNotif(urlHelper) : html.FalseNotif(urlHelper);
            else
                element += value;
            element += "<br/>";
            if (bool.TryParse(value2, out booleanResult))
                element += booleanResult ? html.TrueNotif(urlHelper) : html.FalseNotif(urlHelper);
            else
                element += value2;
            element += "</" + htmlContainerTag + ">";

            return new HtmlString(element);
        }
    }
}