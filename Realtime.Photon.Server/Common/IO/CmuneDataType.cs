
using Cmune.Core.Types;

namespace Cmune.Realtime.Common
{
    /// <summary>
    /// CmuneDataType defines all Data Types that should be serializable.
    /// All DataTypes are mapped to an unique Byte ID.
    /// 
    /// To define custom serialzable datatypes it is recommended to derive from this class and use the byte IDs from 100 - 254.
    /// Then override the class DefaultByteConverter to implement your custom serialization.
    /// Don't forget to activate your custom Serialization by setting RealtimeSerialization.Converter to your own implementation.
    /// </summary>
    //[ExtendableEnumBounds(0, 100)]
    public class CmuneDataType : ExtendableEnum<byte>
    {
        public const byte None = 0;

        //atomic types
        public const byte Byte = 1;
        public const byte SByte = 2;
        public const byte Bool = 3;
        public const byte Int16 = 4;
        public const byte UInt16 = 5;
        public const byte Int32 = 6;
        //public const byte UINT32 = 7;
        public const byte Long = 8;
        //public const byte ULONG = 9;
        public const byte Float = 10;
        //public const byte DOUBLE = 11;
        public const byte String = 12;

        //collection types
        public const byte Array_Byte = 15;
        //public const byte ARRAY_SBYTE = 16;
        //public const byte ARRAY_BOOL = 17;
        public const byte Array_Short = 18;
        public const byte Array_UShort = 19;
        public const byte Array_Int = 20;
        //public const byte ARRAY_UINT = 21;
        public const byte Array_Long = 22;
        //public const byte ARRAY_ULONG = 23;
        public const byte Array_Float = 24;
        //public const byte ARRAY_DOUBLE = 25;
        public const byte Array_String = 26;

        //unity atomics
        public const byte Vector3 = 30;
        public const byte Quaternion = 31;
        public const byte Color = 32;

        //unity collections
        public const byte Array_Vector3 = 35;
        public const byte Array_Quaternion = 36;

        //cmune
        public const byte RoomData = 40;
        //public const byte USERINFO = 41;
        public const byte Transform = 42;
        public const byte AssetType = 43;
        public const byte PhysicsPack = 44;
        public const byte RoomId = 45;
        public const byte CommActorInfo = 46;
        public const byte PerformanceData = 47;
        public const byte SyncObject = 48;

        //collections
        //public const byte ARRAY_USERINFO = 50;
        public const byte Array_RoomId = 51;
        //public const byte ARRAY_COMMUSERINFO = 52;
        public const byte Array_SyncObject = 53;
    }
}
