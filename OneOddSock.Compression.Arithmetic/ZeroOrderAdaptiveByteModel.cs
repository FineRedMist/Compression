using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Sample model for demonstration purposes.
    /// Zero-order adaptive byte based model using a value of 256 as the terminator.
    /// </summary>
    public class ZeroOrderAdaptiveByteModel : IModel<uint>
    {
        /// <summary>
        /// The total of frequencies for all symbols represented by the model.
        /// </summary>
        public uint TotalFrequencies { get; private set; }

        private readonly uint[] _charFrequency = new uint[257];

        /// <summary>
        /// Constructor.
        /// </summary>
        public ZeroOrderAdaptiveByteModel()
        {
            Reset();
        }

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
            return new Range() {Low = low, High = low + _charFrequency[j]};
        }

        /// <summary>
        /// Update the frequency information of <paramref name="symbol"/>.
        /// </summary>
        public void Update(uint symbol)
        {
            _charFrequency[symbol]++;
            TotalFrequencies++;

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
            return new RangeSymbol<uint>()
                       {
                           Range = new Range()
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
            TotalFrequencies = 257; // 256 + escape symbol for termination
            for (uint i = 0; i < 257; i++)
                _charFrequency[i] = 1;
        }
    }
}
