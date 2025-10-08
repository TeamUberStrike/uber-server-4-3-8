using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberStrike.DataCenter.Common.Entities
{
    public class MapClusterView
    {
        #region Properties

        public string ApplicationVersion { get; private set; }
        public List<MapInfoView> Maps { get; private set; }

        #endregion

        #region Constructors

        public MapClusterView(string appVersion, List<MapInfoView> maps)
        {
            ApplicationVersion = appVersion;
            Maps = maps;
        }

        #endregion
    }
}