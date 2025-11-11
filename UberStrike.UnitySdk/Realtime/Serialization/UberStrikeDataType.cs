
using Cmune.Realtime.Common;
using Cmune.Core.Types.Attributes;

namespace UberStrike.Realtime.Common
{
    [ExtendableEnumBounds(101, 200)]
    public class UberStrikeDataType : CmuneDataType
    {
        //Single Types
        public const byte ActorInfo = 101;
        public const byte ShortVector = 102;
        public const byte GameData = 103;

        public const byte Armor = 104;
        public const byte Weapons = 105;
        public const byte Statistics = 106;
        public const byte DamageEvent = 107;
        public const byte Stats = 108;
        public const byte EndOfMatch = 109;

        //Array Types
        public const byte Array_GameData = 111;
    }
}