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