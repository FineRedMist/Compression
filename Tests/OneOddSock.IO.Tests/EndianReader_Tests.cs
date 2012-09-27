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
    public class EndianReader_Tests
    {
        private const uint EndianMarker = 0x345a1264;

        private readonly TestValue[] _testValuesWithBits = new TestValue[]
                                                               {
                                                                   new TestValueByte(0x3c),
                                                                   new TestValueUInt64(0x13259fa0d2875641),
                                                                   new TestValueInt64(0x2436a0b1e3986752),
                                                                   new TestValueUInt32(0x3547b1c2),
                                                                   new TestValueBool(true),
                                                                   new TestValueInt32(0x4658c2d3),
                                                                   new TestValueUInt16(0x5769),
                                                                   new TestValueInt16(0x687a),
                                                                   new TestValueDouble(-1235132.1235123512351566),
                                                                   new TestValueFloat((float) 342.213251516),
                                                                   new TestValueBool(false),
                                                                   new TestValueChar((char) 0x2d3c),
                                                                   new TestValueString("This is a test of a string."),
                                                                   new TestValueBytes(new byte[]
                                                                                          {0x63, 0x12, 0xfa, 0xde, 0x11})
                                                                   ,
                                                                   new TestValueChars(new[] {'a', 'b', 'c', 'd', 'e'}),
                                                               };

        private TestValue[] _testValues = new TestValue[]
                                              {
                                                  new TestValueByte(0x3c),
                                                  new TestValueUInt64(0x13259fa0d2875641),
                                                  new TestValueInt64(0x2436a0b1e3986752),
                                                  new TestValueUInt32(0x3547b1c2),
                                                  new TestValueInt32(0x4658c2d3),
                                                  new TestValueUInt16(0x5769),
                                                  new TestValueInt16(0x687a),
                                                  new TestValueDouble(-1235132.1235123512351566),
                                                  new TestValueFloat((float) 342.213251516),
                                                  new TestValueChar((char) 0x2d3c),
                                                  new TestValueString("This is a test of a string."),
                                                  new TestValueBytes(new byte[] {0x63, 0x12, 0xfa, 0xde, 0x11}),
                                                  new TestValueChars(new[] {'a', 'b', 'c', 'd', 'e'}),
                                              };

        [TestMethod]
        public void BinaryWriterCompatible_NativeEndian()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            var reader = new EndianReader(stream);

            foreach (TestValue value in _testValuesWithBits)
            {
                value.Write(writer);
            }

            writer.Flush();
            stream.Position = 0;

            foreach (TestValue value in _testValuesWithBits)
            {
                value.Read(reader);
            }
        }

        [TestMethod]
        public void EndianWriterCompatible_NonNativeEndian()
        {
            var stream = new MemoryStream();
            var writer = new EndianWriter(stream);
            var reader = new EndianReader(stream);

            writer.Endian = reader.Endian = Endian.NonNative;

            foreach (TestValue value in _testValuesWithBits)
            {
                value.Write(writer);
            }

            writer.Flush();
            stream.Position = 0;

            foreach (TestValue value in _testValuesWithBits)
            {
                value.Read(reader);
            }
        }
    }
}