namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Delegate for writing a single bit <paramref name="value"/>.
    /// </summary>
    public delegate void WriteBitDelegate(bool value);

    /// <summary>
    /// Delegate for reading a single bit.
    /// </summary>
    public delegate bool ReadBitDelegate();

    /// <summary>
    /// Delegate for reading a symbol.
    /// </summary>
    public delegate TSymbolType ReadSymbolDelegate<TSymbolType>();

    /// <summary>
    /// Delegate for writing a <paramref name="symbol"/>.
    /// </summary>
    public delegate void WriteSymbolDelegate<TSymbolType>(TSymbolType symbol) where TSymbolType : struct;
}