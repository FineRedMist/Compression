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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOddSock.IO;

namespace OneOddSock.Compression.Huffman.Tests
{
    [TestClass]
    public class DynamicHuffman_OriginalNYTImplementation
    {
        private static int OriginalNYT(uint treeHeight, uint nytLevel, uint treeWeight, uint nytWeight, uint symbolCount,
                                       char symbol, bool nytOccurred)
        {
            return 0;
        }

        [TestMethod]
        public void VerifyNewCharOutput_astrachan_()
        {
            string input = "astrachan_";
            string expectedOutput = "astrchn_";
            int expectedOutputPosition = 0;
            var compressor = new DynamicHuffman<char>(OriginalNYT);

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
            var compressor = new DynamicHuffman<char>(OriginalNYT);

            var treeSequences = new[]
                                    {
                                        // Bit of history--this was the example I went through manually from a web resource on adaptive 
                                        // Huffman.  It only provided the final state  to verify against.  I put together the trees by
                                        // hand to ensure I understood it, then used the same trees to generate the expected results here.
                                        /* * */ new TreeState(1, true),
                                                /* a */ new TreeState(2, 1, 2, 'a', true),
                                                /* s */ new TreeState(3, 1, 2, 'a', 3, 4, 's', true),
                                                /* t */ new TreeState(4, 1, 2, 3, 4, 'a', 's', 5, 6, 't', true),
                                                /* r */
                                                new TreeState(5, 1, 2, 3, 4, 5, 6, 's', 'a', 't', 7, 8, 'r', true),
                                                /* a */
                                                new TreeState(5, 1, 2, 3, 4, 5, 6, 'a', 's', 't', 7, 8, 'r', true),
                                                /* c */
                                                new TreeState(6, 1, 2, 3, 4, 5, 6, 'a', 7, 8, 't', 's', 'r', 9, 10, 'c',
                                                              true),
                                                /* h */
                                                new TreeState(7, 1, 2, 3, 4, 5, 6, 'a', 7, 8, 9, 10, 's', 'r', 't', 'c',
                                                              11, 12, 'h', true),
                                                /* a */
                                                new TreeState(7, 1, 2, 3, 4, 5, 6, 'a', 7, 8, 9, 10, 's', 'r', 't', 'c',
                                                              11, 12, 'h', true),
                                                /* n */
                                                new TreeState(8, 1, 2, 3, 4, 5, 6, 'a', 7, 8, 9, 10, 11, 12, 'r', 't',
                                                              'c', 's', 'h', 13, 14, 'n', true),
                                                /* _ */
                                                new TreeState(9, 1, 2, 3, 4, 5, 6, 'a', 7, 8, 9, 10, 11, 12, 13, 14, 't',
                                                              'c', 's', 'h', 'r', 'n', 15, 16, '_', true)
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

            var compressor = new DynamicHuffman<char>(OriginalNYT);

            for (int i = 0; i < input.Length; ++i)
            {
                compressor.WriteCode(input[i], writer.Write, writer.Write);
            }
            writer.Flush();
            data.Position = 0;

            var decompressor = new DynamicHuffman<char>(OriginalNYT);
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

            var compressor = new DynamicHuffman<char>(OriginalNYT);

            for (int i = 0; i < len; ++i)
            {
                compressor.WriteCode(input[i], writer.Write, writer.Write);
            }
            writer.Flush();
            data.Position = 0;

            var decompressor = new DynamicHuffman<char>(OriginalNYT);
            for (int i = 0; i < len; ++i)
            {
                Assert.AreEqual(input[i], decompressor.GetSymbol(reader.ReadBoolean, reader.ReadChar));
            }
            Assert.AreEqual(data.Position, data.Length);
        }

        [TestMethod]
        public void VerifyBulkUpdateIncrease()
        {
            string originalInput = "astrachan________";
            var originalCompressor = new DynamicHuffman<char>(OriginalNYT);

            var originalTreeState = new TreeStateStore();

            for (int i = 0; i < originalInput.Length; ++i)
            {
                originalCompressor.WriteCode(originalInput[i], (bit) => { }, (symbol) => { });
            }

            originalCompressor.WriteTable(originalTreeState.WriteSymbol, originalTreeState.WriteUInt32,
                                          originalTreeState.WriteNotYetTransmitted);

            string partialInput = "astrachan";
            var bulkCompressor = new DynamicHuffman<char>(OriginalNYT);

            for (int i = 0; i < partialInput.Length; ++i)
            {
                bulkCompressor.WriteCode(partialInput[i], (bit) => { }, (symbol) => { });
            }

            bulkCompressor.UpdateSymbol('_', 8);

            var bulkTreeState = new TreeStateStore();
            bulkCompressor.WriteTable(bulkTreeState.WriteSymbol, bulkTreeState.WriteUInt32,
                                      bulkTreeState.WriteNotYetTransmitted);

            CollectionAssert.AreEqual(originalTreeState, bulkTreeState);
        }

        //[TestMethod]
        // Disabled for now.  Decreases are not guaranteed symmetric with increases.
        public void VerifyBulkUpdateDecrease()
        {
            string originalInput = "astrachan_";
            var originalCompressor = new DynamicHuffman<char>(OriginalNYT);

            var originalTreeState = new TreeStateStore();

            for (int i = 0; i < originalInput.Length; ++i)
            {
                originalCompressor.WriteCode(originalInput[i], (bit) => { }, (symbol) => { });
            }

            originalCompressor.WriteTable(originalTreeState.WriteSymbol, originalTreeState.WriteUInt32,
                                          originalTreeState.WriteNotYetTransmitted);

            string partialInput = "astrachan";
            var bulkCompressor = new DynamicHuffman<char>(OriginalNYT);

            for (int i = 0; i < partialInput.Length; ++i)
            {
                bulkCompressor.WriteCode(partialInput[i], (bit) => { }, (symbol) => { });
            }

            bulkCompressor.UpdateSymbol('_', 8);
            bulkCompressor.UpdateSymbol('_', -7);

            var bulkTreeState = new TreeStateStore();
            bulkCompressor.WriteTable(bulkTreeState.WriteSymbol, bulkTreeState.WriteUInt32,
                                      bulkTreeState.WriteNotYetTransmitted);

            CollectionAssert.AreEqual(originalTreeState, bulkTreeState);
        }
    }
}