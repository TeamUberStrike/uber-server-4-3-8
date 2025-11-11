using System.Collections.Generic;
using Cmune.Realtime.Common;
using Cmune.Realtime.Photon.Client.Events;
using Cmune.Realtime.Common.Utils;

namespace Cmune.Unity.Client
{
    /// <summary>
    /// Manages 3D objects in the game scene.
    /// This is a singleton for there should be only one manager in a scene.
    /// 
    /// * 2009 Oct 22 - alex
    ///   Delete function "ConfigureSceneSynchronization" which is called 
    ///   by SpaceSynchronizer in function "initSceneRPC".
    /// </summary>
    public static class SceneManager
    {
        public static void ClearAll(AssetType type)
        {
            if (ContentProvider.AssetLoader != null)
            {
                IAssetConfig[] assets = Conversion.ToArray<IAssetConfig>(ContentProvider.AssetLoader.Instances);
                foreach (IAssetConfig i in assets)
                {
                    if (i.AssetType == type)
                        ContentProvider.AssetLoader.UnloadAsset(i.InstanceID);
                }
            }
        }

        #region INTERFACE

        public static int LoadAsset(int assetID)
        {
            return LoadAsset(assetID, null, new CmuneTransform(), true);
        }

        public static int LoadAsset(int assetID, CmuneTransform transform)
        {
            return LoadAsset(assetID, null, transform, true);
        }

        // These two functions are the main interface
        /// <summary>
        /// Load an asset with specified ID and a configuration
        /// </summary>
        /// <param name="assetID">ID of the asset</param>
        /// <param name="config">Configuration</param>
        /// <param name="transform">Transform of the asset</param>
        /// <param name="isLocal">The creator is local or not</param>
        /// <returns>Instance ID</returns>
        public static int LoadAsset(int assetID, List<byte> config, CmuneTransform transform, bool isLocal)
        {
            int instanceID = NextInstanceID;

            ContentProvider.LoadAsset(instanceID, assetID, config, transform, isLocal);

            return instanceID;
        }

        public static int LoadAsset(int instanceID, int assetID, List<byte> config, CmuneTransform transform, bool isLocal)
        {
            ContentProvider.LoadAsset(instanceID, assetID, config, transform, isLocal);

            return instanceID;
        }

        public static void RemoveInstanceWithID(int instanceID)
        {
            ContentProvider.AssetLoader.UnloadAsset(instanceID);
        }

        #endregion

        public static void ResetMode()
        {
        }

        #region PROPERTIES
        public static int Count
        {
            get
            {
                return ContentProvider.AssetLoader.Count;
            }
        }

        public static IEnumerable<IAssetConfig> AllInstances
        {
            get
            {
                return ContentProvider.AssetLoader.Instances;
            }
        }

        public static int NextInstanceID
        {
            get
            {
                return ++_currentInstanceID;
            }
        }
        #endregion

        #region FIELDS
        // The self-incrementing instance ID
        private static int _currentInstanceID = 0;
        #endregion
    }
}