using NUnit.Framework;
using System;

namespace SIMDAlgorithms.Tests
{
    public static class Count
    {
        [Test, Timeout(int.MaxValue)]
        public static void Bool_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<bool>(
            (array) =>
            {
                long std_true = 0;
                long std_false = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    if (array[j] == true)
                    {
                        std_true++;
                    }
                    else
                    {
                        std_false++;
                    }
                }

                Assert.AreEqual(std_true, array.SIMD_Count());
                Assert.AreEqual(std_false, array.SIMD_Count(false));
            },
            rng.NextBool);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Bool_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<bool>(
            (array) =>
            {
                byte std_true = 0;
                byte std_false = 0;

                for (int j = 0; j < array.Length; j++)
                {
                    if (array[j] == true)
                    {
                        std_true++;
                    }
                    else
                    {
                        std_false++;
                    }
                }

                Assert.AreEqual(std_true, array.SIMD_Count(true, TypeCode.Byte));
                Assert.AreEqual(std_false, array.SIMD_Count(false, TypeCode.Byte));
            },
            rng.NextBool);
        }

        #region EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_Equal_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, returnType: TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Equal_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, returnType: TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                long std = 0;
                ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                for (int k = 0; k < array.Length; k++)
                {
                    if (array[k] == test)
                    {
                        std++;
                    }
                }

                if (std == 0)
                {
                    test = array[rng.NextInt(0, array.Length)];

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }
                }

                Assert.AreEqual(std, array.SIMD_Count(test));
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Equal_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, returnType: TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Equal_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, returnType: TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Equal_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                long std = 0;
                long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                for (int k = 0; k < array.Length; k++)
                {
                    if (array[k] == test)
                    {
                        std++;
                    }
                }

                if (std == 0)
                {
                    test = array[rng.NextInt(0, array.Length)];

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }
                }

                Assert.AreEqual(std, array.SIMD_Count(test));
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] == test)
                        {
                            std++;
                        }
                    }

                    if (std == 0)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] == test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test));
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region NOT_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_NotEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo, TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_NotEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo, TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_NotEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo, TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_NotEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo, TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_NotEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] != test)
                        {
                            std++;
                        }
                    }

                    if (std == array.Length)
                    {
                        test = array[rng.NextInt(0, array.Length)];
                        std = 0;

                        for (int k = 0; k < array.Length; k++)
                        {
                            if (array[k] != test)
                            {
                                std++;
                            }
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.NotEqualTo));
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region LESS
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_Less_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan, TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Less_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan, TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                long std = 0;
                ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                for (int k = 0; k < array.Length; k++)
                {
                    if (array[k] < test)
                    {
                        std++;
                    }
                }

                Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Less_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan, TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Less_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan, TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Less_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                long std = 0;
                long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                for (int k = 0; k < array.Length; k++)
                {
                    if (array[k] < test)
                    {
                        std++;
                    }
                }

                Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] < test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThan));
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region GREATER
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_Greater_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan, TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Greater_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan, TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Greater_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan, TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Greater_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan, TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Greater_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] > test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThan));
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region LESS_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_LessOrEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo, TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_LessOrEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo, TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_LessOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_LessOrEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo, TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_LessOrEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo, TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_LessOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_LessOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_LessOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_LessOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] <= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.LessThanOrEqualTo));
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region GREATER_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Byte_GreaterOrEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo, TypeCode.Byte));
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_GreaterOrEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo, TypeCode.UInt16));
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    uint test = rng.NextUInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_GreaterOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    sbyte test = (sbyte)rng.NextInt(byte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_GreaterOrEqual_RangeByte()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte std = 0;
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo, TypeCode.Byte));
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_GreaterOrEqual_RangeShort()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort std = 0;
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo, TypeCode.UInt16));
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_GreaterOrEqual_RangeInt()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    int test = rng.NextInt();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_GreaterOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_GreaterOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    float test = rng.NextFloat();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            rng.NextFloat);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_GreaterOrEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long std = 0;
                    double test = rng.NextDouble();

                    for (int k = 0; k < array.Length; k++)
                    {
                        if (array[k] >= test)
                        {
                            std++;
                        }
                    }

                    Assert.AreEqual(std, array.SIMD_Count(test, Comparison.GreaterThanOrEqualTo));
                }
            },
            rng.NextDouble);
        }
        #endregion
    }
}