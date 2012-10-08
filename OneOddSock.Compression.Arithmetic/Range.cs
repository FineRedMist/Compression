namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Represents a range between to unsigned 32 bit integers.
    /// </summary>
    public struct Range
    {
        /// <summary>
        /// Lower bound of the range.
        /// </summary>
        public uint Low { get; set; }

        /// <summary>
        /// Upper bound of the range.
        /// </summary>
        public uint High { get; set; }

        /// <summary>
        /// Adds <paramref name="value"/> to each element of <paramref name="range"/>.
        /// </summary>
        public static Range operator +(Range range, uint value)
        {
            return new Range {Low = range.Low + value, High = range.High + value};
        }

        /// <summary>
        /// Subtracts <paramref name="value"/> to each element of <paramref name="range"/>.
        /// </summary>
        public static Range operator -(Range range, uint value)
        {
            return new Range {Low = range.Low - value, High = range.High - value};
        }

        /// <summary>
        /// Multiplies <paramref name="value"/> to each element of <paramref name="range"/>.
        /// </summary>
        public static Range operator *(Range range, uint value)
        {
            return new Range {Low = value*range.Low, High = value*range.High};
        }

        /// <summary>
        /// Component-wise add of <paramref name="rhs"/> to <paramref name="lhs"/>.
        /// </summary>
        public static Range operator +(Range lhs, Range rhs)
        {
            return new Range {Low = lhs.Low + rhs.Low, High = lhs.High + rhs.High};
        }

        /// <summary>
        /// Component-wise subtract of <paramref name="rhs"/> from <paramref name="lhs"/>.
        /// </summary>
        public static Range operator -(Range lhs, Range rhs)
        {
            return new Range {Low = lhs.Low - rhs.Low, High = lhs.High - rhs.High};
        }
    }
}