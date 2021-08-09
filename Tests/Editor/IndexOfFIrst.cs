using NUnit.Framework;
using Unity.Collections;

namespace SIMDAlgorithms.Tests
{
    public static class IndexOfFirst
    {

        #region EQUAL
        [Test]
        public static void Byte_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                byte test = 47;
                
                NativeArray<byte> array = new NativeArray<byte>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void UShort_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                ushort test = 47;
                
                NativeArray<ushort> array = new NativeArray<ushort>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void UInt_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                uint test = 47;
                
                NativeArray<uint> array = new NativeArray<uint>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void ULong_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                ulong test = 47;
                
                NativeArray<ulong> array = new NativeArray<ulong>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void SByte_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                sbyte test = 47;
                
                NativeArray<sbyte> array = new NativeArray<sbyte>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Short_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                short test = 47;
                
                NativeArray<short> array = new NativeArray<short>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Int_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                int test = 47;
                
                NativeArray<int> array = new NativeArray<int>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Long_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                long test = 47;
                
                NativeArray<long> array = new NativeArray<long>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Float_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                float test = 47;
                
                NativeArray<float> array = new NativeArray<float>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Double_Direct_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                double test = 47;
                
                NativeArray<double> array = new NativeArray<double>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = 100;
                }

                array[i] = test;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test));

                array.Dispose();
            }
        }

        [Test]
        public static void Byte_Direct_FirstBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[13] = test;

            Assert.AreEqual(13, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_SecondBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[23] = test;

            Assert.AreEqual(23, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_ThirdBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(48, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[40] = test;

            Assert.AreEqual(40, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_FourthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(56, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[52] = test;

            Assert.AreEqual(52, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_SixthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(60, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[58] = test;

            Assert.AreEqual(58, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_SeventhBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(62, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[60] = test;

            Assert.AreEqual(60, array.SIMD_IndexOf(test));

            array[60] = 200;

            array[61] = test;

            Assert.AreEqual(61, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Direct_EighthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(63, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[62] = test;

            Assert.AreEqual(62, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Direct_FirstBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Direct_SecondBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[14] = test;

            Assert.AreEqual(14, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Direct_ThirdBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(24, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[22] = test;

            Assert.AreEqual(22, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Direct_FourthBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(28, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[26] = test;

            Assert.AreEqual(26, array.SIMD_IndexOf(test));

            array.Dispose();
        }

        [Test]
        public static void UShort_Direct_SixthBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(30, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[28] = test;

            Assert.AreEqual(28, array.SIMD_IndexOf(test));

            array[28] = 200;

            array[29] = test;

            Assert.AreEqual(29, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Direct_SeventhBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(31, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[30] = test;

            Assert.AreEqual(30, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Direct_FirstBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Direct_SecondBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Direct_ThirdBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(12, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[10] = test;

            Assert.AreEqual(10, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Direct_SixthBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(14, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[12] = test;

            Assert.AreEqual(12, array.SIMD_IndexOf(test));

            array[12] = 200;

            array[13] = test;

            Assert.AreEqual(13, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Direct_SeventhBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(15, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[14] = test;

            Assert.AreEqual(14, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Direct_FirstBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[1] = test;

            Assert.AreEqual(1, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Direct_SecondBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Direct_SixthBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(6, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[4] = test;

            Assert.AreEqual(4, array.SIMD_IndexOf(test));

            array[4] = 200;

            array[5] = test;

            Assert.AreEqual(5, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Direct_SeventhBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(7, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_FirstBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[13] = test;

            Assert.AreEqual(13, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_SecondBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[23] = test;

            Assert.AreEqual(23, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_ThirdBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(48, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[40] = test;

            Assert.AreEqual(40, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_FourthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(56, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[52] = test;

            Assert.AreEqual(52, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_SixthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(60, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[58] = test;

            Assert.AreEqual(58, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_SeventhBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(62, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[60] = test;

            Assert.AreEqual(60, array.SIMD_IndexOf(test));

            array[60] = 123;

            array[61] = test;

            Assert.AreEqual(61, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Direct_EighthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(63, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 123;
            }

            array[62] = test;

            Assert.AreEqual(62, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Direct_FirstBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Direct_SecondBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[14] = test;

            Assert.AreEqual(14, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Direct_ThirdBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(24, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[22] = test;

            Assert.AreEqual(22, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Direct_FourthBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(28, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[26] = test;

            Assert.AreEqual(26, array.SIMD_IndexOf(test));

            array.Dispose();
        }

        [Test]
        public static void Short_Direct_SixthBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(30, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[28] = test;

            Assert.AreEqual(28, array.SIMD_IndexOf(test));

            array[28] = 200;

            array[29] = test;

            Assert.AreEqual(29, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Direct_SeventhBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(31, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[30] = test;

            Assert.AreEqual(30, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Direct_FirstBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Direct_SecondBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Direct_ThirdBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(12, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[10] = test;

            Assert.AreEqual(10, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Direct_SixthBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(14, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[12] = test;

            Assert.AreEqual(12, array.SIMD_IndexOf(test));

            array[12] = 200;

            array[13] = test;

            Assert.AreEqual(13, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Direct_SeventhBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(15, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[14] = test;

            Assert.AreEqual(14, array.SIMD_IndexOf(test));

            array.Dispose();
        }

        [Test]
        public static void Long_Direct_FirstBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[1] = test;

            Assert.AreEqual(1, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Direct_SecondBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Direct_SixthBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(6, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[4] = test;

            Assert.AreEqual(4, array.SIMD_IndexOf(test));

            array[4] = 200;

            array[5] = test;

            Assert.AreEqual(5, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Direct_SeventhBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(7, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Float_Direct_FirstBranch()
        {
            float test = 122;

            NativeArray<float> array = new NativeArray<float>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Float_Direct_SecondBranch()
        {
            float test = 122;

            NativeArray<float> array = new NativeArray<float>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Float_Direct_ThirdBranch()
        {
            float test = 122;

            NativeArray<float> array = new NativeArray<float>(12, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[10] = test;

            Assert.AreEqual(10, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Float_Direct_SixthBranch()
        {
            float test = 122;

            NativeArray<float> array = new NativeArray<float>(14, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[12] = test;

            Assert.AreEqual(12, array.SIMD_IndexOf(test));

            array[12] = 200;

            array[13] = test;

            Assert.AreEqual(13, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Float_Direct_SeventhBranch()
        {
            float test = 122;

            NativeArray<float> array = new NativeArray<float>(15, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[14] = test;

            Assert.AreEqual(14, array.SIMD_IndexOf(test));

            array.Dispose();
        }

        [Test]
        public static void Double_Direct_FirstBranch()
        {
            double test = 122;

            NativeArray<double> array = new NativeArray<double>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[1] = test;

            Assert.AreEqual(1, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Double_Direct_SecondBranch()
        {
            double test = 122;

            NativeArray<double> array = new NativeArray<double>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[2] = test;

            Assert.AreEqual(2, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Double_Direct_SixthBranch()
        {
            double test = 122;

            NativeArray<double> array = new NativeArray<double>(6, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[4] = test;

            Assert.AreEqual(4, array.SIMD_IndexOf(test));

            array[4] = 200;

            array[5] = test;

            Assert.AreEqual(5, array.SIMD_IndexOf(test));

            array.Dispose();
        }
        
        [Test]
        public static void Double_Direct_SeventhBranch()
        {
            double test = 122;

            NativeArray<double> array = new NativeArray<double>(7, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = 200;
            }

            array[6] = test;

            Assert.AreEqual(6, array.SIMD_IndexOf(test));

            array.Dispose();
        }


        [Test]
        public static void Byte_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }


        [Test]
        public static void UShort_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }
        
        [Test]
        public static void UInt_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }
        
        [Test]
        public static void ULong_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }
        
        [Test]
        public static void SByte_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        
        [Test]
        public static void Short_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }
        
        [Test]
        public static void Int_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }
        
        [Test]
        public static void Long_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }
        
        [Test]
        public static void Float_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }
        
        [Test]
        public static void Double_Equal()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= array[k] != test;
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] == test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= array[(int)k] != test;
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region NOT_EQUAL
        [Test]
        public static void Byte_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                byte test = 47;
                
                NativeArray<byte> array = new NativeArray<byte>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void UShort_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                ushort test = 47;
                
                NativeArray<ushort> array = new NativeArray<ushort>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void UInt_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                uint test = 47;
                
                NativeArray<uint> array = new NativeArray<uint>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void ULong_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                ulong test = 47;
                
                NativeArray<ulong> array = new NativeArray<ulong>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void SByte_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                sbyte test = 47;
                
                NativeArray<sbyte> array = new NativeArray<sbyte>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void Short_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                short test = 47;
                
                NativeArray<short> array = new NativeArray<short>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void Int_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                int test = 47;
                
                NativeArray<int> array = new NativeArray<int>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }

        [Test]
        public static void Long_Indirect_EachIndex()
        {
            for (int i = 0;i < 64;i++)
            {
                long test = 47;
                
                NativeArray<long> array = new NativeArray<long>(64, Allocator.Temp);
                for (int j = 0;j < array.Length;j++)
                {
                    array[j] = test;
                }

                array[i] = 100;
                
                Assert.AreEqual(i, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

                array.Dispose();
            }
        }


        [Test]
        public static void Byte_Indirect_FirstBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[13] = 200;

            Assert.AreEqual(13, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_SecondBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[23] = 200;

            Assert.AreEqual(23, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_ThirdBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(48, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[40] = 200;

            Assert.AreEqual(40, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_FourthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(56, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[52] = 200;

            Assert.AreEqual(52, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_SixthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(60, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[58] = 200;

            Assert.AreEqual(58, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_SeventhBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(62, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[60] = 200;

            Assert.AreEqual(60, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[60] = test;

            array[61] = 200;

            Assert.AreEqual(61, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Byte_Indirect_EighthBranch()
        {
            byte test = 122;

            NativeArray<byte> array = new NativeArray<byte>(63, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[62] = 200;

            Assert.AreEqual(62, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void UShort_Indirect_FirstBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Indirect_SecondBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[14] = 200;

            Assert.AreEqual(14, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Indirect_ThirdBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(24, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[22] = 200;

            Assert.AreEqual(22, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Indirect_FourthBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(28, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[26] = 200;

            Assert.AreEqual(26, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void UShort_Indirect_SixthBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(30, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[28] = 200;

            Assert.AreEqual(28, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[28] = test;

            array[29] = 200;

            Assert.AreEqual(29, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UShort_Indirect_SeventhBranch()
        {
            ushort test = 122;

            NativeArray<ushort> array = new NativeArray<ushort>(31, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[30] = 200;

            Assert.AreEqual(30, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void UInt_Indirect_FirstBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[2] = 200;

            Assert.AreEqual(2, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Indirect_SecondBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Indirect_ThirdBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(12, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[10] = 200;

            Assert.AreEqual(10, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Indirect_SixthBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(14, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[12] = 200;

            Assert.AreEqual(12, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[12] = test;

            array[13] = 200;

            Assert.AreEqual(13, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void UInt_Indirect_SeventhBranch()
        {
            uint test = 122;

            NativeArray<uint> array = new NativeArray<uint>(15, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[14] = 200;

            Assert.AreEqual(14, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void ULong_Indirect_FirstBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[1] = 200;

            Assert.AreEqual(1, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Indirect_SecondBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[2] = 200;

            Assert.AreEqual(2, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Indirect_SixthBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(6, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[4] = 200;

            Assert.AreEqual(4, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[4] = test;

            array[5] = 200;

            Assert.AreEqual(5, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void ULong_Indirect_SeventhBranch()
        {
            ulong test = 122;

            NativeArray<ulong> array = new NativeArray<ulong>(7, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void SByte_Indirect_FirstBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[13] = 47;

            Assert.AreEqual(13, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_SecondBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(32, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[23] = 47;

            Assert.AreEqual(23, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_ThirdBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(48, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[40] = 47;

            Assert.AreEqual(40, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_FourthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(56, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[52] = 47;

            Assert.AreEqual(52, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_SixthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(60, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[58] = 47;

            Assert.AreEqual(58, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_SeventhBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(62, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[60] = 47;

            Assert.AreEqual(60, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[60] = test;

            array[61] = 47;

            Assert.AreEqual(61, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void SByte_Indirect_EighthBranch()
        {
            sbyte test = 122;

            NativeArray<sbyte> array = new NativeArray<sbyte>(63, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[62] = 47;

            Assert.AreEqual(62, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void Short_Indirect_FirstBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Indirect_SecondBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(16, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[14] = 200;

            Assert.AreEqual(14, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Indirect_ThirdBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(24, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[22] = 200;

            Assert.AreEqual(22, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Indirect_FourthBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(28, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[26] = 200;

            Assert.AreEqual(26, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void Short_Indirect_SixthBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(30, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[28] = 200;

            Assert.AreEqual(28, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[28] = test;

            array[29] = 200;

            Assert.AreEqual(29, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Short_Indirect_SeventhBranch()
        {
            short test = 122;

            NativeArray<short> array = new NativeArray<short>(31, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[30] = 200;

            Assert.AreEqual(30, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void Int_Indirect_FirstBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[2] = 200;

            Assert.AreEqual(2, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Indirect_SecondBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(8, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Indirect_ThirdBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(12, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[10] = 200;

            Assert.AreEqual(10, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Indirect_SixthBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(14, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[12] = 200;

            Assert.AreEqual(12, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[12] = test;

            array[13] = 200;

            Assert.AreEqual(13, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Int_Indirect_SeventhBranch()
        {
            int test = 122;

            NativeArray<int> array = new NativeArray<int>(15, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[14] = 200;

            Assert.AreEqual(14, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void Long_Indirect_FirstBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[1] = 200;

            Assert.AreEqual(1, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Indirect_SecondBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(4, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[2] = 200;

            Assert.AreEqual(2, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Indirect_SixthBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(6, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[4] = 200;

            Assert.AreEqual(4, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array[4] = test;

            array[5] = 200;

            Assert.AreEqual(5, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }
        
        [Test]
        public static void Long_Indirect_SeventhBranch()
        {
            long test = 122;

            NativeArray<long> array = new NativeArray<long>(7, Allocator.Temp);
            for (int i = 0;i < array.Length;i++)
            {
                array[i] = test;
            }

            array[6] = 200;

            Assert.AreEqual(6, array.SIMD_IndexOf(test, Comparison.NotEqualTo));

            array.Dispose();
        }

        [Test]
        public static void Byte_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }

        [Test]
        public static void UShort_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test]
        public static void UInt_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }

        [Test]
        public static void ULong_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test]
        public static void SByte_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test]
        public static void Short_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test]
        public static void Int_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }

        [Test]
        public static void Long_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test]
        public static void Float_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }

        [Test]
        public static void Double_NotEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test, Comparison.NotEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] != test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] != test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] != test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion
        
        #region GREATER
        [Test]
        public static void Byte_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }

        [Test]
        public static void UShort_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test]
        public static void UInt_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }

        [Test]
        public static void ULong_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test]
        public static void SByte_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test]
        public static void Short_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test]
        public static void Int_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }

        [Test]
        public static void Long_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test]
        public static void Float_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }

        [Test]
        public static void Double_Greater()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] > test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] > test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] > test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion
        
        #region LESS
        [Test]
        public static void Byte_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }

        [Test]
        public static void UShort_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test]
        public static void UInt_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }

        [Test]
        public static void ULong_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test]
        public static void SByte_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test]
        public static void Short_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test]
        public static void Int_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }

        [Test]
        public static void Long_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test]
        public static void Float_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }

        [Test]
        public static void Double_Less()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThan);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] < test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] < test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] < test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region GREATER_EQUAL
        [Test]
        public static void Byte_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }

        [Test]
        public static void UShort_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test]
        public static void UInt_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }

        [Test]
        public static void ULong_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test]
        public static void SByte_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test]
        public static void Short_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test]
        public static void Int_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }

        [Test]
        public static void Long_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test]
        public static void Float_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }

        [Test]
        public static void Double_GreaterEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test, Comparison.GreaterThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] >= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] >= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] >= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion

        #region LESS_EQUAL
        [Test]
        public static void Byte_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<byte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    byte test = (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (byte)rng.NextInt(byte.MinValue, byte.MaxValue + 1),
            1000);
        }

        [Test]
        public static void UShort_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ushort>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    ushort test = (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (ushort)rng.NextInt(ushort.MinValue, ushort.MaxValue + 1),
            100000);
        }

        [Test]
        public static void UInt_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<uint>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    uint test = rng.NextUInt();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextUInt);
        }

        [Test]
        public static void ULong_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<ulong>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    ulong test = rng.NextUInt() | ((ulong)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((ulong)rng.NextUInt() << 32));
        }

        [Test]
        public static void SByte_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<sbyte>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    sbyte test = (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (sbyte)rng.NextInt(sbyte.MinValue, sbyte.MaxValue + 1));
        }

        [Test]
        public static void Short_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<short>(
            (array) =>
            {
                for (int j = 0; j < 10; j++)
                {
                    short test = (short)rng.NextInt(short.MinValue, short.MaxValue + 1);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => (short)rng.NextInt(short.MinValue, short.MaxValue + 1));
        }

        [Test]
        public static void Int_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<int>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    int test = rng.NextInt();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextInt);
        }

        [Test]
        public static void Long_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<long>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    long test = rng.NextUInt() | ((long)rng.NextUInt() << 32);

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            () => rng.NextUInt() | ((long)rng.NextUInt() << 32));
        }

        [Test]
        public static void Float_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<float>(
            (array) =>
            {
                for (int j = 0; j < 50; j++)
                {
                    float test = rng.NextFloat();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;

                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextFloat);
        }

        [Test]
        public static void Double_LessEqual()
        {
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(Helpers.GetRngSeed);

            Helpers.Test<double>(
            (array) =>
            {
                for (int j = 0; j < 100; j++)
                {
                    double test = rng.NextDouble();

                    long index = array.SIMD_IndexOf(test, Comparison.LessThanOrEqualTo);

                    if (index == -1)
                    {
                        bool noOccurrence = true;

                        for (int k = 0; k < array.Length; k++)
                        {
                            noOccurrence &= !(array[k] <= test);
                        }

                        Assert.IsTrue(noOccurrence);
                    }
                    else
                    {
                        bool isCorrectIndex = array[(int)index] <= test;
                        bool noPreviousOccurrence = true;
                        
                        for (long k = 0; k < index; k++)
                        {
                            noPreviousOccurrence &= !(array[(int)k] <= test);
                        }

                        Assert.IsTrue(isCorrectIndex);
                        Assert.IsTrue(noPreviousOccurrence);
                    }
                }
            },
            rng.NextDouble);
        }
        #endregion
    }
}