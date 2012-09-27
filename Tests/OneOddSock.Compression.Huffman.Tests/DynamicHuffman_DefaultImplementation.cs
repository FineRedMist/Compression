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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOddSock.IO;

namespace OneOddSock.Compression.Huffman.Tests
{
    [TestClass]
    public class DynamicHuffman_DefaultImplementation
    {
        [TestMethod]
        public void VerifyNewCharOutput_astrachan_()
        {
            string input = "astrachan_";
            string expectedOutput = "astrchn_";
            int expectedOutputPosition = 0;
            var compressor = new DynamicHuffman<char>();

            for (int i = 0; i < input.Length; ++i)
            {
                compressor.WriteCode(input[i], (value) => { },
                                     (symbol)
                                     =>
                                         {
                                             Assert.IsTrue(expectedOutputPosition < expectedOutput.Length);
                                             Assert.AreEqual(expectedOutput[expectedOutputPosition++], symbol);
                                         });
            }
        }

        [TestMethod]
        public void VerifyTree_astrachan_()
        {
            string input = "astrachan_";
            var compressor = new DynamicHuffman<char>();

            var treeSequences = new[]
                                    {
                                        /* * */ new TreeState(1, true),
                                                /* a */ new TreeState(2, 1, 2, true, 'a'),
                                                /* s */ new TreeState(3, 1, 2, true, 3, 4, 'a', 's'),
                                                /* t */ new TreeState(4, 1, 2, true, 3, 4, 5, 6, 'a', 's', 't'),
                                                /* r */
                                                new TreeState(5, 1, 2, true, 3, 4, 5, 6, 7, 8, 's', 'a', 't', 'r'),
                                                /* a */
                                                new TreeState(5, 1, 2, 3, 4, true, 5, 6, 7, 8, 'a', 's', 't', 'r'),
                                                /* c */
                                                new TreeState(6, 1, 2, 3, 4, true, 5, 6, 7, 8, 'a', 9, 10, 't', 's', 'r',
                                                              'c'),
                                                /* h */
                                                new TreeState(7, 1, 2, 3, 4, true, 5, 6, 7, 8, 'a', 9, 10, 11, 12, 's',
                                                              'r', 't', 'c', 'h'),
                                                /* a */
                                                new TreeState(7, 1, 2, 3, 4, 5, 6, true, 7, 8, 'a', 9, 10, 11, 12, 's',
                                                              'r', 't', 'c', 'h'),
                                                /* n */
                                                new TreeState(8, 1, 2, 3, 4, 5, 6, true, 7, 8, 'a', 9, 10, 11, 12, 13,
                                                              14, 'r', 't', 'c', 's', 'h', 'n'),
                                                /* _ */
                                                new TreeState(9, 1, 2, 3, 4, true, 5, 6, 7, 8, 'a', 9, 10, 11, 12, 13,
                                                              14, 15, 16, 't', 'c', 's', 'h', 'r', 'n', '_'),
                                    };


            compressor.WriteTable(treeSequences[0].WriteSymbol, treeSequences[0].WriteUInt32,
                                  treeSequences[0].WriteNotYetTransmitted);
            treeSequences[0].Final();

            for (int i = 0; i < input.Length; ++i)
            {
                compressor.WriteCode(input[i], (value) => { }, (symbol) => { });
                compressor.WriteTable(treeSequences[i + 1].WriteSymbol, treeSequences[i + 1].WriteUInt32,
                                      treeSequences[i + 1].WriteNotYetTransmitted);
                treeSequences[i + 1].Final();
            }
        }

        [TestMethod]
        public void VerifyEncoding_astrachan_()
        {
            string input = "astrachan_";
            var data = new MemoryStream();
            var writer = new BitBinaryWriter(data);
            var reader = new BitBinaryReader(data);

            var compressor = new DynamicHuffman<char>();

            for (int i = 0; i < input.Length; ++i)
            {
                compressor.WriteCode(input[i], writer.Write, writer.Write);
            }
            writer.Flush();
            data.Position = 0;

            var decompressor = new DynamicHuffman<char>();
            for (int i = 0; i < input.Length; ++i)
            {
                Assert.AreEqual(input[i], decompressor.GetSymbol(reader.ReadBoolean, reader.ReadChar));
            }
            Assert.AreEqual(data.Position, data.Length);
        }

        [TestMethod]
        public void VerifyEncoding_largecorpus()
        {
            string input = TestResources.RFC5_Text;
            int len = input.Length;
            var data = new MemoryStream();
            var writer = new BitBinaryWriter(data);
            var reader = new BitBinaryReader(data);

            var compressor = new DynamicHuffman<char>();

            for (int i = 0; i < len; ++i)
            {
                compressor.WriteCode(input[i], writer.Write, writer.Write);
            }
            writer.Flush();
            data.Position = 0;

            var decompressor = new DynamicHuffman<char>();
            for (int i = 0; i < len; ++i)
            {
                Assert.AreEqual(input[i], decompressor.GetSymbol(reader.ReadBoolean, reader.ReadChar));
            }
            Assert.AreEqual(data.Position, data.Length);
        }
    }
}