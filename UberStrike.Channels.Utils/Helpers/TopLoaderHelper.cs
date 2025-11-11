using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace UberStrike.Channels.Utils.Helpers
{
    public static class TopLoaderHelper
    {
        private static string LoaderScript()
        {
            var script = HtmlBaseHelper.JsBegin() + " function ShowTopLoader() { "
                + " $(\"#toploader\").show(); } "
                + " function HideTopLoader() { $(\"#toploader\").hide(); }"
                + HtmlBaseHelper.JsEnd();
            return script;
        }

        private static string LoaderBind()
        {
            var script = HtmlBaseHelper.JsBegin()
            + "$(\"#toploader\").bind(\"ajaxSend\", function () {"
//            + " console.log(\"show\");"
            + " ShowTopLoader();"
            + "}).bind(\"ajaxComplete\", function (data) { "
            + " HideTopLoader();"
            + " });"
            + HtmlBaseHelper.JsEnd();
            return script;
        }

        public static string LoaderElement(HtmlHelper html, UrlHelper url)
        {
            var element = "<div style=\"width: 20px; height: 20px\">"
                + "<div style=\"display:none\" id=\"toploader\">"
                + html.ImageTag(url, "KongregateChannel/ajax-loader-kg.gif")
                + "</div>"
                + "</div>";
            return element;
        }

        public static HtmlString TopLoader(this HtmlHelper html, UrlHelper url)
        {
            return new HtmlString(LoaderElement(html, url) + LoaderScript() + LoaderBind());
        }
    }
}
