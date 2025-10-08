using System;

namespace Cmune.Util
{
    public static class ConvertBytes
    {
        const double toKilo = 0.0009765625;

        public static float ToKiloBytes(ulong bytes)
        {
            return Convert.ToSingle(bytes * toKilo);
        }

        public static float ToKiloBytes(int bytes)
        {
            return Convert.ToSingle(bytes * toKilo);
        }

        public static float ToKiloBytes(long bytes)
        {
            return Convert.ToSingle(bytes * toKilo);
        }


        public static float ToMegaBytes(ulong bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo);
        }
     
        public static float ToMegaBytes(long bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo);
        }

        public static float ToMegaBytes(int bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo);
        }


        public static float ToGigaBytes(ulong bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo * toKilo);
        }

        public static float ToGigaBytes(long bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo * toKilo);
        }
  
        public static float ToGigaBytes(int bytes)
        {
            return Convert.ToSingle(bytes * toKilo * toKilo * toKilo);
        }
    }
}