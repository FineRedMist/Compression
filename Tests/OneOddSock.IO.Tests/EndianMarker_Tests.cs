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
    public class EndianMarker_Tests
    {
        private const uint EndianMarker_Little = 0x3245af2d;
        private const uint EndianMarker_Big = 0x2daf4532;

        [TestMethod]
        public void LittleEndian_LittleNative()
        {
            var data = new byte[] {0x2d, 0xaf, 0x45, 0x32, 0x1a, 0x2b, 0x3c, 0x4d};
            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            Assert.AreEqual(reader, reader.ProcessEndianMarker(EndianMarker_Little));
            Assert.AreEqual((uint) 0x4d3c2b1a, reader.ReadUInt32());
        }

        [TestMethod]
        public void BigEndian_LittleNative()
        {
            var data = new byte[] {0x32, 0x45, 0xaf, 0x2d, 0x1a, 0x2b, 0x3c, 0x4d};
            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            var ereader = reader.ProcessEndianMarker(EndianMarker_Little) as EndianReader;
            Assert.AreNotEqual(reader, ereader);
            Assert.AreEqual(Endian.Big, ereader.Endian);
            Assert.AreEqual((uint) 0x1a2b3c4d, ereader.ReadUInt32());
        }

        [TestMethod]
        public void BigEndian_BigNative()
        {
            var data = new byte[] {0x2d, 0xaf, 0x45, 0x32, 0x1a, 0x2b, 0x3c, 0x4d};
            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            var ereader = reader.ProcessEndianMarker(EndianMarker_Big) as EndianReader;
            Assert.AreNotEqual(reader, ereader);
            Assert.AreEqual((uint) 0x1a2b3c4d, ereader.ReadUInt32());
        }

        [TestMethod]
        public void LittleEndian_BigNative()
        {
            var data = new byte[] {0x32, 0x45, 0xaf, 0x2d, 0x1a, 0x2b, 0x3c, 0x4d};
            var stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            BinaryReader ereader = reader.ProcessEndianMarker(EndianMarker_Big);
            Assert.AreEqual(reader, ereader);
            Assert.AreEqual((uint) 0x4d3c2b1a, ereader.ReadUInt32());
        }
    }
}