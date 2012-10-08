namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Defines the methods required to represent a model for use with arithmetic coding.
    /// </summary>
    public interface IModel<TSymbolType>
        where TSymbolType : struct
    {
        /// <summary>
        /// The total of frequencies for all symbols represented by the model.
        /// </summary>
        uint TotalFrequencies { get; }

        /// <summary>
        /// The [Low, High) range covered by <paramref name="symbol"/>.
        /// </summary>
        Range GetRange(TSymbolType symbol);

        /// <summary>
        /// Update the frequency information of <paramref name="symbol"/>.
        /// </summary>
        void Update(TSymbolType symbol);

        /// <summary>
        /// Finds the symbol for the corresponding <paramref name="value"/>.
        /// </summary>
        RangeSymbol<TSymbolType> Decode(uint value);

        /// <summary>
        /// Resets the model statistics.
        /// </summary>
        void Reset();
    }
}