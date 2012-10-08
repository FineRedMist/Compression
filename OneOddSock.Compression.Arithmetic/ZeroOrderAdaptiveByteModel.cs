using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Zero-order adaptive byte based model.
    /// </summary>
    internal class ZeroOrderAdaptiveByteModel : IModel<uint>
    {
        private readonly uint[] _charFrequency = new uint[258];

        /// <summary>
        /// Constructor.
        /// </summary>
        public ZeroOrderAdaptiveByteModel()
        {
            Reset();
        }

        /// <summary>
        /// New character indicator in the stream.
        /// </summary>
        public const uint NewCharacter = 256;
        /// <summary>
        /// Stream terminator to end processing.
        /// </summary>
        public const uint StreamTerminator = 257;

        #region IModel<uint> Members

        /// <summary>
        /// The total of frequencies for all symbols represented by the model.
        /// </summary>
        public uint TotalFrequencies { get; private set; }

        /// <summary>
        /// The [Low, High) range covered by <paramref name="symbol"/>.
        /// </summary>
        public Range GetRange(uint symbol)
        {
            // cumulate frequencies
            uint low = 0;
            int j = 0;
            for (; j < symbol; j++)
            {
                low += _charFrequency[j];
            }
            return new Range {Low = low, High = low + _charFrequency[j]};
        }

        /// <summary>
        /// Update the frequency information of <paramref name="symbol"/>.
        /// </summary>
        public void Update(uint symbol)
        {
            _charFrequency[symbol]++;
            TotalFrequencies++;

            if (symbol != NewCharacter 
                && _charFrequency[symbol] > 1
                && _charFrequency[NewCharacter] > 1)
            {
                _charFrequency[NewCharacter]--;
                TotalFrequencies--;
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
            uint low = 0;
            uint symbol = 0;
            for (; low + _charFrequency[symbol] <= value; symbol++)
            {
                low += _charFrequency[symbol];
            }
            return new RangeSymbol<uint>
                       {
                           Range = new Range
                                       {
                                           Low = low,
                                           High = low + _charFrequency[symbol]
                                       },
                           Symbol = symbol
                       };
        }

        /// <summary>
        /// Resets the model statistics.
        /// </summary>
        public void Reset()
        {
            // initialize probabilities with 1
            for (uint i = 0; i < _charFrequency.Length - 2; i++)
                _charFrequency[i] = 0;
            _charFrequency[NewCharacter] = 10;
            _charFrequency[StreamTerminator] = 1;
            TotalFrequencies = _charFrequency[NewCharacter] + _charFrequency[StreamTerminator];
        }

        #endregion
    }
}
