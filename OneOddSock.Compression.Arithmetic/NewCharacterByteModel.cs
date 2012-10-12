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
        private readonly PartialSumTreeByte _stats;

        public NewCharacterByteModel()
        {
            _stats = new PartialSumTreeByte(byte.MaxValue);
            Init();
        }

        private uint this[byte index]
        {
            get { return _stats.GetWeight(index); }
        }

        private void Init()
        {
            for(uint symbol = 0; symbol <= byte.MaxValue; ++symbol)
            {
                _stats.UpdateWeight(symbol, 1);
            }
        }

        #region IModel<byte> Members

        public uint TotalFrequencies { get { return _stats.Total; } }

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
            if (_stats.GetWeight(symbol) > 0)
            {
                _stats.UpdateWeight(symbol, -1);
            }
        }

        public RangeSymbol<byte> Decode(uint value)
        {
            byte symbol = (byte) _stats.GetSymbol(value);

            return new RangeSymbol<byte>
                       {
                           Range = _stats[symbol],
                           Symbol = symbol
                       };
        }

        public void Reset()
        {
            _stats.Reset();
            Init();
        }

        #endregion

        public bool Emitted(byte index)
        {
            return this[index] == 0;
        }
    }
}