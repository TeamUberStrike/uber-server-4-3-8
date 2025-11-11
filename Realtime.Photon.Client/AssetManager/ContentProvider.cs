using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Util;

namespace Cmune.Unity.Client
{
    /// <summary>
    /// Main Interface to instantiate Content -> from memory, disk or web
    /// </summary>
    public static class ContentProvider
    {
        public static void SetAssetLoader(IAssetLoader loader)
        {
            _assetLoader = loader;
        }

        /// <summary>
        /// Load the asset with specified ID plus a specific configuration
        /// </summary>
        /// <param name="assetID">The ID of the local instance</param>
        /// <param name="config">The configuration of the asset</param>
        public static void LoadAsset(int instanceID, int assetID, List<byte> config, CmuneTransform transform, bool isLocal)
        {
            IContent content;

            if (_assetLoader == null)
            {
                CmuneDebug.Exception("AssetLoader is null!");
            }

            if (_contents.TryGetValue(assetID, out content))
            {
                //If there is not special configuration passed, use the stored configuration
                //In the case of using BuildInContent the config is null too
                //thats fine as the object is already configured by the inspector.
                List<byte> c = config;
                if (c == null)
                    c = content.Configuration;

                _assetLoader.LoadAsset(instanceID, content.Type, assetID, c, transform, isLocal);
            }
            else
            {
                CmuneDebug.LogError("Can not find the content with ID: " + assetID);
            }
        }

        public static void UpdateContents(IContent[] list)
        {
            foreach (IContent c in list)
            {
                if (_contents.ContainsKey(c.AssetID))
                {
                    _contents.Remove(c.AssetID);
                }

                _contents.Add(c.AssetID, c);
            }
        }

        #region FIELDS

        private static Dictionary<int, IContent> _contents = new Dictionary<int, IContent>();

        private static IAssetLoader _assetLoader = null;

        public static IAssetLoader AssetLoader
        {
            get { return ContentProvider._assetLoader; }
        }

        #endregion
    }
}