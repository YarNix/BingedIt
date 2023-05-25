using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BingedIt.Common
{
    internal static class EnumHelper
    {
        public static TEnum GetHighestBit<TEnum, TInteger>(this TEnum enumValue) where TEnum : struct, Enum
                                                                                 where TInteger : struct, IComparable, IConvertible, ISpanFormattable, IComparable<TInteger>, IEquatable<TInteger>, IBinaryInteger<TInteger>
        {
            TInteger leadZero = TInteger.LeadingZeroCount(Unsafe.As<TEnum, TInteger>(ref enumValue));
            int shiftAmount = (Marshal.SizeOf<TInteger>() * 8) - Unsafe.As<TInteger, int>(ref leadZero) - 1;
            TInteger value = TInteger.One;
            if (shiftAmount != 0)
                value = int.IsPositive(shiftAmount) ? value << shiftAmount : value >> int.Abs(shiftAmount);
            return Unsafe.As<TInteger, TEnum>(ref value);
        }
        public static TEnum GetLowestBit<TEnum, TInteger>(this TEnum enumValue) where TEnum : struct, Enum
                                                                             where TInteger : struct, IComparable, IConvertible, ISpanFormattable, IComparable<TInteger>, IEquatable<TInteger>, IBinaryInteger<TInteger>
        {
            TInteger trailZero = TInteger.TrailingZeroCount(Unsafe.As<TEnum, TInteger>(ref enumValue));
            int bitCount = Marshal.SizeOf<TInteger>() * 8;
            int shiftCount = bitCount - Unsafe.As<TInteger, int>(ref trailZero);
            TInteger value = shiftCount == bitCount ? TInteger.Zero : TInteger.One << shiftCount;
            string param = Convert.ToString(Unsafe.As<TEnum, long>(ref enumValue), 2),
                   ret = Convert.ToString(Unsafe.As<TInteger, long>(ref value), 2);
            return Unsafe.As<TInteger, TEnum>(ref value);
        }
    }
}
