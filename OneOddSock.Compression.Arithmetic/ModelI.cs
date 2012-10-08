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
    /// Abstract model interface.
    /// </summary>
    public abstract class ModelI
    {
        /// <summary>
        /// Arithmetic coder.
        /// </summary>
        protected readonly ArithmeticCoder Coder = new ArithmeticCoder();

        /// <summary>
        /// Bit Writer
        /// </summary>
        protected WriteBitDelegate BitWriter { get; set; }

        /// <summary>
        /// Bit Reader
        /// </summary>
        protected ReadBitDelegate BitReader { get; set; }

        /// <summary>
        /// Symbol Writer
        /// </summary>
        protected WriteSymbolDelegate<byte> SymbolWriter { get; set; }

        /// <summary>
        /// Symbol Reader
        /// </summary>
        protected ReadSymbolDelegate<byte> SymbolReader { get; set; }

        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols read from <paramref name="symbolReader"/>
        /// writing encoded bits to <paramref name="bitWriter"/>.
        /// </summary>
        public void Encode(ReadSymbolDelegate<byte> symbolReader, WriteBitDelegate bitWriter, uint symbolCount)
        {
            BitWriter = bitWriter;
            SymbolReader = symbolReader;
            Encode(symbolCount);
            Coder.EncodeFinish(bitWriter);
        }

        /// <summary>
        /// Decodes symbols from <paramref name="bitReader"/> writing them to <paramref name="symbolWriter"/>.
        /// </summary>
        public void Decode(ReadBitDelegate bitReader, WriteSymbolDelegate<byte> symbolWriter)
        {
            BitReader = bitReader;
            SymbolWriter = symbolWriter;
            Coder.DecodeStart(bitReader);
            Decode();
        }

        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols.
        /// </summary>
        protected abstract void Encode(uint symbolCount);

        /// <summary>
        /// Decodes.
        /// </summary>
        protected abstract void Decode();
    };
}