using System.Collections.Generic;
using Cmune.Realtime.Common;

namespace Cmune.Unity.Client
{
    public interface IContent
    {
        AssetType Type { get; }

        int AssetID { get; }

        List<byte> Configuration { get; set; }
    }
}