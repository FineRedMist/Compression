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
    public class BitRingBuffer_Tests
    {
        [TestMethod]
        public void InitialState()
        {
            var streamer = new BitRingBuffer(0x100);
            Assert.AreEqual(0x100, streamer.AvailableBytes);
            Assert.AreEqual(0, streamer.UsedBytes);
            Assert.AreEqual(0, streamer.LengthBytes);
            Assert.AreEqual(0, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8, streamer.AvailableBits);
        }

        [TestMethod]
        public void OneBit_Written()
        {
            var streamer = new BitRingBuffer(0x100);
            streamer.Write(true);
            Assert.AreEqual(0x100 - 1, streamer.AvailableBytes);
            Assert.AreEqual(1, streamer.UsedBytes);
            Assert.AreEqual(0, streamer.LengthBytes);
            Assert.AreEqual(1, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8 - 1, streamer.AvailableBits);
        }

        [TestMethod]
        public void TwoBits_Written()
        {
            var streamer = new BitRingBuffer(0x100);
            streamer.Write(true);
            streamer.Write(false);
            Assert.AreEqual(0x100 - 1, streamer.AvailableBytes);
            Assert.AreEqual(1, streamer.UsedBytes);
            Assert.AreEqual(0, streamer.LengthBytes);
            Assert.AreEqual(2, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8 - 2, streamer.AvailableBits);
        }

        [TestMethod]
        public void Seven_Bits_Written()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 7; ++i)
            {
                streamer.Write(true);
            }
            Assert.AreEqual(0x100 - 1, streamer.AvailableBytes);
            Assert.AreEqual(1, streamer.UsedBytes);
            Assert.AreEqual(0, streamer.LengthBytes);
            Assert.AreEqual(7, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8 - 7, streamer.AvailableBits);
        }

        [TestMethod]
        public void Eight_Bits_Written()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 8; ++i)
            {
                streamer.Write(true);
            }
            Assert.AreEqual(0x100 - 1, streamer.AvailableBytes);
            Assert.AreEqual(1, streamer.UsedBytes);
            Assert.AreEqual(1, streamer.LengthBytes);
            Assert.AreEqual(8, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8 - 8, streamer.AvailableBits);
        }

        [TestMethod]
        public void Nine_Bits_Written()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 9; ++i)
            {
                streamer.Write(true);
            }
            Assert.AreEqual(0x100 - 2, streamer.AvailableBytes);
            Assert.AreEqual(2, streamer.UsedBytes);
            Assert.AreEqual(1, streamer.LengthBytes);
            Assert.AreEqual(9, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8 - 9, streamer.AvailableBits);
        }

        [TestMethod]
        public void Almost_Full()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8 - 1; ++i)
            {
                streamer.Write(true);
            }
            Assert.AreEqual(0, streamer.AvailableBytes);
            Assert.AreEqual(0x100, streamer.UsedBytes);
            Assert.AreEqual(0x100 - 1, streamer.LengthBytes);
            Assert.AreEqual(0x100 * 8 - 1, streamer.LengthBits);
            Assert.AreEqual(1, streamer.AvailableBits);
        }

        [TestMethod]
        public void Full()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8; ++i)
            {
                streamer.Write(true);
            }
            Assert.AreEqual(0, streamer.AvailableBytes);
            Assert.AreEqual(0x100, streamer.UsedBytes);
            Assert.AreEqual(0x100, streamer.LengthBytes);
            Assert.AreEqual(0x100 * 8, streamer.LengthBits);
            Assert.AreEqual(0, streamer.AvailableBits);
        }

        [TestMethod]
        public void ReadOneBit_After_Full()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8; ++i)
            {
                streamer.Write(true);
            }
            streamer.ReadBoolean();
            Assert.AreEqual(0, streamer.AvailableBytes);
            Assert.AreEqual(0x100, streamer.UsedBytes);
            Assert.AreEqual(0x100 - 1, streamer.LengthBytes);
            Assert.AreEqual(0x100 * 8 - 1, streamer.LengthBits);
            Assert.AreEqual(1, streamer.AvailableBits);
        }

        [TestMethod]
        public void ReadAll_After_Full()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8; ++i)
            {
                streamer.Write(true);
            }
            for (int i = 0; i < 0x100 * 8; ++i)
            {
                streamer.ReadBoolean();
            }
            Assert.AreEqual(0x100, streamer.AvailableBytes);
            Assert.AreEqual(0, streamer.UsedBytes);
            Assert.AreEqual(0, streamer.LengthBytes);
            Assert.AreEqual(0, streamer.LengthBits);
            Assert.AreEqual(0x100 * 8, streamer.AvailableBits);
        }

        [TestMethod]
        public void ReadByte_BitByBit()
        {
            byte b = 0xc7; // 11000111
            var values = new[] { false, true, true, false, false, false, true, true, true };
            var streamer = new BitRingBuffer(0x100);
            streamer.Write(false);
            streamer.Write(b);
            for (int i = 0; i < 8; ++i)
            {
                Assert.AreEqual(values[i], streamer.ReadBoolean());
            }
        }

        [TestMethod]
        public void ReadFullBuffer_BitByBit()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8; ++i)
            {
                streamer.Write((i * (i % 7)) % 2 == 1);
            }

            for (int i = 0; i < 0x100 * 8; ++i)
            {
                Assert.AreEqual((i * (i % 7)) % 2 == 1, streamer.ReadBoolean());
            }
        }

        [TestMethod]
        public void WriteByteArray()
        {
            var streamer = new BitRingBuffer(0x100);
            var buffer = new byte[0x100];
            for (int i = 0; i < 0x100; ++i)
            {
                buffer[i] = (byte)(((i + 123) * (i + 7)) % 256);
            }
            streamer.Write(buffer, 0, 0x100);
            var result = new byte[0x100];
            streamer.ReadBytes(result, 0, 0x100);
            CollectionAssert.AreEqual(buffer, result);
        }

        /// <summary>
        /// Ensures the index wraps around the buffer correctly.
        /// </summary>
        [TestMethod]
        public void EnsureIndexWrap()
        {
            var streamer = new BitRingBuffer(2);
            streamer.Write(0x3c);
            for (int i = 0; i < 7; ++i)
            {
                streamer.Write(true);
            }
            byte result = streamer.ReadByte();
            Assert.AreEqual((byte)0x3c, result);
            streamer.Write(false); // End of buffer
            streamer.Write(true); // Wrap
            result = streamer.ReadByte();
            Assert.AreEqual((byte)0xfe, result);
            Assert.AreEqual(true, streamer.ReadBoolean());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Overread_Bit()
        {
            var streamer = new BitRingBuffer(0x100);
            streamer.ReadBoolean();
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Overwrite_Bit()
        {
            var streamer = new BitRingBuffer(0x100);
            for (int i = 0; i < 0x100 * 8 + 1; ++i)
            {
                streamer.Write(true);
            }
        }
    }
}