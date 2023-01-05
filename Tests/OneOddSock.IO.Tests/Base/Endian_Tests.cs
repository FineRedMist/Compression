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
    public unsafe class Endian_Tests
    {
        [TestMethod]
        public void Switch()
        {
            Assert.AreEqual(Endian.Little, Endian.Big.Switch);
            Assert.AreEqual(Endian.Big, Endian.Little.Switch);
        }

        [TestMethod]
        public void IsNative()
        {
            Assert.AreEqual(BitConverter.IsLittleEndian, Endian.Little.IsNative);
            Assert.AreNotEqual(BitConverter.IsLittleEndian, Endian.Big.IsNative);
            Assert.AreEqual(BitConverter.IsLittleEndian, Endian.Little == Endian.Native);
            Assert.AreEqual(BitConverter.IsLittleEndian, Endian.Big == Endian.NonNative);
        }

        [TestMethod]
        public void CopyLinear_NativeNative()
        {
            var data = new byte[] { 0x73, 0x54, 0xf3, 0x1d };
            var output = new byte[data.Length];
            var expected = new byte[data.Length];
            Array.Copy(data, expected, data.Length);

            Endian.Native.To(Endian.Native).BetweenBuffers(output, 0, data, 0, data.Length);
            CollectionAssert.AreEqual(output, expected);
        }

        [TestMethod]
        public void CopyLinear_NonNativeNonNative()
        {
            var data = new byte[] { 0x73, 0x54, 0xf3, 0x1d };
            var output = new byte[data.Length];
            var expected = new byte[data.Length];
            Array.Copy(data, expected, data.Length);

            Endian.NonNative.To(Endian.NonNative).BetweenBuffers(output, 0, data, 0, data.Length);
            CollectionAssert.AreEqual(output, expected);
        }

        [TestMethod]
        public void CopyReverse_NativeNonNative()
        {
            var data = new byte[] { 0x73, 0x54, 0xf3, 0x1d };
            var output = new byte[data.Length];
            var expected = new byte[data.Length];
            Array.Copy(data, expected, data.Length);
            Array.Reverse(expected);

            Endian.Native.To(Endian.NonNative).BetweenBuffers(output, 0, data, 0, data.Length);
            CollectionAssert.AreEqual(output, expected);
        }

        [TestMethod]
        public void CopyReverse_NonNativeNative()
        {
            var data = new byte[] { 0x73, 0x54, 0xf3, 0x1d };
            var output = new byte[data.Length];
            var expected = new byte[data.Length];
            Array.Copy(data, expected, data.Length);
            Array.Reverse(expected);

            Endian.NonNative.To(Endian.Native).BetweenBuffers(output, 0, data, 0, data.Length);
            CollectionAssert.AreEqual(output, expected);
        }

        private byte[] ToBytes_Setup(byte[] target, int index, byte[] dataNativeFormat, bool reverse)
        {
            var expected = new byte[target.Length];
            Array.Copy(expected, target, target.Length);
            if (reverse)
            {
                Array.Reverse(dataNativeFormat);
            }
            Array.Copy(dataNativeFormat, 0, expected, index, dataNativeFormat.Length);
            return expected;
        }

        [TestMethod]
        public void ToBytes_Char_Linear()
        {
            var testValue = (char)0x3f4c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToChar(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Double_Linear()
        {
            double testValue = -12.14358173405891752235;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToDouble(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Float_Linear()
        {
            float testValue = -320.58173405891f;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToSingle(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int64_Linear()
        {
            long testValue = 0x3f235364f30b1a2c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToInt64(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int32_Linear()
        {
            int testValue = 0x3f235364;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToInt32(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int16_Linear()
        {
            short testValue = 0x3f23;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToInt16(expected, 1));
        }

        [TestMethod]
        public void ToBytes_UInt64_Linear()
        {
            ulong testValue = 0xff235364f30b1a2c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToUInt64(expected, 1));
        }

        [TestMethod]
        public void ToBytes_UInt32_Linear()
        {
            uint testValue = 0x3f235364;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToUInt32(expected, 1));
        }

        [TestMethod]
        public void ToBytes_UInt16_Linear()
        {
            ushort testValue = 0x3f23;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), false);
            Endian.Native.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.Native.To(Endian.Native).ToUInt16(expected, 1));
        }


        [TestMethod]
        public void ToBytes_Char_Reverse()
        {
            var testValue = (char)0x3f4c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToChar(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Double_Reverse()
        {
            double testValue = -12.14358173405891752235;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToDouble(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Float_Reverse()
        {
            float testValue = -320.58173405891f;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToSingle(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int64_Reverse()
        {
            long testValue = 0x3f235364f30b1a2c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToInt64(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int32_Reverse()
        {
            int testValue = 0x3f235364;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToInt32(expected, 1));
        }

        [TestMethod]
        public void ToBytes_Int16_Reverse()
        {
            short testValue = 0x3f23;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToInt16(expected, 1));
        }

        [TestMethod]
        public void ToBytes_UInt64_Reverse()
        {
            ulong testValue = 0xff235364f30b1a2c;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToUInt64(expected, 1));
        }

        [TestMethod]
        public void ToBytes_UInt32_Reverse()
        {
            uint testValue = 0x3f235364;
            var target = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            byte[] expected = ToBytes_Setup(target, 1, BitConverter.GetBytes(testValue), true);
            Endian.NonNative.To(Endian.Native).ToBytes(testValue, target, 1);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreEqual(testValue, Endian.NonNative.To(Endian.Native).ToUInt32(expected, 1));
        }

        [TestMethod]
        public void Convert_Char_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (char*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Double_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (double*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Float_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (float*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int64_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (long*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int32_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (int*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int16_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (short*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt64_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (ulong*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt32_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (uint*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt16_Linear()
        {
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            {
                var testValue = (ushort*)&buffer[0];
                Assert.AreEqual(*testValue, Endian.Native.To(Endian.Native).Convert(*testValue));
            }
        }


        [TestMethod]
        public void Convert_Char_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (char*)&buffer[0];
                var actualValue = (char*)&revbuffer[revdata.Length - sizeof(char)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Double_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (double*)&buffer[0];
                var actualValue = (double*)&revbuffer[revdata.Length - sizeof(double)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Float_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (float*)&buffer[0];
                var actualValue = (float*)&revbuffer[revdata.Length - sizeof(float)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int64_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (long*)&buffer[0];
                var actualValue = (long*)&revbuffer[revdata.Length - sizeof(long)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int32_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (int*)&buffer[0];
                var actualValue = (int*)&revbuffer[revdata.Length - sizeof(int)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_Int16_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (short*)&buffer[0];
                var actualValue = (short*)&revbuffer[revdata.Length - sizeof(short)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt64_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (ulong*)&buffer[0];
                var actualValue = (ulong*)&revbuffer[revdata.Length - sizeof(ulong)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt32_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (uint*)&buffer[0];
                var actualValue = (uint*)&revbuffer[revdata.Length - sizeof(uint)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        public void Convert_UInt16_Reverse()
        {
            var revdata = new byte[] { 0x02, 0xb6, 0x9c, 0x4a, 0xd1, 0x11, 0xe3, 0x64, 0x72, 0x23 };
            var data = new byte[] { 0x23, 0x72, 0x64, 0xe3, 0x11, 0xd1, 0x4a, 0x9c, 0xb6, 0x02 };
            fixed (byte* buffer = data)
            fixed (byte* revbuffer = revdata)
            {
                var testValue = (ushort*)&buffer[0];
                var actualValue = (ushort*)&revbuffer[revdata.Length - sizeof(ushort)];
                Assert.AreEqual(*actualValue, Endian.Native.To(Endian.NonNative).Convert(*testValue));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Convert_Linear_Fail_OutOfRange_Destination()
        {
            var target = new byte[1];
            Endian.Native.To(Endian.Native).ToBytes((char)0x3fc2, target, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Convert_Reverse_Fail_OutOfRange_Destination()
        {
            var target = new byte[1];
            Endian.NonNative.To(Endian.Native).ToBytes((char)0x3fc2, target, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Convert_Linear_Fail_OutOfRange_Source()
        {
            var source = new byte[1];
            Endian.Native.To(Endian.Native).ToChar(source, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Convert_Reverse_Fail_OutOfRange_Source()
        {
            var source = new byte[1];
            Endian.NonNative.To(Endian.Native).ToChar(source, 0);
        }
    }
}