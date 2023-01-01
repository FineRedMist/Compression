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
    public class BitStreamReader_Tests
    {
        [TestMethod]
        public void OneBitRead()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            Assert.AreEqual(false, reader.ReadBoolean());

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(1, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);
        }

        [TestMethod]
        public void SevenBitsRead()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(7, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);
        }

        [TestMethod]
        public void EightBitsRead()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(8, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(1, reader.Position);
        }

        [TestMethod]
        public void EightBitsAndOneByteRead()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());

            Assert.AreEqual(0x4b, reader.ReadByte());

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(16, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(2, reader.Position);
        }

        [TestMethod]
        public void CompletelyRead()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());
            Assert.AreEqual(false, reader.ReadBoolean());

            Assert.AreEqual(0x4b, reader.ReadByte());

            var expected = new byte[8];
            Array.Copy(data, 2, expected, 0, 8);
            var result = new byte[8];
            int toRead = 8;
            while (toRead > 0)
            {
                int read = reader.Read(result, 8 - toRead, toRead);
                toRead -= read;
            }

            CollectionAssert.AreEqual(expected, result);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(80, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(10, reader.Position);
        }

        [TestMethod]
        public void Seek()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            reader.Seek(7, SeekOrigin.Begin);

            Assert.AreEqual(56, reader.BitPosition);
            Assert.AreEqual(7, reader.Position);

            Assert.AreEqual(0x89, reader.ReadByte());
            Assert.AreEqual(true, reader.ReadBoolean());

            Assert.AreEqual(7, reader.Position = 7);

            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(0x13, reader.ReadByte());

            var buffer = new byte[10];
            Assert.AreEqual(1, reader.Read(buffer, 0, 10));
            Assert.AreEqual(0x2e, buffer[0]);
            Assert.AreEqual(-1, reader.ReadByte());

            Assert.AreEqual(73, reader.BitPosition);
            Assert.AreEqual(9, reader.Position);
        }

        [TestMethod]
        public void SetPosition()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            Assert.AreEqual(true, reader.CanRead);
            Assert.AreEqual(true, reader.CanSeek);
            Assert.AreEqual(false, reader.CanWrite);

            Assert.AreEqual(80, reader.BitLength);
            Assert.AreEqual(0, reader.BitPosition);
            Assert.AreEqual(10, reader.Length);
            Assert.AreEqual(0, reader.Position);

            reader.Position = 7;

            Assert.AreEqual(56, reader.BitPosition);
            Assert.AreEqual(7, reader.Position);

            Assert.AreEqual(0x89, reader.ReadByte());
            Assert.AreEqual(true, reader.ReadBoolean());

            Assert.AreEqual(7, reader.Position = 7);

            Assert.AreEqual(true, reader.ReadBoolean());
            Assert.AreEqual(0x13, reader.ReadByte());

            var buffer = new byte[10];
            Assert.AreEqual(1, reader.Read(buffer, 0, 10));
            Assert.AreEqual(0x2e, buffer[0]);
            Assert.AreEqual(-1, reader.ReadByte());

            Assert.AreEqual(73, reader.BitPosition);
            Assert.AreEqual(9, reader.Position);
        }

        [TestMethod]
        public void LeaveOpen_True()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            using (var reader = new BitStreamReader(stream, true))
            {
                Assert.AreEqual(true, reader.CanRead);
                Assert.AreEqual(true, reader.CanSeek);
                Assert.AreEqual(false, reader.CanWrite);

                Assert.AreEqual(80, reader.BitLength);
                Assert.AreEqual(0, reader.BitPosition);
                Assert.AreEqual(10, reader.Length);
                Assert.AreEqual(0, reader.Position);

                reader.Seek(7, SeekOrigin.Begin);

                Assert.AreEqual(56, reader.BitPosition);
                Assert.AreEqual(7, reader.Position);

                Assert.AreEqual(0x89, reader.ReadByte());
                Assert.AreEqual(true, reader.ReadBoolean());

                Assert.AreEqual(7, reader.Position = 7);

                Assert.AreEqual(true, reader.ReadBoolean());
                Assert.AreEqual(0x13, reader.ReadByte());

                var buffer = new byte[10];
                Assert.AreEqual(1, reader.Read(buffer, 0, 10));
                Assert.AreEqual(0x2e, buffer[0]);
                Assert.AreEqual(-1, reader.ReadByte());

                Assert.AreEqual(73, reader.BitPosition);
                Assert.AreEqual(9, reader.Position);
            }
            stream.Position = 0;
            using (var reader = new BitStreamReader(stream, false))
            {
                Assert.AreEqual(true, reader.CanRead);
                Assert.AreEqual(true, reader.CanSeek);
                Assert.AreEqual(false, reader.CanWrite);

                Assert.AreEqual(80, reader.BitLength);
                Assert.AreEqual(0, reader.BitPosition);
                Assert.AreEqual(10, reader.Length);
                Assert.AreEqual(0, reader.Position);

                reader.Seek(7, SeekOrigin.Begin);

                Assert.AreEqual(56, reader.BitPosition);
                Assert.AreEqual(7, reader.Position);

                Assert.AreEqual(0x89, reader.ReadByte());
                Assert.AreEqual(true, reader.ReadBoolean());

                Assert.AreEqual(7, reader.Position = 7);

                Assert.AreEqual(true, reader.ReadBoolean());
                Assert.AreEqual(0x13, reader.ReadByte());

                var buffer = new byte[10];
                Assert.AreEqual(1, reader.Read(buffer, 0, 10));
                Assert.AreEqual(0x2e, buffer[0]);
                Assert.AreEqual(-1, reader.ReadByte());

                Assert.AreEqual(73, reader.BitPosition);
                Assert.AreEqual(9, reader.Position);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void LeaveOpen_False_ObjectDisposed()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            using (var reader = new BitStreamReader(stream, false))
            {
                Assert.AreEqual(true, reader.CanRead);
                Assert.AreEqual(true, reader.CanSeek);
                Assert.AreEqual(false, reader.CanWrite);

                Assert.AreEqual(80, reader.BitLength);
                Assert.AreEqual(0, reader.BitPosition);
                Assert.AreEqual(10, reader.Length);
                Assert.AreEqual(0, reader.Position);

                reader.Seek(7, SeekOrigin.Begin);

                Assert.AreEqual(56, reader.BitPosition);
                Assert.AreEqual(7, reader.Position);

                Assert.AreEqual(0x89, reader.ReadByte());
                Assert.AreEqual(true, reader.ReadBoolean());

                Assert.AreEqual(7, reader.Position = 7);

                Assert.AreEqual(true, reader.ReadBoolean());
                Assert.AreEqual(0x13, reader.ReadByte());

                var buffer = new byte[10];
                Assert.AreEqual(1, reader.Read(buffer, 0, 10));
                Assert.AreEqual(0x2e, buffer[0]);
                Assert.AreEqual(-1, reader.ReadByte());

                Assert.AreEqual(73, reader.BitPosition);
                Assert.AreEqual(9, reader.Position);
            }
            stream.Position = 0;
            using (var reader = new BitStreamReader(stream, false))
            {
                Assert.AreEqual(true, reader.CanRead);
                Assert.AreEqual(true, reader.CanSeek);
                Assert.AreEqual(false, reader.CanWrite);

                Assert.AreEqual(80, reader.BitLength);
                Assert.AreEqual(0, reader.BitPosition);
                Assert.AreEqual(10, reader.Length);
                Assert.AreEqual(0, reader.Position);

                reader.Seek(7, SeekOrigin.Begin);

                Assert.AreEqual(56, reader.BitPosition);
                Assert.AreEqual(7, reader.Position);

                Assert.AreEqual(0x89, reader.ReadByte());
                Assert.AreEqual(true, reader.ReadBoolean());

                Assert.AreEqual(7, reader.Position = 7);

                Assert.AreEqual(true, reader.ReadBoolean());
                Assert.AreEqual(0x13, reader.ReadByte());

                var buffer = new byte[10];
                Assert.AreEqual(1, reader.Read(buffer, 0, 10));
                Assert.AreEqual(0x2e, buffer[0]);
                Assert.AreEqual(-1, reader.ReadByte());

                Assert.AreEqual(73, reader.BitPosition);
                Assert.AreEqual(9, reader.Position);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void WriteBoolean_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            reader.Write(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void WriteByte_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            reader.WriteByte(0x31);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void WriteBuffer_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            reader.Write(new byte[] { 0x35, 0x12 }, 0, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetLength_NotSupported()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);
            reader.SetLength(12);
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ReadBoolean_EndOfStream()
        {
            var data = new byte[] { 0x3c, 0x4b, 0x1a, 0x54, 0x22, 0x10, 0xaf, 0x89, 0x97, 0x65 };
            var stream = new MemoryStream(data);
            var reader = new BitStreamReader(stream);

            reader.Seek(10, SeekOrigin.Begin);
            reader.ReadBoolean();
        }
    }
}