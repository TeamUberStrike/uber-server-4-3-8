
using System.Reflection;
using UberStrike.Core.Types;

namespace Cmune.Realtime.Common
{
    public static class Protocol
    {
        public const byte Major = 1;
        public const byte Minor = 7;

        public static readonly string Version = string.Format("{0}.{1}", Major, Minor);
    }

    [CmuneEnum]
    public enum AssetType
    {
        SPACE = 1,
        AVATAR = 3,
        OBJECT = 4
    }

    [System.Flags]
    public enum PlayerTagFlag
    {
        None = BIT_FLAGS.BIT_NONE,
        Speedhacker = BIT_FLAGS.BIT_01,
        Ammohacker = BIT_FLAGS.BIT_02,
        ReportedCheater = BIT_FLAGS.BIT_03,
        ReportedAbuser = BIT_FLAGS.BIT_04,
    }

    public enum PerfCounter
    {
        //System Counter
        MemoryAvailable = 0,
        ProcessorTime = 1,
        TotalPhysicalMemory = 2,

        //Process Counter
        ProcessPrivateBytes = 10,
        ProcessRuntime = 11,
        VirtualBytes = 12,

        //Internal Count
        CountPlayers = 20,
        CountGames = 21,

        //Photon Counter
        PhotonPeers = 30,

        //.Net Counter
        DotNetExceptions = 40,
        DotNetCurrentThreads = 41,
        DotNetBytesInAllHeaps = 42,
        DotNetLOH = 43,
    }

    public static class BindingFlag
    {
        public static BindingFlags PublicStatic = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    }

    public enum KeyboardLayout
    {
        QWERTY = 0,
        AZERTY = 1,
        QWERTZ = 2
    }

    /// <summary>
    /// BYTE (8 states)
    /// </summary>
    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
    }

    public enum MessageChannel
    {
        ALL = 0,
        LOBBY = 1,
        GAME = 2,
    }

    public enum ReportType
    {
        ABUSION = 1,
        CHEAT = 2,
    }
}
