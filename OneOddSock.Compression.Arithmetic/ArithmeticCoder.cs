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
    /// Arithmetic coder using integer ranges.
    /// </summary>
    public class ArithmeticCoder
    {
        // constants to split the number space of 32 bit integers
        // most significant bit kept free to prevent overflows
        private const uint FirstQuarter = 0x20000000;
        private const uint ThirdQuarter = 0x60000000;
        private const uint Half = 0x40000000;
        private uint _buffer;
        private Range _range;
        private uint _scale;
        private uint _step;

        private static readonly Range BoundaryAdjust = new Range() {Low = 0, High = 1};

        /// <summary>
        /// Constructor
        /// </summary>
        public ArithmeticCoder()
        {
            _range.Low = 0;
            _range.High = 0x7FFFFFFF; // just work with least significant 31 bits
            _scale = 0;

            _buffer = 0;
            _step = 0;
        }

        private void EmitE3Mappings(WriteBitDelegate bitWriter, bool value)
        {
            // perform e3 mappings
            for (; _scale > 0; _scale--)
            {
                bitWriter(value);
            }
        }

        /// <summary>
        /// Encodes the range updating the state of the encoder.
        /// </summary>
        public void Encode(Range counts,
                           uint total,
                           WriteBitDelegate bitWriter)
            // total < 2^29
        {
            // partition number space into single steps
            _step = (_range.Length())/total; // interval open at the top => +1

            // Update bounds -- interval open at the top => -1 for High.
            _range = (counts*_step + _range.Low) - BoundaryAdjust;

            // apply e1/e2 mapping
            while ((_range.High < Half) || (_range.Low >= Half))
            {
                bool isHighLessThanHalf = _range.High < Half; // true => emit false for lower half.
                uint sub = isHighLessThanHalf ? 0 : Half;

                bitWriter(!isHighLessThanHalf);
                EmitE3Mappings(bitWriter, isHighLessThanHalf);

                _range = (_range - sub)*2 + BoundaryAdjust;
            }

            // e3
            while (_range.In(FirstQuarter, ThirdQuarter))
            {
                // keep necessary e3 mappings in mind
                _scale++;
                _range = (_range - FirstQuarter)*2 + BoundaryAdjust;
            }
        }

        /// <summary>
        /// Finishes encoding, writing any remaining bits.
        /// </summary>
        /// <param name="bitWriter"></param>
        public void EncodeFinish(WriteBitDelegate bitWriter)
        {
            // There are two possibilities of how _range.Low and _range.High can be distributed,
            // which means that two bits are enough to distinguish them.

            bool isLowGreaterThanFirstQuarter = _range.Low >= FirstQuarter;
            bitWriter(isLowGreaterThanFirstQuarter);

            ++_scale; // Ensures at least one additional bit is written.
            if (!isLowGreaterThanFirstQuarter)
            {
                EmitE3Mappings(bitWriter, true); // These will default to false for the other case.
            }
        }

        /// <summary>
        /// Initializes the decoder by reading the first set of values for the state.
        /// </summary>
        /// <param name="bitReader"></param>
        public void DecodeStart(ReadBitDelegate bitReader)
        {
            // Fill buffer with bits from the input stream
            for (int i = 0; i < 31; i++) // just use the 31 least significant bits
            {
                _buffer = (_buffer << 1) | ((uint) (bitReader() ? 1 : 0));
            }
        }

        /// <summary>
        /// Retrieves a symbol given the total frequency range.
        /// </summary>
        /// <param name="total"></param>
        /// <returns></returns>
        public uint DecodeTarget(uint total)
            // total < 2^29
        {
            // split number space into single steps
            _step = (_range.Length())/total; // interval open at the top => +1

            // return current value
            return (_buffer - _range.Low)/_step;
        }


        /// <summary>
        /// Updates the decoder based on the provided range.
        /// </summary>
        public void Decode(Range counts,
                           ReadBitDelegate bitReader)
        {
            // Update bounds -- interval open at the top => -1 for High
            _range = (counts*_step + _range.Low) - BoundaryAdjust;

            // e1/e2 mapping
            while ((_range.High < Half) || (_range.Low >= Half))
            {
                bool isHighLessThanHalf = _range.High < Half; // true => emit false for lower half.
                uint sub = isHighLessThanHalf ? 0 : Half;

                _range = (_range - sub)*2 + BoundaryAdjust;
                _buffer = (_buffer - sub)*2 + ((uint) (bitReader() ? 1 : 0));

                _scale = 0;
            }

            // e3 mapping
            while (_range.In(FirstQuarter, ThirdQuarter))
            {
                _scale++;
                _range = (_range - FirstQuarter)*2 + BoundaryAdjust;
                _buffer = 2*(_buffer - FirstQuarter) + ((uint) (bitReader() ? 1 : 0));
            }
        }
    };
}