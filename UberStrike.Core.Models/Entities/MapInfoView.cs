// -----------------------------------------------------------------------
// <copyright file="MapInfoView.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace UberStrike.DataCenter.Common.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Cmune.DataCenter.Common.Entities;
    using UberStrike.Core.Types;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MapInfoView
    {
        #region Properties

        public int MapId { get; private set; }
        public string DisplayName { get; private set; }
        public string SceneName { get; private set; }
        public string Description { get; private set; }
        public bool InUse { get; private set; }
        public bool IsBlueBox { get; private set; }
        public int ItemId { get; private set; }
        public Dictionary<MapType, MapVersionView> Assets { get; private set; }

        #endregion

        #region Constructors

        public MapInfoView(int mapId, string displayName, string sceneName, string description, bool inUse, bool isBlueBox, int itemId, Dictionary<MapType, MapVersionView> assets)
        {
            this.MapId = mapId;
            this.DisplayName = displayName;
            this.SceneName = sceneName;
            this.Description = description;
            this.InUse = inUse;
            this.IsBlueBox = isBlueBox;
            this.Assets = assets;
            this.ItemId = itemId;
        }

        #endregion
    }
}
