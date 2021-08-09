using Unity.Burst.Intrinsics;

using static Unity.Burst.Intrinsics.X86;

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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi8(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi8(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi8(1 << 7);

                        return Avx2.mm256_cmpgt_epi8(Avx2.mm256_xor_si256(left, MASK), Avx2.mm256_xor_si256(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi8(1 << 7);

                        return Avx2.mm256_cmpgt_epi8(Avx2.mm256_xor_si256(right, MASK), Avx2.mm256_xor_si256(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi8(left, Avx2.mm256_max_epu8(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi8(left, Avx2.mm256_min_epu8(left, right));
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 Bytes128(v128 left, v128 right, Comparison where)
        {
            if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi8(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi8(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi8(unchecked((sbyte)(1 << 7)));

                        return Sse2.cmpgt_epi8(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi8(unchecked((sbyte)(1 << 7)));

                        return Sse2.cmpgt_epi8(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi8(left, Sse2.max_epu8(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi8(left, Sse2.min_epu8(left, right));
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi16(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi16(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi16(unchecked((short)(1 << 15)));

                        return Avx2.mm256_cmpgt_epi16(Avx2.mm256_xor_si256(left, MASK), Avx2.mm256_xor_si256(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi16(unchecked((short)(1 << 15)));

                        return Avx2.mm256_cmpgt_epi16(Avx2.mm256_xor_si256(right, MASK), Avx2.mm256_xor_si256(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi16(left, Avx2.mm256_max_epu16(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi16(left, Avx2.mm256_min_epu16(left, right));
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 UShorts128(v128 left, v128 right, Comparison where)
        {
            if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                        return Sse2.cmpgt_epi16(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                        return Sse2.cmpgt_epi16(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi16(left, Sse4_1.max_epu16(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi16(left, Sse4_1.min_epu16(left, right));
                    }

                    default: return default(v128);
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                        return Sse2.cmpgt_epi16(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi16(unchecked((short)(1 << 15)));

                        return Sse2.cmpgt_epi16(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi16(Sse2.setzero_si128(), Sse2.subs_epu16(right, left));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi16(Sse2.setzero_si128(), Sse2.subs_epu16(left, right));
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi32(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi32(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi32(unchecked((int)(1 << 31)));

                        return Avx2.mm256_cmpgt_epi32(Avx2.mm256_xor_si256(left, MASK), Avx2.mm256_xor_si256(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi32(unchecked((int)(1 << 31)));

                        return Avx2.mm256_cmpgt_epi32(Avx2.mm256_xor_si256(right, MASK), Avx2.mm256_xor_si256(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi32(left, Avx2.mm256_max_epu32(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi32(left, Avx2.mm256_min_epu32(left, right));
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Sse4_1: Comparison.NotEqualTo; Sse2: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 UInts128(v128 left, v128 right, Comparison where)
        {
            if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                        return Sse2.cmpgt_epi32(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                        return Sse2.cmpgt_epi32(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi32(left, Sse4_1.max_epu32(left, right));
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Sse2.cmpeq_epi32(left, Sse4_1.min_epu32(left, right));
                    }

                    default: return default(v128);
                }
            }
            else if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                        return Sse2.cmpgt_epi32(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi32(unchecked((int)(1 << 31)));

                        return Sse2.cmpgt_epi32(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi64(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi64x(unchecked((long)(1ul << 63)));

                        return Avx2.mm256_cmpgt_epi64(Avx2.mm256_xor_si256(left, MASK), Avx2.mm256_xor_si256(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v256 MASK = Avx.mm256_set1_epi64x(unchecked((long)(1ul << 63)));

                        return Avx2.mm256_cmpgt_epi64(Avx2.mm256_xor_si256(right, MASK), Avx2.mm256_xor_si256(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 ULongs128(v128 left, v128 right, Comparison where)
        {
            if (Sse4_2.IsSse42Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        v128 MASK = Sse2.set1_epi64x(unchecked((long)(1ul << 63)));

                        return Sse4_2.cmpgt_epi64(Sse2.xor_si128(left, MASK), Sse2.xor_si128(right, MASK));
                    }
                    case Comparison.LessThan:
                    {
                        v128 MASK = Sse2.set1_epi64x(unchecked((long)(1ul << 63)));

                        return Sse4_2.cmpgt_epi64(Sse2.xor_si128(right, MASK), Sse2.xor_si128(left, MASK));
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v128);
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi8(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi8(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Avx2.mm256_cmpgt_epi8(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Avx2.mm256_cmpgt_epi8(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpgt_epi8(right, left);
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Avx2.mm256_cmpgt_epi8(left, right);
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo         </summary>
        public static v128 SBytes128(v128 left, v128 right, Comparison where)
        {
            if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi8(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi8(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Sse2.cmpgt_epi8(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Sse2.cmpgt_epi8(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        return Sse2.cmpgt_epi8(right, left);
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        return Sse2.cmpgt_epi8(left, right);
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi16(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi16(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Avx2.mm256_cmpgt_epi16(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Avx2.mm256_cmpgt_epi16(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Shorts128(v128 left, v128 right, Comparison where)
        {
            if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi16(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Sse2.cmpgt_epi16(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Sse2.cmpgt_epi16(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan; 
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi32(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi32(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Avx2.mm256_cmpgt_epi32(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Avx2.mm256_cmpgt_epi32(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Ints128(v128 left, v128 right, Comparison where)
        {
            if (Sse2.IsSse2Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse2.cmpeq_epi32(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Sse2.cmpgt_epi32(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Sse2.cmpgt_epi32(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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
                    case Comparison.EqualTo:              
                    {
                        return Avx2.mm256_cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Avx2.mm256_cmpeq_epi64(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Avx2.mm256_cmpgt_epi64(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Avx2.mm256_cmpgt_epi64(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v256);
                }
            }
            else
            {
                return default(v256);
            }
        }
        
        /// <summary>       Inverted for: Comparison.NotEqualTo, Comparison.GreaterThanOrEqualTo, Comparison.LessThanOrEqualTo         </summary>
        public static v128 Longs128(v128 left, v128 right, Comparison where)
        {
            if (Sse4_2.IsSse42Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.GreaterThan:
                    {
                        return Sse4_2.cmpgt_epi64(left, right);
                    }
                    case Comparison.LessThan:
                    {
                        return Sse4_2.cmpgt_epi64(right, left);
                    }
                    case Comparison.GreaterThanOrEqualTo:
                    {
                        goto case Comparison.LessThan;
                    }
                    case Comparison.LessThanOrEqualTo:
                    {
                        goto case Comparison.GreaterThan;
                    }

                    default: return default(v128);
                }
            }
            else if (Sse4_1.IsSse41Supported)
            {
                switch (where)
                {
                    case Comparison.EqualTo:              
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }
                    case Comparison.NotEqualTo:
                    {
                        return Sse4_1.cmpeq_epi64(left, right);
                    }

                    default: return default(v128);
                }
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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

                    _ => default(v256),
                };
            }
            else
            {
                return default(v256);
            }
        }

        public static v128 Floats128(v128 left, v128 right, Comparison where)
        {
            if (Sse.IsSseSupported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Sse.cmpeq_ps(left, right),
                    Comparison.NotEqualTo           => Sse.cmpneq_ps(left, right),
                    Comparison.GreaterThan          => Sse.cmpgt_ps(left, right),
                    Comparison.LessThan             => Sse.cmplt_ps(left, right),
                    Comparison.GreaterThanOrEqualTo => Sse.cmpge_ps(left, right),
                    Comparison.LessThanOrEqualTo    => Sse.cmple_ps(left, right),

                    _ => default(v128),
                };
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
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

                    _ => default(v256),
                };
            }
            else
            {
                return default(v256);
            }
        }

        public static v128 Doubles128(v128 left, v128 right, Comparison where)
        {
            if (Sse2.IsSse2Supported)
            {
                return where switch
                {
                    Comparison.EqualTo              => Sse2.cmpeq_pd(left, right),
                    Comparison.NotEqualTo           => Sse2.cmpneq_pd(left, right),
                    Comparison.GreaterThan          => Sse2.cmpgt_pd(left, right),
                    Comparison.LessThan             => Sse2.cmplt_pd(left, right),
                    Comparison.GreaterThanOrEqualTo => Sse2.cmpge_pd(left, right),
                    Comparison.LessThanOrEqualTo    => Sse2.cmple_pd(left, right),

                    _ => default(v128),
                };
            }
            else
            {
                return default(v128);
            }
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

                _ => default(bool),
            };
        }
        #endregion
    }
}