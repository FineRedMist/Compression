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
        private uint _high;
        private uint _low;
        private uint _scale;
        private uint _step;

        /// <summary>
        /// Constructor
        /// </summary>
        public ArithmeticCoder()
        {
            _low = 0;
            _high = 0x7FFFFFFF; // just work with least significant 31 bits
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
        /// <param name="lowCount"></param>
        /// <param name="highCount"></param>
        /// <param name="total"></param>
        /// <param name="bitWriter"></param>
        public void Encode(uint lowCount,
                           uint highCount,
                           uint total,
                           WriteBitDelegate bitWriter)
            // total < 2^29
        {
            // partition number space into single steps
            _step = (_high - _low + 1)/total; // interval open at the top => +1

            // update upper bound
            _high = _low + _step*highCount - 1; // interval open at the top => -1

            // update lower bound
            _low = _low + _step*lowCount;

            // apply e1/e2 mapping
            while ((_high < Half) || (_low >= Half))
            {
                bool isHighLessThanHalf = _high < Half; // true => emit false for lower half.
                uint sub = isHighLessThanHalf ? 0 : Half;

                bitWriter(!isHighLessThanHalf);
                EmitE3Mappings(bitWriter, isHighLessThanHalf);
                _low = 2*(_low - sub);
                _high = 2*(_high - sub) + 1;
            }

            // e3
            while ((FirstQuarter <= _low) && (_high < ThirdQuarter))
            {
                // keep necessary e3 mappings in mind
                _scale++;
                _low = 2*(_low - FirstQuarter);
                _high = 2*(_high - FirstQuarter) + 1;
            }
        }

        /// <summary>
        /// Finishes encoding, writing any remaining bits.
        /// </summary>
        /// <param name="bitWriter"></param>
        public void EncodeFinish(WriteBitDelegate bitWriter)
        {
            // There are two possibilities of how _low and _high can be distributed,
            // which means that two bits are enough to distinguish them.

            bool isLowGreaterThanFirstQuarter = _low >= FirstQuarter;
            bitWriter(isLowGreaterThanFirstQuarter);

            ++_scale; // Ensures at least one additional bit is written.
            EmitE3Mappings(bitWriter, !isLowGreaterThanFirstQuarter);
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
            _step = (_high - _low + 1)/total; // interval open at the top => +1

            // return current value
            return (_buffer - _low)/_step;
        }


        /// <summary>
        /// Updates the decoder based on the provided range.
        /// </summary>
        /// <param name="lowCount"></param>
        /// <param name="highCount"></param>
        /// <param name="bitReader"></param>
        public void Decode(uint lowCount,
                           uint highCount,
                           ReadBitDelegate bitReader)
        {
            // update upper bound
            _high = _low + _step*highCount - 1; // interval open at the top => -1

            // update lower bound
            _low = _low + _step*lowCount;

            // e1/e2 mapping
            while ((_high < Half) || (_low >= Half))
            {
                bool isHighLessThanHalf = _high < Half; // true => emit false for lower half.
                uint sub = isHighLessThanHalf ? 0 : Half;

                _low = 2*(_low - sub);
                _high = 2*(_high - sub) + 1;
                _buffer = 2*(_buffer - sub) + ((uint) (bitReader() ? 1 : 0));

                _scale = 0;
            }

            // e3 mapping
            while ((FirstQuarter <= _low) && (_high < ThirdQuarter))
            {
                _scale++;
                _low = 2*(_low - FirstQuarter);
                _high = 2*(_high - FirstQuarter) + 1;
                _buffer = 2*(_buffer - FirstQuarter) + ((uint) (bitReader() ? 1 : 0));
            }
        }
    };
}