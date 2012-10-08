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
    /// Zero order dynamic model of the byte distribution.
    /// </summary>
    public class ModelOrder0C : ModelI
    {
        private readonly ZeroOrderAdaptiveByteModel model = new ZeroOrderAdaptiveByteModel();

        /// <summary>
        /// Constructor
        /// </summary>
        public ModelOrder0C()
        {
            ResetModel();
        }

        private void ResetModel()
        {
            model.Reset();
        }

        /// <summary>
        /// Encodes <paramref name="symbolCount"/> symbols.
        /// </summary>
        protected override void Encode(uint symbolCount)
        {
            for (uint i = 0; i < symbolCount; ++i)
            {
                byte symbol = SymbolReader();

                Coder.Encode(symbol, model, BitWriter);
            }

            // write escape symbol for termination
            Coder.Encode((uint) 256, model, BitWriter);
        }

        /// <summary>
        /// Decodes.
        /// </summary>
        protected override void Decode()
        {
            uint symbol;

            do
            {
                symbol = Coder.Decode(model, BitReader);
                if (symbol != 256)
                {
                    SymbolWriter((byte) symbol);
                }
            } while (symbol != 256); // until termination symbol read
        }
    };
}