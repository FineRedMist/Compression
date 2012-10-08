namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Provides the range and the corresponding symbol from the model.
    /// </summary>
    public struct RangeSymbol<TSymbolType>
        where TSymbolType : struct
    {
        /// <summary>
        /// Range of the symbol [Low, High).
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Symbol represented by the range in the model.
        /// </summary>
        public TSymbolType Symbol { get; set; }
    }
}