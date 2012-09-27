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

namespace BitStreamTests
{
    [TestClass]
    public class VarIntSInt64_Tests
    {
        private void TestValue_SInt64(params long[] values)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                long value = values[i];
                long size = values[i + 1];
                long pos = stream.Position;
                writer.WriteVar(value);
                writer.Flush();
                long newPos = stream.Position;
                Assert.AreEqual(size, newPos - pos, "Size mismatch for {0}", value);
            }

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                long value = values[i];
                long result = reader.ReadVarInt64();
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void SInt64_0()
        {
            TestValue_SInt64(0, 1);
        }

        [TestMethod]
        public void SInt64_3f()
        {
            TestValue_SInt64(0x3f, 1);
        }

        [TestMethod]
        public void SInt64_40()
        {
            TestValue_SInt64(0x40, 2);
        }

        [TestMethod]
        public void SInt64_7f()
        {
            TestValue_SInt64(0x7f, 2);
        }

        [TestMethod]
        public void SInt64_80()
        {
            TestValue_SInt64(0x80, 2);
        }

        [TestMethod]
        public void SInt64_1fff()
        {
            TestValue_SInt64(0x1fff, 2);
        }

        [TestMethod]
        public void SInt64_2000()
        {
            TestValue_SInt64(0x2000, 3);
        }

        [TestMethod]
        public void SInt64_3fff()
        {
            TestValue_SInt64(0x3fff, 3);
        }

        [TestMethod]
        public void SInt64_4000()
        {
            TestValue_SInt64(0x4000, 3);
        }

        [TestMethod]
        public void SInt64_407f()
        {
            TestValue_SInt64(0x407f, 3);
        }

        [TestMethod]
        public void SInt64_ffff()
        {
            TestValue_SInt64(0xffff, 3);
        }

        [TestMethod]
        public void SInt64_fffff()
        {
            TestValue_SInt64(0xfffff, 3);
        }

        [TestMethod]
        public void SInt64_100000()
        {
            TestValue_SInt64(0x100000, 4);
        }

        [TestMethod]
        public void SInt64_1fffff()
        {
            TestValue_SInt64(0x1fffff, 4);
        }

        [TestMethod]
        public void SInt64_200000()
        {
            TestValue_SInt64(0x200000, 4);
        }

        [TestMethod]
        public void SInt64_7ffffff()
        {
            TestValue_SInt64(0x7ffffff, 4);
        }

        [TestMethod]
        public void SInt64_8000000()
        {
            TestValue_SInt64(0x8000000, 5);
        }

        [TestMethod]
        public void SInt64_fffffff()
        {
            TestValue_SInt64(0xfffffff, 5);
        }

        [TestMethod]
        public void SInt64_10000000()
        {
            TestValue_SInt64(0x10000000, 5);
        }

        [TestMethod]
        public void SInt64_3ffffffff()
        {
            TestValue_SInt64(0x3ffffffff, 5);
        }

        [TestMethod]
        public void SInt64_400000000()
        {
            TestValue_SInt64(0x400000000, 6);
        }

        [TestMethod]
        public void SInt64_7ffffffff()
        {
            TestValue_SInt64(0x7ffffffff, 6);
        }

        [TestMethod]
        public void SInt64_800000000()
        {
            TestValue_SInt64(0x800000000, 6);
        }

        [TestMethod]
        public void SInt64_1ffffffffff()
        {
            TestValue_SInt64(0x1ffffffffff, 6);
        }

        [TestMethod]
        public void SInt64_20000000000()
        {
            TestValue_SInt64(0x20000000000, 7);
        }

        [TestMethod]
        public void SInt64_3ffffffffff()
        {
            TestValue_SInt64(0x3ffffffffff, 7);
        }

        [TestMethod]
        public void SInt64_40000000000()
        {
            TestValue_SInt64(0x40000000000, 7);
        }

        [TestMethod]
        public void SInt64_1fffffffffff()
        {
            TestValue_SInt64(0x1fffffffffff, 7);
        }

        [TestMethod]
        public void SInt64_2000000000000()
        {
            TestValue_SInt64(0x2000000000000, 8);
        }

        [TestMethod]
        public void SInt64_7fffffffffffff()
        {
            TestValue_SInt64(0x7fffffffffffff, 8);
        }

        [TestMethod]
        public void SInt64_80000000000000()
        {
            TestValue_SInt64(0x80000000000000, 9);
        }

        [TestMethod]
        public void SInt64_ffffffffffffff()
        {
            TestValue_SInt64(0xffffffffffffff, 9);
        }

        [TestMethod]
        public void SInt64_100000000000000()
        {
            TestValue_SInt64(0x100000000000000, 9);
        }

        [TestMethod]
        public void SInt64_7fffffffffffffff()
        {
            TestValue_SInt64(0x7fffffffffffffff, 9);
        }

        [TestMethod]
        public void SInt64_Multiple()
        {
            TestValue_SInt64(0x0, 1, 0x7f, 2, 0x80, 2, 0x3fff, 3, 0xffff, 3, 0x0, 1, 0x3fff, 3, 0x80, 2, 0xffff, 3, 0x7f,
                             2);
        }
    }
}