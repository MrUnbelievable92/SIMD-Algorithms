namespace SIMDAlgorithms
{
    unsafe public static partial class Algorithms
    {
        internal const byte BYTES_IN_SHORT = sizeof(short) / sizeof(byte);
        internal const byte BYTES_IN_INT = sizeof(int) / sizeof(byte);
        internal const byte BYTES_IN_LONG = sizeof(long) / sizeof(byte);
        internal const byte BYTES_IN_V128 = /*sizeof(v128)*/16 / sizeof(byte);
        internal const byte BYTES_IN_V256 = /*sizeof(v256)*/32 / sizeof(byte);

        internal const byte SHORTS_IN_INT = sizeof(int) / sizeof(short);
        internal const byte SHORTS_IN_LONG = sizeof(long) / sizeof(short);
        internal const byte SHORTS_IN_V128 = /*sizeof(v128)*/16 / sizeof(short);
        internal const byte SHORTS_IN_V256 = /*sizeof(v256)*/32 / sizeof(short);

        internal const byte INTS_IN_LONG = sizeof(long) / sizeof(int);
        internal const byte INTS_IN_V128 = /*sizeof(v128)*/16 / sizeof(int);
        internal const byte INTS_IN_V256 = /*sizeof(v256)*/32 / sizeof(int);

        internal const byte LONGS_IN_V128 = /*sizeof(v128)*/16 / sizeof(long);
        internal const byte LONGS_IN_V256 = /*sizeof(v256)*/32 / sizeof(long);

        internal const byte V128_IN_V256 = /*sizeof(v256)*/32 / /*sizeof(v128)*/16;
    }
}
