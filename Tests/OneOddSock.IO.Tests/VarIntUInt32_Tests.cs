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
    public class VarIntUInt32_Tests
    {
        private void TestValue_UInt32(params uint[] values)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                uint value = values[i];
                uint size = values[i + 1];
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
                uint value = values[i];
                uint result = reader.ReadVarUInt32();
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void UInt32_0()
        {
            TestValue_UInt32(0, 1);
        }

        [TestMethod]
        public void UInt32_7f()
        {
            TestValue_UInt32(0x7f, 1);
        }

        [TestMethod]
        public void UInt32_80()
        {
            TestValue_UInt32(0x80, 2);
        }

        [TestMethod]
        public void UInt32_3fff()
        {
            TestValue_UInt32(0x3fff, 2);
        }

        [TestMethod]
        public void UInt32_4000()
        {
            TestValue_UInt32(0x4000, 3);
        }

        [TestMethod]
        public void UInt32_407f()
        {
            TestValue_UInt32(0x407f, 3);
        }

        [TestMethod]
        public void UInt32_ffff()
        {
            TestValue_UInt32(0xffff, 3);
        }

        [TestMethod]
        public void UInt32_1fffff()
        {
            TestValue_UInt32(0x1fffff, 3);
        }

        [TestMethod]
        public void UInt32_200000()
        {
            TestValue_UInt32(0x200000, 4);
        }

        [TestMethod]
        public void UInt32_fffffff()
        {
            TestValue_UInt32(0xfffffff, 4);
        }

        [TestMethod]
        public void UInt32_10000000()
        {
            TestValue_UInt32(0x10000000, 5);
        }

        [TestMethod]
        public void UInt32_ffffffff()
        {
            TestValue_UInt32(0xffffffff, 5);
        }

        [TestMethod]
        public void UInt32_Multiple()
        {
            TestValue_UInt32(0x0, 1, 0x7f, 1, 0x80, 2, 0x3fff, 2, 0xffff, 3, 0x0, 1, 0x3fff, 2, 0x80, 2, 0xffff, 3, 0x7f,
                             1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void UInt16_Overflow_Failure_UpperBound()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            ulong upper = uint.MaxValue;
            writer.WriteVar(upper + 1);
            writer.Flush();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            uint result = reader.ReadVarUInt32();
            Assert.IsTrue(false, "This ought to be unreachable.");
        }
    }
}