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

 /* This file is entirely for providing functionality for testing.
 * This content will be moved out once other examples have been 
 * created that don't rely on this level of specificity.
 */

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Sample model for demonstration purposes.
    /// Zero-order adaptive byte based model using a value of 256 as the terminator.
    /// </summary>
    public class ZeroOrderAdaptiveByteModel : IModel<uint>
    {
        private readonly uint[] _charFrequency = new uint[257];

        /// <summary>
        /// Constructor.
        /// </summary>
        public ZeroOrderAdaptiveByteModel()
        {
            Reset();
        }

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
            TotalFrequencies = 257; // 256 + escape symbol for termination
            for (uint i = 0; i < 257; i++)
                _charFrequency[i] = 1;
        }

        #endregion
    }

    /// <summary>
    /// Extensions to the ArithmeticCoding class for testing purposes.
    /// </summary>
    public static class ArithmeticCodingTestExtensions
    {
        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols read from <paramref name="symbolReader"/>
        /// writing encoded bits to <paramref name="bitWriter"/>.
        /// </summary>
        public static void Encode(this ArithmeticCoder coder, IModel<uint> model, WriteBitDelegate bitWriter,
                                  ReadSymbolDelegate<byte> symbolReader, uint symbolCount)
        {
            for (uint i = 0; i < symbolCount; ++i)
            {
                byte symbol = symbolReader();

                coder.Encode(symbol, model, bitWriter);
            }

            // write escape symbol for termination
            coder.Encode((uint) 256, model, bitWriter);
            coder.EncodeFinish(bitWriter);
        }

        /// <summary>
        /// Decodes symbols from <paramref name="coder"/> using the provided <paramref name="model"/>.
        /// </summary>
        public static void Decode(this ArithmeticCoder coder, IModel<uint> model, WriteSymbolDelegate<byte> symbolWriter,
                                  ReadBitDelegate bitReader)
        {
            coder.DecodeStart(bitReader);

            uint symbol;

            do
            {
                symbol = coder.Decode(model, bitReader);
                if (symbol != 256)
                {
                    symbolWriter((byte) symbol);
                }
            } while (symbol != 256); // until termination symbol read
        }
    }
}