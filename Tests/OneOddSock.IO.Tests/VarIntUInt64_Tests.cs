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
    public class VarIntUInt64_Tests
    {
        private void TestValue_UInt64(params ulong[] values)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                ulong value = values[i];
                ulong size = values[i + 1];
                long pos = stream.Position;
                writer.WriteVar(value);
                writer.Flush();
                long newPos = stream.Position;
                Assert.AreEqual(size, (ulong)(newPos - pos), "Size mismatch for {0}", value);
            }

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                ulong value = values[i];
                ulong result = reader.ReadVarUInt64();
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void UInt64_0()
        {
            TestValue_UInt64(0, 1);
        }

        [TestMethod]
        public void UInt64_7f()
        {
            TestValue_UInt64(0x7f, 1);
        }

        [TestMethod]
        public void UInt64_80()
        {
            TestValue_UInt64(0x80, 2);
        }

        [TestMethod]
        public void UInt64_3fff()
        {
            TestValue_UInt64(0x3fff, 2);
        }

        [TestMethod]
        public void UInt64_4000()
        {
            TestValue_UInt64(0x4000, 3);
        }

        [TestMethod]
        public void UInt64_407f()
        {
            TestValue_UInt64(0x407f, 3);
        }

        [TestMethod]
        public void UInt64_ffff()
        {
            TestValue_UInt64(0xffff, 3);
        }

        [TestMethod]
        public void UInt64_1fffff()
        {
            TestValue_UInt64(0x1fffff, 3);
        }

        [TestMethod]
        public void UInt64_200000()
        {
            TestValue_UInt64(0x200000, 4);
        }

        [TestMethod]
        public void UInt64_fffffff()
        {
            TestValue_UInt64(0xfffffff, 4);
        }

        [TestMethod]
        public void UInt64_10000000()
        {
            TestValue_UInt64(0x10000000, 5);
        }

        [TestMethod]
        public void UInt64_7ffffffff()
        {
            TestValue_UInt64(0x7ffffffff, 5);
        }

        [TestMethod]
        public void UInt64_800000000()
        {
            TestValue_UInt64(0x800000000, 6);
        }

        [TestMethod]
        public void UInt64_3ffffffffff()
        {
            TestValue_UInt64(0x3ffffffffff, 6);
        }

        [TestMethod]
        public void UInt64_40000000000()
        {
            TestValue_UInt64(0x40000000000, 7);
        }

        [TestMethod]
        public void UInt64_1fffffffffff()
        {
            TestValue_UInt64(0x1fffffffffff, 7);
        }

        [TestMethod]
        public void UInt64_2000000000000()
        {
            TestValue_UInt64(0x2000000000000, 8);
        }

        [TestMethod]
        public void UInt64_ffffffffffffff()
        {
            TestValue_UInt64(0xffffffffffffff, 8);
        }

        [TestMethod]
        public void UInt64_100000000000000()
        {
            TestValue_UInt64(0x100000000000000, 9);
        }

        [TestMethod]
        public void UInt64_ffffffffffffffff()
        {
            TestValue_UInt64(0xffffffffffffffff, 9);
        }

        [TestMethod]
        public void UInt64_Multiple()
        {
            TestValue_UInt64(0x0, 1, 0x7f, 1, 0x80, 2, 0x3fff, 2, 0xffff, 3, 0x0, 1, 0x3fff, 2, 0x80, 2, 0xffff, 3, 0x7f,
                             1);
        }
    }
}