using Unity.Burst.Intrinsics;
using MaxMath.Intrinsics;

using static Unity.Burst.Intrinsics.X86;
using MaxMath;

namespace SIMDAlgorithms
{
    internal static class Compare
    {
        #region byte
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v256 Bytes256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi8(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi8(left, right);
                    case Comparison.GreaterThan:            return Xse.mm256_cmpgt_epu8(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epu8(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.mm256_cmpge_epu8(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.mm256_cmple_epu8(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 Bytes128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi8(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi8(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epu8(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epu8(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.cmpge_epu8(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.cmple_epu8(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool Bytes(byte left, byte right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region ushort
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v256 UShorts256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi16(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi16(left, right);
                    case Comparison.GreaterThan:            return Xse.mm256_cmpgt_epu16(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epu16(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.mm256_cmpge_epu16(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.mm256_cmple_epu16(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 UShorts128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi16(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi16(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epu16(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epu16(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.cmpge_epu16(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.cmple_epu16(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool UShorts(ushort left, ushort right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region uint
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v256 UInts256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi32(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi32(left, right);
                    case Comparison.GreaterThan:            return Xse.mm256_cmpgt_epu32(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epu32(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.mm256_cmpge_epu32(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.mm256_cmple_epu32(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Sse4_1: Comparison.NotEqualTo; Xse: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 UInts128(v128 left, v128 right, Comparison where)
        {
            if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi32(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi32(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epu32(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epu32(left, right);
                    case Comparison.GreaterThanOrEqualTo:   return Xse.cmpge_epu32(left, right);
                    case Comparison.LessThanOrEqualTo:      return Xse.cmple_epu32(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi32(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi32(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epu32(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epu32(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool UInts(uint left, uint right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region ulong
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v256 ULongs256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi64(left, right);
                    case Comparison.GreaterThan:            return Xse.mm256_cmpgt_epu64(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmpgt_epu64(right, left);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 ULongs128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsCMP64Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi64(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epu64(left, right);
                    case Comparison.LessThan:               return Xse.cmpgt_epu64(right, left);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:        return Xse.cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:     return Xse.cmpeq_epi64(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool ULongs(ulong left, ulong right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region sbyte
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v256 SBytes256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi8(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi8(left, right);
                    case Comparison.GreaterThan:            return Avx2.mm256_cmpgt_epi8(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epi8(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 SBytes128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi8(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi8(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epi8(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epi8(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool SBytes(sbyte left, sbyte right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region short
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v256 Shorts256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi16(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi16(left, right);
                    case Comparison.GreaterThan:            return Avx2.mm256_cmpgt_epi16(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epi16(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Shorts128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi16(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi16(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epi16(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epi16(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool Shorts(short left, short right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region int
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v256 Ints256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi32(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi32(left, right);
                    case Comparison.GreaterThan:            return Avx2.mm256_cmpgt_epi32(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epi32(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Ints128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi32(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi32(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epi32(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epi32(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool Ints(int left, int right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region long
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v256 Longs256(v256 left, v256 right, Comparison where)
        {
            if (Avx2.IsAvx2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Avx2.mm256_cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:             return Avx2.mm256_cmpeq_epi64(left, right);
                    case Comparison.GreaterThan:            return Xse.mm256_cmpgt_epi64(left, right);
                    case Comparison.LessThan:               return Xse.mm256_cmplt_epi64(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Longs128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsCMP64Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi64(left, right);
                    case Comparison.GreaterThan:            return Xse.cmpgt_epi64(left, right);
                    case Comparison.LessThan:               return Xse.cmplt_epi64(left, right);
                    case Comparison.GreaterThanOrEqualTo:   goto case Comparison.LessThan;
                    case Comparison.LessThanOrEqualTo:      goto case Comparison.GreaterThan;

                    default: throw new IllegalInstructionException();
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:                return Xse.cmpeq_epi64(left, right);
                    case Comparison.NotEqualTo:             return Xse.cmpeq_epi64(left, right);

                    default: throw new IllegalInstructionException();
                }
            }
            else throw new IllegalInstructionException();
        }

        public static bool Longs(long left, long right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region float
        public static v256 Floats256(v256 left, v256 right, Comparison where)
        {
            if (Avx.IsAvxSupported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.EQ_OQ),
                    Comparison.NotEqualTo           => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.NEQ_OQ),
                    Comparison.GreaterThan          => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.GT_OQ),
                    Comparison.LessThan             => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.LT_OQ),
                    Comparison.GreaterThanOrEqualTo => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.GE_OQ),
                    Comparison.LessThanOrEqualTo    => Avx.mm256_cmp_ps(left, right, (int)Avx.CMP.LE_OQ),

                    _ => throw new IllegalInstructionException()
                };
            }
            else throw new IllegalInstructionException();
        }

        public static v128 Floats128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Xse.cmpeq_ps(left, right),
                    Comparison.NotEqualTo           => Xse.cmpneq_ps(left, right),
                    Comparison.GreaterThan          => Xse.cmpgt_ps(left, right),
                    Comparison.LessThan             => Xse.cmplt_ps(left, right),
                    Comparison.GreaterThanOrEqualTo => Xse.cmpge_ps(left, right),
                    Comparison.LessThanOrEqualTo    => Xse.cmple_ps(left, right),

                    _ => throw new IllegalInstructionException()
                };
            }
            else throw new IllegalInstructionException();
        }

        public static bool Floats(float left, float right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion

        #region double
        public static v256 Doubles256(v256 left, v256 right, Comparison where)
        {
            if (Avx.IsAvxSupported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.EQ_OQ),
                    Comparison.NotEqualTo           => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.NEQ_OQ),
                    Comparison.GreaterThan          => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.GT_OQ),
                    Comparison.LessThan             => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.LT_OQ),
                    Comparison.GreaterThanOrEqualTo => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.GE_OQ),
                    Comparison.LessThanOrEqualTo    => Avx.mm256_cmp_pd(left, right, (int)Avx.CMP.LE_OQ),

                    _ => throw new IllegalInstructionException()
                };
            }
            else throw new IllegalInstructionException();
        }

        public static v128 Doubles128(v128 left, v128 right, Comparison where)
        {
            if (BurstArchitecture.IsSIMDSupported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Xse.cmpeq_pd(left, right),
                    Comparison.NotEqualTo           => Xse.cmpneq_pd(left, right),
                    Comparison.GreaterThan          => Xse.cmpgt_pd(left, right),
                    Comparison.LessThan             => Xse.cmplt_pd(left, right),
                    Comparison.GreaterThanOrEqualTo => Xse.cmpge_pd(left, right),
                    Comparison.LessThanOrEqualTo    => Xse.cmple_pd(left, right),

                    _ => throw new IllegalInstructionException()
                };
            }
            else throw new IllegalInstructionException();
        }

        public static bool Doubles(double left, double right, Comparison where)
        {
            return where switch
            {
                Comparison.EqualTo              => left == right,
                Comparison.NotEqualTo           => left != right,
                Comparison.GreaterThan          => left > right,
                Comparison.LessThan             => left < right,
                Comparison.GreaterThanOrEqualTo => left >= right,
                Comparison.LessThanOrEqualTo    => left <= right,

                _ => throw new IllegalInstructionException()
            };
        }
        #endregion
    }
}