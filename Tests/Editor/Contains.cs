using NUnit.Framework;
using Unity.Collections;

namespace SIMDAlgorithms.Tests
{
    public static class Contains
    {
        #region EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
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
                for (int j = 0; j < 10; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
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
                for (int j = 0; j < 10; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
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
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = array.Contains(test);
                    bool mine = array.SIMD_Contains(test);

                    Assert.AreEqual(std, mine);

                    if (std == false)
                    {
                        test = array[rng.NextInt(0, array.Length)];

                        Assert.AreEqual(array.Contains(test), array.SIMD_Contains(test));
                    }
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion

        #region NOT_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
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
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
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
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
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
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.NotEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion

        #region LESS
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
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
                for (int j = 0; j < 10; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
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
                for (int j = 0; j < 10; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
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
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] < test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion

        #region GREATER
        [Test, Timeout(int.MaxValue)]
        public static void Byte_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
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
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
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
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
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
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] > test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThan);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion

        #region LESS_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] <= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.LessThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion

        #region GREATER_EQUAL
        [Test, Timeout(int.MaxValue)]
        public static void Byte_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UShort_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void UInt_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    uint test = rng.NextUInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            rng.NextUInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void ULong_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void SByte_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1),
            500);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Short_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1),
            100000);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Int_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    int test = rng.NextInt();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            rng.NextInt);
        }

        [Test, Timeout(int.MaxValue)]
        public static void Long_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Float_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    float test = rng.NextFloat();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextFloat(float.MinValue, float.MaxValue));
        }

        [Test, Timeout(int.MaxValue)]
        public static void Double_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 25; j++)
                {
                    double test = rng.NextDouble();

                    bool std = false;

                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] >= test)
                        {
                            std = true;
                            break;
                        }
                    }

                    bool mine = array.SIMD_Contains(test, Comparison.GreaterThanOrEqualTo);

                    Assert.AreEqual(std, mine);
                }
            },
            () => rng.NextDouble(double.MinValue / 2d, double.MaxValue / 2d));
        }
        #endregion
    }
}