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

namespace BitStreamTests
{
    [TestClass]
    public class VarIntSInt32_Tests
    {
        private void TestValue_SInt32(params int[] values)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                int value = values[i];
                int size = values[i + 1];
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
                int value = values[i];
                int result = reader.ReadVarInt32();
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void SInt32_0()
        {
            TestValue_SInt32(0, 1);
        }

        [TestMethod]
        public void SInt32_3f()
        {
            TestValue_SInt32(0x3f, 1);
        }

        [TestMethod]
        public void SInt32_40()
        {
            TestValue_SInt32(0x40, 2);
        }

        [TestMethod]
        public void SInt32_7f()
        {
            TestValue_SInt32(0x7f, 2);
        }

        [TestMethod]
        public void SInt32_80()
        {
            TestValue_SInt32(0x80, 2);
        }

        [TestMethod]
        public void SInt32_1fff()
        {
            TestValue_SInt32(0x1fff, 2);
        }

        [TestMethod]
        public void SInt32_2000()
        {
            TestValue_SInt32(0x2000, 3);
        }

        [TestMethod]
        public void SInt32_3fff()
        {
            TestValue_SInt32(0x3fff, 3);
        }

        [TestMethod]
        public void SInt32_4000()
        {
            TestValue_SInt32(0x4000, 3);
        }

        [TestMethod]
        public void SInt32_407f()
        {
            TestValue_SInt32(0x407f, 3);
        }

        [TestMethod]
        public void SInt32_fffff()
        {
            TestValue_SInt32(0xfffff, 3);
        }

        [TestMethod]
        public void SInt32_100000()
        {
            TestValue_SInt32(0x100000, 4);
        }

        [TestMethod]
        public void SInt32_1fffff()
        {
            TestValue_SInt32(0x1fffff, 4);
        }

        [TestMethod]
        public void SInt32_200000()
        {
            TestValue_SInt32(0x200000, 4);
        }

        [TestMethod]
        public void SInt32_7ffffff()
        {
            TestValue_SInt32(0x7ffffff, 4);
        }

        [TestMethod]
        public void SInt32_8000000()
        {
            TestValue_SInt32(0x8000000, 5);
        }

        [TestMethod]
        public void SInt32_fffffff()
        {
            TestValue_SInt32(0xfffffff, 5);
        }

        [TestMethod]
        public void SInt32_10000000()
        {
            TestValue_SInt32(0x10000000, 5);
        }

        [TestMethod]
        public void SInt32_ffffffff()
        {
            TestValue_SInt32(0x7fffffff, 5);
        }

        [TestMethod]
        public void SInt32_Multiple()
        {
            TestValue_SInt32(0x0, 1, 0x7f, 2, 0x80, 2, 0x3fff, 3, 0xffff, 3, 0x0, 1, 0x3fff, 3, 0x80, 2, 0xffff, 3, 0x7f,
                             2);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void SInt32_Overflow_Failure_LowerBound()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            long lower = int.MinValue;
            writer.WriteVar(lower - 1);
            writer.Flush();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            int result = reader.ReadVarInt32();
            Assert.IsTrue(false, "This ought to be unreachable.");
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void SInt32_Overflow_Failure_UpperBound()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            long upper = int.MaxValue;
            writer.WriteVar(upper + 1);
            writer.Flush();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            int result = reader.ReadVarInt32();
            Assert.IsTrue(false, "This ought to be unreachable.");
        }
    }
}