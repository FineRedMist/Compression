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