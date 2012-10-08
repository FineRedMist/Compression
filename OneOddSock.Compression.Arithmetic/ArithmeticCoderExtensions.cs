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
    /// Extensions to ArithmeticCoder to simplify usage.
    /// </summary>
    public static class ArithmeticCoderExtensions
    {
        /// <summary>
        /// Encodes <paramref name="symbol"/> using the <paramref name="coder"/> with
        /// the provided <paramref name="model"/>.
        /// </summary>
        public static void Encode<TSymbolType>(this ArithmeticCoder coder, TSymbolType symbol, IModel<TSymbolType> model,
                                               WriteBitDelegate bitWriter)
            where TSymbolType : struct
        {
            // cumulate frequencies
            Range count = model.GetRange(symbol);

            // encode symbol
            coder.Encode(count, model.TotalFrequencies, bitWriter);

            // update model
            model.Update(symbol);
        }

        /// <summary>
        /// Decodes a symbol using the <paramref name="coder"/> with the provided 
        /// <paramref name="model"/>.
        /// </summary>
        public static TSymbolType Decode<TSymbolType>(this ArithmeticCoder coder, IModel<TSymbolType> model,
                                                      ReadBitDelegate bitReader)
            where TSymbolType : struct
        {
            // read value
            uint value = coder.DecodeTarget(model.TotalFrequencies);

            // determine symbol
            RangeSymbol<TSymbolType> rangeSymbol = model.Decode(value);

            // adapt decoder
            coder.Decode(rangeSymbol.Range, bitReader);

            // update model
            model.Update(rangeSymbol.Symbol);

            return rangeSymbol.Symbol;
        }

        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols read from <paramref name="symbolReader"/>
        /// writing encoded bits to <paramref name="bitWriter"/>.
        /// </summary>
        public static void Encode(this ArithmeticCoder coder, IModel<uint> model, WriteBitDelegate bitWriter,
                                  ReadSymbolDelegate<byte> symbolReader, uint symbolCount)
        {
            for (uint i = 0; i < symbolCount; ++i)
            {
                byte symbol = symbolReader();

                coder.Encode(symbol, model, bitWriter);
            }

            // write escape symbol for termination
            coder.Encode((uint) 256, model, bitWriter);
            coder.EncodeFinish(bitWriter);
        }

        /// <summary>
        /// Decodes symbols from <paramref name="coder"/> using the provided <paramref name="model"/>.
        /// </summary>
        public static void Decode(this ArithmeticCoder coder, IModel<uint> model, WriteSymbolDelegate<byte> symbolWriter,
                                  ReadBitDelegate bitReader)
        {
            coder.DecodeStart(bitReader);

            uint symbol;

            do
            {
                symbol = coder.Decode(model, bitReader);
                if (symbol != 256)
                {
                    symbolWriter((byte) symbol);
                }
            } while (symbol != 256); // until termination symbol read
        }
    }
}