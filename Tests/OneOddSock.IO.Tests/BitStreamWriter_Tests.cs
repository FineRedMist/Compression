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
    public class BitStreamWriter_Tests
    {
        [TestMethod]
        public void WriteByte()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.WriteByte(0x3c);

                Assert.AreEqual(1, writer.Length);
                Assert.AreEqual(8, writer.BitLength);
                Assert.AreEqual(8, writer.BitPosition);
                Assert.AreEqual(1, writer.Position);
            }

            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0x3c, stream.GetBuffer()[0]);
        }

        [TestMethod]
        public void WriteBit()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Write(true);

                Assert.AreEqual(1, writer.Length);
                Assert.AreEqual(1, writer.BitLength);
                Assert.AreEqual(1, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);
            }

            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0x80, stream.GetBuffer()[0]);
        }

        [TestMethod]
        public void WriteSevenBits()
        {
            int bitCount = 7;
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);


                for (int i = 0; i < bitCount; ++i)
                {
                    writer.Write(true);
                }

                Assert.AreEqual((bitCount + 7) >> 3, writer.Length);
                Assert.AreEqual(bitCount, writer.BitLength);
                Assert.AreEqual(bitCount, writer.BitPosition);
                Assert.AreEqual(bitCount >> 3, writer.Position);
            }
        }

        [TestMethod]
        public void WriteEightBits()
        {
            int bitCount = 8;
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);


                for (int i = 0; i < bitCount; ++i)
                {
                    writer.Write(true);
                }

                Assert.AreEqual((bitCount + 7) >> 3, writer.Length);
                Assert.AreEqual(bitCount, writer.BitLength);
                Assert.AreEqual(bitCount, writer.BitPosition);
                Assert.AreEqual(bitCount >> 3, writer.Position);
            }
        }

        [TestMethod]
        public void WriteBit_Seek_WriteBit()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Write(true);

                Assert.AreEqual(1, writer.Length);
                Assert.AreEqual(1, writer.BitLength);
                Assert.AreEqual(1, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(1, stream.Length);
                Assert.AreEqual(0, stream.Position);
                Assert.AreEqual(0x80, stream.GetBuffer()[0]);

                writer.Write(false);
            }
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0, stream.GetBuffer()[0]);
        }

        [TestMethod]
        public void WriteArray()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var expected = new byte[] { 0x9e, 0x25, 0x8d, 0x2a, 0x11, 0x08, 0x57, 0xc4, 0xcb, 0xb2, 0x80 };

            var result = new byte[expected.Length];

            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Write(true);
                writer.Write(data, 0, data.Length);

                Assert.AreEqual(11, writer.Length);
                Assert.AreEqual(81, writer.BitLength);
                Assert.AreEqual(81, writer.BitPosition);
                Assert.AreEqual(10, writer.Position);
            }

            Array.Copy(stream.GetBuffer(), 0, result, 0, expected.Length);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SetLength()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.SetLength(10);
                writer.Write(true);

                Assert.AreEqual(10, writer.Length);
                Assert.AreEqual(80, writer.BitLength);
                Assert.AreEqual(1, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);
            }

            Assert.AreEqual(10, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0x80, stream.GetBuffer()[0]);
        }

        [TestMethod]
        public void SetLength_SetPosition()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.SetLength(10);
                writer.Write(true);
                writer.Position = 5;
                writer.Write(true);

                Assert.AreEqual(10, writer.Length);
                Assert.AreEqual(80, writer.BitLength);
                Assert.AreEqual(41, writer.BitPosition);
                Assert.AreEqual(5, writer.Position);
            }

            Assert.AreEqual(10, stream.Length);
            Assert.AreEqual(6, stream.Position);
            Assert.AreEqual(0x80, stream.GetBuffer()[0]);
            Assert.AreEqual(0x80, stream.GetBuffer()[5]);
        }

        [TestMethod]
        public void leaveOpen_True()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, true))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Write(true);

                Assert.AreEqual(1, writer.Length);
                Assert.AreEqual(1, writer.BitLength);
                Assert.AreEqual(1, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(1, stream.Length);
                Assert.AreEqual(0, stream.Position);
                Assert.AreEqual(0x80, stream.GetBuffer()[0]);

                writer.Write(false);
            }
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0, stream.GetBuffer()[0]);

            using (var writer = new BitStreamWriter(stream, false))
            {
                var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
                writer.Write(data, 0, data.Length);
                writer.Flush();

                Assert.AreEqual(11, stream.Length);
                Assert.AreEqual(11, stream.Position);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void leaveOpen_False()
        {
            var stream = new MemoryStream();
            using (var writer = new BitStreamWriter(stream, false))
            {
                Assert.AreEqual(false, writer.CanRead);
                Assert.AreEqual(true, writer.CanWrite);
                Assert.AreEqual(true, writer.CanSeek);

                Assert.AreEqual(0, writer.Length);
                Assert.AreEqual(0, writer.BitLength);
                Assert.AreEqual(0, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Write(true);

                Assert.AreEqual(1, writer.Length);
                Assert.AreEqual(1, writer.BitLength);
                Assert.AreEqual(1, writer.BitPosition);
                Assert.AreEqual(0, writer.Position);

                writer.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(1, stream.Length);
                Assert.AreEqual(0, stream.Position);
                Assert.AreEqual(0x80, stream.GetBuffer()[0]);

                writer.Write(false);
            }
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0, stream.GetBuffer()[0]);

            using (var writer = new BitStreamWriter(stream, false))
            {
                var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
                writer.Write(data, 0, data.Length);
                writer.Flush();

                Assert.AreEqual(11, stream.Length);
                Assert.AreEqual(11, stream.Position);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReadBoolean_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            using (var writer = new BitStreamWriter(stream, false))
            {
                writer.ReadBoolean();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReadByte_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            using (var writer = new BitStreamWriter(stream, false))
            {
                writer.ReadByte();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ReadArray_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            using (var writer = new BitStreamWriter(stream, false))
            {
                var result = new byte[10];
                writer.Read(result, 0, result.Length);
            }
        }
    }
}