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
    /// Defines the methods required to represent a model for use with arithmetic coding.
    /// </summary>
    public interface IModel<TSymbolType>
        where TSymbolType : struct
    {
        /// <summary>
        /// The total of frequencies for all symbols represented by the model.
        /// </summary>
        uint TotalFrequencies { get; }

        /// <summary>
        /// The [Low, High) range covered by <paramref name="symbol"/>.
        /// </summary>
        Range GetRange(TSymbolType symbol);

        /// <summary>
        /// Update the frequency information of <paramref name="symbol"/>.
        /// </summary>
        void Update(TSymbolType symbol);

        /// <summary>
        /// Finds the symbol for the corresponding <paramref name="value"/>.
        /// </summary>
        RangeSymbol<TSymbolType> Decode(uint value);

        /// <summary>
        /// Resets the model statistics.
        /// </summary>
        void Reset();
    }
}