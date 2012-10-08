/*	Copyright 2012 Brent Scriver

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

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