using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using Cmune.DataCenter.Utils;
using UberStrike.Channels.Portal.Models;

namespace UberStrike.Channels.Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        [OutputCache(Duration=1200,Location=OutputCacheLocation.Server)]
        public ActionResult DisplayRssFeed()
        {
            var wordPressFeedUrl = ConfigurationUtilities.ReadConfigurationManager("WordPressFeedURL");
            int scriptStart = Environment.TickCount;

            StringBuilder blogDebug = new StringBuilder();
            blogDebug.Append("<!-- ");
            List<UberStrikeFeedModel> blogFeed = new List<UberStrikeFeedModel>();

            try
            {
                WebRequest request = WebRequest.Create(wordPressFeedUrl);
                request.Timeout = 45000;

                using (WebResponse response = request.GetResponse())
                using (XmlReader blogRssXmlReader = XmlReader.Create(response.GetResponseStream()))
                {
                    SyndicationFeed blogRss = SyndicationFeed.Load(blogRssXmlReader);

                    if (blogRss.Items != null && blogRss.Items.Count() > 0)
                    {
                        blogDebug.Append("[blogRss:");
                        blogDebug.Append(blogRss.Items.Count());
                        blogDebug.Append(" elements]");

                        List<SyndicationItem> feeds = blogRss.Items.Take(ConfigurationUtilities.ReadConfigurationManagerInt("WordPressFeeds")).ToList();

                        DateTime timeLimit = DateTime.Now.AddMonths(-2);

                        for (int i = 0; i < 20 && i < feeds.Count && feeds[i].PublishDate > timeLimit; i++)
                        {
                            string articleContent = String.Empty;

                            foreach (SyndicationElementExtension extension in feeds[i].ElementExtensions)
                            {
                                XElement ele = extension.GetObject<XElement>();

                                if (ele.Name.LocalName == "encoded" && ele.Name.Namespace.ToString().Contains("content"))
                                {
                                    articleContent = ele.Value;
                                }
                            }

                            blogFeed.Add(new UberStrikeFeedModel(feeds[i].Links[0].Uri.ToString(), feeds[i].Title.Text, articleContent, feeds[i].Categories[0].Name.ToString(), feeds[i].PublishDate.DateTime));
                        }
                    }
                    else
                    {
                        blogDebug.Append("[blogRss:NULL OR EMPTY!]");
                    }
                }

            }
            catch (Exception ex)
            {
                blogDebug.Append("[EXCEPTION:[StackTrace:");
                blogDebug.Append(ex.StackTrace);
                blogDebug.Append("][MESSAGE:");
                blogDebug.Append(ex.Message);
                blogDebug.Append("]]");
                CmuneLog.LogException(ex, "Unable to fetch blog");
            }

            blogDebug.Append("[Generation time: ");

            int scriptEnd = Environment.TickCount;
            int duration = scriptEnd - scriptStart;
            blogDebug.Append("[Duration:");
            blogDebug.Append(duration);
            blogDebug.Append("ms (");
            blogDebug.Append(duration / 1000);
            blogDebug.Append("s)]");

            blogDebug.Append("[Generation time: ");
            blogDebug.Append(DateTime.Now.ToString("HH:mm:ss"));
            blogDebug.Append("]");
            blogDebug.Append(" -->");

            return PartialView("RssFeedPartial", blogFeed);
        }

        public ActionResult RefreshTopMenu()
        {
            return PartialView("LittleTopMenuPartial");
        }
    }
}
