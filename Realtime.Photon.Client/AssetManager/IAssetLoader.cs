using System.Collections.Generic;
using Cmune.Realtime.Common;

namespace Cmune.Unity.Client
{
    public interface IAssetLoader
    {
        void LoadAsset(int instanceID, AssetType type, int assetID, List<byte> config, CmuneTransform transform, bool isLocal);

        void UnloadAsset(int instanceID);

        ICollection<IAssetConfig> Instances { get; }

        int Count { get; }
    }
}