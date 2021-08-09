using System;
using System.Runtime.CompilerServices;

namespace SIMDAlgorithms
{
    unsafe public static class GetSafeRange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode Summation(TypeCode summandType, long summandCount)
        {
            switch (summandType)
            {
                case TypeCode.Boolean:
                {
                    bool possible8BitOverflow  = summandCount > byte.MaxValue;
                    bool possible16BitOverflow = summandCount > ushort.MaxValue;
                    bool possible32BitOverflow = summandCount > uint.MaxValue;

                    return (TypeCode)((int)TypeCode.Byte + ((*(byte*)&possible8BitOverflow + *(byte*)&possible16BitOverflow + *(byte*)&possible32BitOverflow) << 1));
                }
                case TypeCode.SByte:
                {
                    bool possible16BitOverflow = summandCount > short.MinValue / sbyte.MinValue;
                    bool possible32BitOverflow = summandCount > int.MinValue / sbyte.MinValue;

                    return (TypeCode)((int)TypeCode.Int16 + ((*(byte*)&possible16BitOverflow + *(byte*)&possible32BitOverflow) << 1));
                }
                case TypeCode.Int16:
                {
                    bool possible32BitOverflow = summandCount > int.MinValue / short.MinValue;

                    return (TypeCode)((int)TypeCode.Int32 + (*(byte*)&possible32BitOverflow << 1));
                }
                case TypeCode.Int32:
                {
                    return TypeCode.Int64;
                }
                case TypeCode.Int64:
                {
                    return TypeCode.Int64;
                }
                case TypeCode.Byte:
                {
                    bool possible16BitOverflow = summandCount > ushort.MaxValue / byte.MaxValue;
                    bool possible32BitOverflow = summandCount > uint.MaxValue / byte.MaxValue;

                    return (TypeCode)((int)TypeCode.UInt16 + ((*(byte*)&possible16BitOverflow + *(byte*)&possible32BitOverflow) << 1));
                }
                case TypeCode.UInt16:
                {
                    bool possible32BitOverflow = summandCount > uint.MaxValue / ushort.MaxValue;

                    return (TypeCode)((int)TypeCode.UInt32 + (*(byte*)&possible32BitOverflow << 1));
                }
                case TypeCode.UInt32:
                {
                    return TypeCode.UInt64;
                }
                case TypeCode.UInt64:
                {
                    return TypeCode.UInt64;
                }
                case TypeCode.Single:
                {
                    return TypeCode.Double;
                }
                case TypeCode.Double:
                {
                    return TypeCode.Double;
                }
                default:
                {
                    throw new ArrayTypeMismatchException($"Type { summandType } cannot be summed.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode Count(long arrayLength)
        {
            bool possible8BitOverflow  = arrayLength > byte.MaxValue;
            bool possible16BitOverflow = arrayLength > ushort.MaxValue;
            bool possible32BitOverflow = arrayLength > uint.MaxValue;

            return (TypeCode)((int)TypeCode.Byte + ((*(byte*)&possible8BitOverflow + *(byte*)&possible16BitOverflow + *(byte*)&possible32BitOverflow) << 1));
        }
    }
}