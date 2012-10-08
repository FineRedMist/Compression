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
    /// Zero order dynamic model of the byte distribution.
    /// </summary>
    public class ModelOrder0C : ModelI
    {
        private readonly uint[] _charFrequency = new uint[257];
        private uint _totalFrequencies;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelOrder0C()
        {
            ResetModel();
        }

        private void ResetModel()
        {
            // initialize probabilities with 1
            _totalFrequencies = 257; // 256 + escape symbol for termination
            for (uint i = 0; i < 257; i++)
                _charFrequency[i] = 1;
        }

        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols.
        /// </summary>
        protected override void Encode(uint symbolCount)
        {
            for (uint i = 0; i < symbolCount; ++i)
            {
                byte symbol = SymbolReader();

                // cumulate frequencies
                uint lowCount = 0;
                byte j = 0;
                for (; j < symbol; j++)
                {
                    lowCount += _charFrequency[j];
                }

                // encode symbol
                Coder.Encode(lowCount, lowCount + _charFrequency[j], _totalFrequencies, BitWriter);

                // update model
                _charFrequency[symbol]++;
                _totalFrequencies++;

                if (_totalFrequencies >= (1 << 29))
                {
                    ResetModel();
                }
            }

            // write escape symbol for termination
            Coder.Encode(_totalFrequencies - 1, _totalFrequencies, _totalFrequencies, BitWriter);
        }

        /// <summary>
        /// Decodes.
        /// </summary>
        protected override void Decode()
        {
            uint symbol;

            do
            {
                uint value;

                // read value
                value = Coder.DecodeTarget(_totalFrequencies);

                uint lowCount = 0;

                // determine symbol
                for (symbol = 0; lowCount + _charFrequency[symbol] <= value; symbol++)
                {
                    lowCount += _charFrequency[symbol];
                }

                // write symbol
                if (symbol < 256)
                {
                    SymbolWriter((byte) symbol);
                }
                else
                {
                    break;
                }

                // adapt decoder
                Coder.Decode(lowCount, lowCount + _charFrequency[symbol], BitReader);

                // update model
                _charFrequency[symbol]++;
                _totalFrequencies++;
                if (_totalFrequencies >= (1 << 29))
                {
                    ResetModel();
                }
            } while (symbol != 256); // until termination symbol read
        }
    };
}