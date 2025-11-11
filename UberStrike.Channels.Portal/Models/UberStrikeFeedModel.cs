using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UberStrike.Channels.Portal.Models
{
    public class UberStrikeFeedModel
    {
        #region Properties

        public string Link { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Tag { get; private set; }
        public DateTime Date { get; private set; }

        #endregion Properties

        #region Constructors

        public UberStrikeFeedModel(string link, string title, string description, string tag, DateTime date)
        {
            this.Link = link;
            this.Title = title;
            this.Description = description;
            this.Tag = tag;
            this.Date = date;
        }

        #endregion Constructors
    }
}