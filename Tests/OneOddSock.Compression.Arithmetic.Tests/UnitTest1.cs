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
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOddSock.IO;

namespace OneOddSock.Compression.Arithmetic.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] data = Encoding.UTF8.GetBytes(TestResources.RFC5_Text);

            var stream = new MemoryStream();
            var encoder = new ArithmeticStream(stream, CompressionMode.Compress, true);

            encoder.Write(data, 0, data.Length);
            encoder.Flush();

            stream.Position = 0;

            var decoder = new ArithmeticStream(stream, CompressionMode.Decompress, true);
            var decoded = new byte[data.Length];

            decoder.Read(decoded, 0, decoded.Length);

            CollectionAssert.AreEqual(data, decoded);

            Assert.AreEqual(16235, stream.Length);
        }
    }
}