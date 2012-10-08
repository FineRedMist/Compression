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