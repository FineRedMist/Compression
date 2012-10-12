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

using System.Collections.Generic;

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Zero-order adaptive byte based model.
    /// </summary>
    internal class ZeroOrderAdaptiveByteModel : IModel<uint>
    {
        /// <summary>
        /// New character indicator in the stream.
        /// </summary>
        public const uint NewCharacter = 256;

        /// <summary>
        /// Stream terminator to end processing.
        /// </summary>
        public const uint StreamTerminator = 257;

        private readonly IEnumerable<KeyValuePair<uint, uint>> _initialWeights = new[]
                                                                                     {
                                                                                         new KeyValuePair<uint, uint>(
                                                                                             NewCharacter, 10),
                                                                                         new KeyValuePair<uint, uint>(
                                                                                             StreamTerminator, 1)
                                                                                     };

        private readonly PartialSumTreeFixedSize _stats;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ZeroOrderAdaptiveByteModel()
        {
            _stats = new PartialSumTreeFixedSize(257, _initialWeights);
        }

        #region IModel<uint> Members

        /// <summary>
        /// The total of frequencies for all symbols represented by the model.
        /// </summary>
        public uint TotalFrequencies
        {
            get { return _stats.TotalWeight; }
        }

        /// <summary>
        /// The [Low, High) range covered by <paramref name="symbol"/>.
        /// </summary>
        public Range GetRange(uint symbol)
        {
            return _stats[symbol];
        }

        /// <summary>
        /// Update the frequency information of <paramref name="symbol"/>.
        /// </summary>
        public void Update(uint symbol)
        {
            _stats.UpdateWeight(symbol, 1);

            if (symbol != NewCharacter
                && _stats.GetWeight(symbol) > 1
                && _stats.GetWeight(NewCharacter) > 1)
            {
                _stats.UpdateWeight(NewCharacter, -1);
            }

            if (TotalFrequencies >= (1 << 29))
            {
                Reset();
            }
        }

        /// <summary>
        /// Finds the symbol for the corresponding <paramref name="value"/>.
        /// </summary>
        public RangeSymbol<uint> Decode(uint value)
        {
            uint symbol = _stats.GetSymbol(value);

            return new RangeSymbol<uint>
                       {
                           Range = _stats[symbol],
                           Symbol = symbol
                       };
        }

        /// <summary>
        /// Resets the model statistics.
        /// </summary>
        public void Reset()
        {
            _stats.Reset();
            _stats.UpdateWeights(_initialWeights);
        }

        #endregion
    }
}