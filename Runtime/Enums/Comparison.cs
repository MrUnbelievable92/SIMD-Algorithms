using System.Runtime.CompilerServices;
using Unity.Mathematics;
using DevTools;
using MaxMath;

namespace SIMDAlgorithms
{
    public enum Comparison : byte
    {
        /// <summary>       arrayElement == value     </summary>
        EqualTo,
        /// <summary>       arrayElement != value     </summary>
        NotEqualTo,

        /// <summary>       arrayElement > value     </summary>
        GreaterThan,
        /// <summary>       arrayElement < value     </summary>
        LessThan,

        /// <summary>       arrayElement >= value     </summary>
        GreaterThanOrEqualTo,
        /// <summary>       arrayElement <= value     </summary>
        LessThanOrEqualTo
    }

    public static class ComparisonExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, byte a, byte b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, sbyte a, sbyte b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, ushort a, ushort b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, short a, short b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, uint a, uint b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, int a, int b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, ulong a, ulong b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(this Comparison c, long a, long b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, byte2 a, byte2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, sbyte2 a, sbyte2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, byte3 a, byte3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, sbyte3 a, sbyte3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, byte4 a, byte4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, sbyte4 a, sbyte4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, byte8 a, byte8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, sbyte8 a, sbyte8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool16 Compare(this Comparison c, byte16 a, byte16 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool16 Compare(this Comparison c, sbyte16 a, sbyte16 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool32 Compare(this Comparison c, byte32 a, byte32 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool32 Compare(this Comparison c, sbyte32 a, sbyte32 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, ushort2 a, ushort2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, short2 a, short2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, ushort3 a, ushort3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, short3 a, short3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, ushort4 a, ushort4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, short4 a, short4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, ushort8 a, ushort8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, short8 a, short8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool16 Compare(this Comparison c, ushort16 a, ushort16 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool16 Compare(this Comparison c, short16 a, short16 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, uint2 a, uint2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, int2 a, int2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, uint3 a, uint3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, int3 a, int3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, uint4 a, uint4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, int4 a, int4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, uint8 a, uint8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool8 Compare(this Comparison c, int8 a, int8 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, ulong2 a, ulong2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 Compare(this Comparison c, long2 a, long2 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, ulong3 a, ulong3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 Compare(this Comparison c, long3 a, long3 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, ulong4 a, ulong4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 Compare(this Comparison c, long4 a, long4 b)
        {
            switch (c)
            {
                case Comparison.EqualTo:                return a == b;
                case Comparison.NotEqualTo:             return a != b;
                case Comparison.GreaterThan:            return a > b;
                case Comparison.LessThan:               return a < b;
                case Comparison.GreaterThanOrEqualTo:   return a >= b;
                case Comparison.LessThanOrEqualTo:      return a <= b;
                
                default: throw Assert.Unreachable();
            }
        }
    }
}