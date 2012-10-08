using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Extensions for the range class.
    /// </summary>
    public static class RangeExtensions
    {
        /// <summary>
        /// The length between High and Low, inclusive.
        /// </summary>
        public static uint Length(this Range range)
        {
            return range.High - range.Low + 1;
        }

        /// <summary>
        /// Determines if the range is entirely within [lowerBound, upperBound).
        /// </summary>
        public static bool In(this Range range, uint lowerBound, uint upperBound)
        {
            return lowerBound <= range.Low && range.High < upperBound;
        }
    }
}
