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

 /* This file is entirely for providing functionality for testing.
 * This content will be moved out once other examples have been 
 * created that don't rely on this level of specificity.
 */

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// Extensions to the ArithmeticCoding class for testing purposes.
    /// </summary>
    public static class ArithmeticCodingTestExtensions
    {
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