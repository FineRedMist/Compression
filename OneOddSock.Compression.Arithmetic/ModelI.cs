namespace OneOddSock.Compression.Arithmetic
{
    internal abstract class ModelI
    {
        protected readonly ArithmeticCoder Coder = new ArithmeticCoder();
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
}