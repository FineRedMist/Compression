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
    internal class NewCharacterByteModel : IModel<byte>
    {
        private readonly bool[] _pendingBytes = new bool[256];

        public NewCharacterByteModel()
        {
            Reset();
        }

        private uint this[byte index]
        {
            get { return (uint) (_pendingBytes[index] ? 1 : 0); }
        }

        public bool Emitted(byte index)
        {
            return !_pendingBytes[index];
        }

        #region IModel<byte> Members

        public uint TotalFrequencies { get; private set; }

        public Range GetRange(byte symbol)
        {
            // cumulate frequencies
            uint low = 0;
            byte j = 0;
            for (; j < symbol; j++)
            {
                low += this[j];
            }
            return new Range {Low = low, High = low + 1};
        }

        public void Update(byte symbol)
        {
            if (_pendingBytes[symbol])
            {
                _pendingBytes[symbol] = false;
                --TotalFrequencies;
            }
        }

        public RangeSymbol<byte> Decode(uint value)
        {
            uint low = 0;
            byte symbol = 0;
            for (; low + this[symbol] <= value; symbol++)
            {
                low += this[symbol];
            }
            return new RangeSymbol<byte>
                       {
                           Range = new Range
                                       {
                                           Low = low,
                                           High = low + 1
                                       },
                           Symbol = symbol
                       };
        }

        public void Reset()
        {
            for (int i = 0; i < _pendingBytes.Length; ++i)
            {
                _pendingBytes[(byte)i] = true;
            }
            TotalFrequencies = 256;
        }

        #endregion
    }
}