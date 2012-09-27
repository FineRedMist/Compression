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

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOddSock.IO;

namespace OneOddSock.Compression.Huffman.Tests
{
    [TestClass]
    public class StaticHuffman
    {
        private static IDictionary<char, uint> CharacterFrequencies(string input)
        {
            var results = new Dictionary<char, uint>(100);
            foreach (char ch in input)
            {
                uint value = 0;
                results.TryGetValue(ch, out value);
                results[ch] = value + 1;
            }
            return results;
        }

        [TestMethod]
        public void VerifyTable()
        {
            string input = "astrachan_";
            var compressor = new StaticHuffman<char>(CharacterFrequencies(input));

            // Node ids:					0    1    2    3    4    5    6    7    8     9    10    11    12     13      14
            // Weights:						1    1    1    1    1    1    1    3    2     2    2     3     4      6       10
            var state = new TreeState(8, '_', 'c', 'h', 'n', 'r', 's', 't', 'a', 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 7, 11,
                                      12, 13);
            compressor.WriteTable(state.WriteSymbol, state.WriteUInt32);
            state.Final();
        }

        [TestMethod]
        public void VerifyReconstruction()
        {
            string input = "astrachan_";
            var compressor = new StaticHuffman<char>(CharacterFrequencies(input));

            // Node ids:					0    1    2    3    4    5    6    7    8     9    10    11    12     13      14
            // Weights:						1    1    1    1    1    1    1    3    2     2    2     3     4      6       10
            var state = new TreeState(8, '_', 'c', 'h', 'n', 'r', 's', 't', 'a', 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 7, 11,
                                      12, 13);
            compressor.WriteTable(state.WriteSymbol, state.WriteUInt32);
            state.Final();

            state.Reset();
            var decompressor = new StaticHuffman<char>(state.ReadSymbol, state.ReadUInt32);
            state.Final();
            state.Reset();
            decompressor.WriteTable(state.WriteSymbol, state.WriteUInt32);
            state.Final();
        }

        [TestMethod]
        public void VerifyCompression_astrachan_()
        {
            var stream = new MemoryStream();
            string input = "astrachan_";
            var compressor = new StaticHuffman<char>(CharacterFrequencies(input));

            // Node ids:					     0    1    2    3    4    5    6    7  8     9    10    11    12     13      14
            // Weights:						     1    1    1    1    1    1    1    3  2     2     2     3     4      6      10
            var state = new TreeState(8, '_', 'c', 'h', 'n', 'r', 's', 't', 'a', 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 7, 11,
                                      12, 13);
            compressor.WriteTable(state.WriteSymbol, state.WriteUInt32);
            state.Final();

            var writer = new BitStreamWriter(stream, true);
            foreach (char ch in input)
            {
                compressor.WriteCode(ch, writer.Write);
            }
            writer.Flush();

            state.Reset();
            var decompressor = new StaticHuffman<char>(state.ReadSymbol, state.ReadUInt32);
            state.Final();
            state.Reset();
            decompressor.WriteTable(state.WriteSymbol, state.WriteUInt32);
            state.Final();

            stream.Position = 0;

            var reader = new BitStreamReader(stream, true);
            foreach (char ch in input)
            {
                Assert.AreEqual(ch, decompressor.GetSymbol(reader.ReadBoolean));
            }
        }

        [TestMethod]
        public void VerifyCompression_LargeCorpus()
        {
            var stream = new MemoryStream();
            var writer = new BitStreamWriter(stream, true);
            var reader = new BitStreamReader(stream, true);
            string input = TestResources.RFC5_Text;
            var state = new TreeStateStore();
            var compressor = new StaticHuffman<char>(CharacterFrequencies(input));

            compressor.WriteTable(state.WriteSymbol, state.WriteUInt32);

            foreach (char ch in input)
            {
                compressor.WriteCode(ch, writer.Write);
            }
            writer.Flush();

            state.Reset();

            var decompressor = new StaticHuffman<char>(state.ReadSymbol, state.ReadUInt32);

            stream.Position = 0;

            foreach (char ch in input)
            {
                Assert.AreEqual(ch, decompressor.GetSymbol(reader.ReadBoolean));
            }
        }
    }
}