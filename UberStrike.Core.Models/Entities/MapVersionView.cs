// -----------------------------------------------------------------------
// <copyright file="MapVersionView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UberStrike.DataCenter.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MapVersionView
    {
        #region Properties

        public string FileName { get; private set; }
        public DateTime LastUpdatedDate { get; set; }

        #endregion

        #region Constructors

        public MapVersionView(string fileName, DateTime lastUpdatedDate)
        {
            this.FileName = fileName;
            this.LastUpdatedDate = lastUpdatedDate;
        }

        #endregion
    }
}
