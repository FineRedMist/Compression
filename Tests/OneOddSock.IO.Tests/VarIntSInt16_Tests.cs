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

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneOddSock.IO;

namespace BitStreamTests
{
    [TestClass]
    public class VarIntSInt16_Tests
    {
        private void TestValue_SInt16(params short[] values)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            for (int i = 0; i < values.Length; i += 2)
            {
                short value = values[i];
                short size = values[i + 1];
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
                short value = values[i];
                short result = reader.ReadVarInt16();
                Assert.AreEqual(value, result);
            }
        }

        [TestMethod]
        public void SInt16_0()
        {
            TestValue_SInt16(0, 1);
        }

        [TestMethod]
        public void SInt16_1()
        {
            TestValue_SInt16(1, 1);
        }

        [TestMethod]
        public void SInt16_N1()
        {
            TestValue_SInt16(-1, 1);
        }

        [TestMethod]
        public void SInt16_3f()
        {
            TestValue_SInt16(0x3f, 1);
        }

        [TestMethod]
        public void SInt16_40()
        {
            TestValue_SInt16(0x40, 2);
        }

        [TestMethod]
        public void SInt16_7f()
        {
            TestValue_SInt16(0x7f, 2);
        }

        [TestMethod]
        public void SInt16_80()
        {
            TestValue_SInt16(0x80, 2);
        }

        [TestMethod]
        public void SInt16_1fff()
        {
            TestValue_SInt16(0x1fff, 2);
        }

        [TestMethod]
        public void SInt16_2000()
        {
            TestValue_SInt16(0x2000, 3);
        }

        [TestMethod]
        public void SInt16_3fff()
        {
            TestValue_SInt16(0x3fff, 3);
        }

        [TestMethod]
        public void SInt16_4000()
        {
            TestValue_SInt16(0x4000, 3);
        }

        [TestMethod]
        public void SInt16_ffff()
        {
            TestValue_SInt16(0x7fff, 3);
        }

        [TestMethod]
        public void SInt16_Multiple()
        {
            TestValue_SInt16(0x0, 1, 0x7f, 2, 0x80, 2, 0x3fff, 3, 0x7fff, 3, 0x0, 1, 0x3fff, 3, 0x80, 2, 0x7fff, 3, 0x7f,
                             2);
        }

        [TestMethod]
        [ExpectedException(typeof (OverflowException))]
        public void SInt16_Overflow_Failure_LowerBound()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.WriteVar(-32769);
            writer.Flush();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            short result = reader.ReadVarInt16();
            Assert.IsTrue(false, "This ought to be unreachable.");
        }

        [TestMethod]
        [ExpectedException(typeof (OverflowException))]
        public void SInt16_Overflow_Failure_UpperBound()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.WriteVar(32768);
            writer.Flush();

            stream.Position = 0;
            var reader = new BinaryReader(stream);
            short result = reader.ReadVarInt16();
            Assert.IsTrue(false, "This ought to be unreachable.");
        }
    }
}