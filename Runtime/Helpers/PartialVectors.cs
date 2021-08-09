using Unity.Burst.CompilerServices;

using static Unity.Burst.Intrinsics.X86;

namespace SIMDAlgorithms
{
    internal static class PartialVectors
    {
        public static bool ShouldMask(Comparison where, byte value)
        {
            switch (where)
            {
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value != 0;
                case Comparison.LessThanOrEqualTo:      return true;
                case Comparison.GreaterThan:            return false;
                
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThanOrEqualTo:   return !Constant.IsConstantExpression(value) && value == 0;

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, ushort value)
        {
            switch (where)
            {
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value != 0;
                case Comparison.LessThanOrEqualTo:      return true;
                case Comparison.GreaterThan:            return false;
                
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThanOrEqualTo:   return !Constant.IsConstantExpression(value) && value == 0;

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, uint value)
        {
            switch (where)
            {
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value != 0;
                case Comparison.LessThanOrEqualTo:      return true;
                case Comparison.GreaterThan:            return false;
                
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThanOrEqualTo:   { if (Sse4_1.IsSse41Supported) return !Constant.IsConstantExpression(value) && value == 0; else return true; };

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, sbyte value)
        {
            switch (where)
            {
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThan:            return !Constant.IsConstantExpression(value) && value < 0;
                case Comparison.GreaterThanOrEqualTo:   return true;
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value > 0;
                case Comparison.LessThanOrEqualTo:      return true;

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, short value)
        {
            switch (where)
            {
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThan:            return !Constant.IsConstantExpression(value) && value < 0;
                case Comparison.GreaterThanOrEqualTo:   return true;
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value > 0;
                case Comparison.LessThanOrEqualTo:      return true;

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, int value)
        {
            switch (where)
            {
                case Comparison.NotEqualTo:             return true;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0;
                case Comparison.GreaterThan:            return !Constant.IsConstantExpression(value) && value < 0;
                case Comparison.GreaterThanOrEqualTo:   return true;
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value > 0;
                case Comparison.LessThanOrEqualTo:      return true;

                default: return true;
            }
        }

        public static bool ShouldMask(Comparison where, float value)
        {
            switch (where)
            {
                case Comparison.NotEqualTo:             return !Constant.IsConstantExpression(value) && value != 0f;
                case Comparison.EqualTo:                return !Constant.IsConstantExpression(value) && value == 0f;
                case Comparison.GreaterThan:            return !Constant.IsConstantExpression(value) && value < 0f;
                case Comparison.GreaterThanOrEqualTo:   return !Constant.IsConstantExpression(value) && value <= 0f;
                case Comparison.LessThan:               return !Constant.IsConstantExpression(value) && value > 0f;
                case Comparison.LessThanOrEqualTo:      return !Constant.IsConstantExpression(value) && value >= 0f;

                default: return true;
            }
        }
    }
}