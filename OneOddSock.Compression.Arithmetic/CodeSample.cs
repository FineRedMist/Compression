namespace OneOddSock.Compression.Arithmetic
{
    internal class ArithmeticCoderC
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

        public ArithmeticCoderC()
        {
            _low = 0;
            _high = 0x7FFFFFFF; // just work with least significant 31 bits
            _scale = 0;

            _buffer = 0;
            _step = 0;
        }

        public WriteBitDelegate BitWriter { get; set; }
        public ReadBitDelegate BitReader { get; set; }
        public WriteSymbolDelegate<byte> SymbolWriter { get; set; }
        public ReadSymbolDelegate<byte> SymbolReader { get; set; }

        public void Encode(uint lowCount,
                           uint highCount,
                           uint total)
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
                if (_high < Half)
                {
                    BitWriter(false);
                    _low = _low*2;
                    _high = _high*2 + 1;

                    // perform e3 mappings
                    for (; _scale > 0; _scale--)
                    {
                        BitWriter(true);
                    }
                }
                else if (_low >= Half)
                {
                    BitWriter(true);
                    _low = 2*(_low - Half);
                    _high = 2*(_high - Half) + 1;

                    // perform e3 mappings
                    for (; _scale > 0; _scale--)
                    {
                        BitWriter(false);
                    }
                }
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

        public void EncodeFinish()
        {
            // There are two possibilities of how _low and _high can be distributed,
            // which means that two bits are enough to distinguish them.

            if (_low < FirstQuarter) // _low < FirstQuarter < Half <= _high
            {
                BitWriter(false);

                for (int i = 0; i < _scale + 1; i++) // perform e3-skaling
                {
                    BitWriter(true);
                }
            }
            else // _low < Half < ThirdQuarter <= _high
            {
                BitWriter(true); // zeros added automatically by the decoder; no need to send them
                for (int i = 0; i < _scale + 1; i++) // perform e3-skaling
                {
                    BitWriter(false);
                }
            }
        }

        public void DecodeStart()
        {
            // Fill buffer with bits from the input stream
            for (int i = 0; i < 31; i++) // just use the 31 least significant bits
            {
                _buffer = (_buffer << 1) | ((uint) (BitReader() ? 1 : 0));
            }
        }

        public uint DecodeTarget(uint total)
            // total < 2^29
        {
            // split number space into single steps
            _step = (_high - _low + 1)/total; // interval open at the top => +1

            // return current value
            return (_buffer - _low)/_step;
        }


        public void Decode(uint lowCount,
                           uint highCount)
        {
            // update upper bound
            _high = _low + _step*highCount - 1; // interval open at the top => -1

            // update lower bound
            _low = _low + _step*lowCount;

            // e1/e2 mapping
            while ((_high < Half) || (_low >= Half))
            {
                if (_high < Half)
                {
                    _low = _low*2;
                    _high = _high*2 + 1;
                    _buffer = (uint) (2*_buffer + (BitReader() ? 1 : 0));
                }
                else if (_low >= Half)
                {
                    _low = 2*(_low - Half);
                    _high = 2*(_high - Half) + 1;
                    _buffer = 2*(_buffer - Half) + ((uint) (BitReader() ? 1 : 0));
                }
                _scale = 0;
            }

            // e3 mapping
            while ((FirstQuarter <= _low) && (_high < ThirdQuarter))
            {
                _scale++;
                _low = 2*(_low - FirstQuarter);
                _high = 2*(_high - FirstQuarter) + 1;
                _buffer = 2*(_buffer - FirstQuarter) + ((uint) (BitReader() ? 1 : 0));
            }
        }

        // encoder & decoder
    };


    internal abstract class ModelI
    {
        protected readonly ArithmeticCoderC Coder = new ArithmeticCoderC();
        protected WriteBitDelegate BitWriter { get; set; }
        protected ReadBitDelegate BitReader { get; set; }
        protected WriteSymbolDelegate<byte> SymbolWriter { get; set; }
        protected ReadSymbolDelegate<byte> SymbolReader { get; set; }

        public void Encode(ReadSymbolDelegate<byte> symbolReader, WriteBitDelegate bitWriter, uint symbolCount)
        {
            BitWriter = Coder.BitWriter = bitWriter;
            SymbolReader = Coder.SymbolReader = symbolReader;
            Encode(symbolCount);
            Coder.EncodeFinish();
        }

        public void Decode(ReadBitDelegate bitReader, WriteSymbolDelegate<byte> symbolWriter)
        {
            BitReader = Coder.BitReader = bitReader;
            SymbolWriter = Coder.SymbolWriter = symbolWriter;
            Coder.DecodeStart();
            Decode();
        }

        protected abstract void Encode(uint symbolCount);
        protected abstract void Decode();
    };

    internal class ModelOrder0C : ModelI
    {
        private readonly uint[] _charFrequency = new uint[257];
        private uint _totalFrequencies;

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

        protected override void Encode(uint symbolCount)
        {
            for (uint i = 0; i < symbolCount; ++i)
            {
                byte symbol = SymbolReader();

                // cumulate frequencies
                uint lowCount = 0;
                byte j = 0;
                for (; j < symbol; j++)
                    lowCount += _charFrequency[j];

                // encode symbol
                Coder.Encode(lowCount, lowCount + _charFrequency[j], _totalFrequencies);

                // update model
                _charFrequency[symbol]++;
                _totalFrequencies++;

                if(_totalFrequencies >= (1 << 29))
                {
                    ResetModel();
                }
            }

            // write escape symbol for termination
            Coder.Encode(_totalFrequencies - 1, _totalFrequencies, _totalFrequencies);
        }

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
                    lowCount += _charFrequency[symbol];

                // write symbol
                if (symbol < 256)
                {
                    SymbolWriter((byte) symbol);
                }

                // adapt decoder
                Coder.Decode(lowCount, lowCount + _charFrequency[symbol]);

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