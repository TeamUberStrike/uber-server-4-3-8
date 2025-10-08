using UnityEngine;
using Cmune.Realtime.Common;

namespace Cmune.Unity.Client
{
    /// <summary>
    /// Interface to ConfigScript
    /// </summary>
    public interface IAssetConfig
    {
        int AssetID { get; }

        AssetType AssetType { get; }

        GameObject GameObject { get; }

        string Name { get; }

        int InstanceID { get; }

        CmuneTransform GetTransform();

        Vector3 Bounds { get; }

        void Destroy();
    }

    /// <summary>
    /// Interface to ConfigScript
    /// </summary>
    public interface INetAssetConfig : IAssetConfig, INetworkClass, IByteArray
    {
    }
}