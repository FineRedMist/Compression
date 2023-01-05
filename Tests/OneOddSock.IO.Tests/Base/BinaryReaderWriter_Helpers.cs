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

namespace BitStreamTests
{
    public interface TestValue
    {
        void Write(BinaryWriter writer);
        void Read(BinaryReader reader);
    };

    public class TestValueBool : TestValue
    {
        private readonly bool _value;

        public TestValueBool(bool value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadBoolean());
        }

        #endregion
    }

    public class TestValueByte : TestValue
    {
        private readonly byte _value;

        public TestValueByte(byte value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadByte());
        }

        #endregion
    }

    public class TestValueInt16 : TestValue
    {
        private readonly short _value;

        public TestValueInt16(short value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadInt16());
        }

        #endregion
    }

    public class TestValueUInt16 : TestValue
    {
        private readonly ushort _value;

        public TestValueUInt16(ushort value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadUInt16());
        }

        #endregion
    }

    public class TestValueInt32 : TestValue
    {
        private readonly int _value;

        public TestValueInt32(int value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadInt32());
        }

        #endregion
    }

    public class TestValueUInt32 : TestValue
    {
        private readonly uint _value;

        public TestValueUInt32(uint value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadUInt32());
        }

        #endregion
    }

    public class TestValueInt64 : TestValue
    {
        private readonly long _value;

        public TestValueInt64(long value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadInt64());
        }

        #endregion
    }

    public class TestValueUInt64 : TestValue
    {
        private readonly ulong _value;

        public TestValueUInt64(ulong value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadUInt64());
        }

        #endregion
    }

    public class TestValueFloat : TestValue
    {
        private readonly float _value;

        public TestValueFloat(float value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadSingle());
        }

        #endregion
    }

    public class TestValueDouble : TestValue
    {
        private readonly double _value;

        public TestValueDouble(double value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadDouble());
        }

        #endregion
    }

    public class TestValueString : TestValue
    {
        private readonly string _value;

        public TestValueString(string value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadString());
        }

        #endregion
    }

    public class TestValueChar : TestValue
    {
        private readonly char _value;

        public TestValueChar(char value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            Assert.AreEqual(_value, reader.ReadChar());
        }

        #endregion
    }

    public class TestValueBytes : TestValue
    {
        private readonly byte[] _value;

        public TestValueBytes(byte[] value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            CollectionAssert.AreEqual(_value, reader.ReadBytes(_value.Length));
        }

        #endregion
    }

    public class TestValueChars : TestValue
    {
        private readonly char[] _value;

        public TestValueChars(char[] value)
        {
            _value = value;
        }

        #region TestValue Members

        public void Write(BinaryWriter writer)
        {
            writer.Write(_value);
        }

        public void Read(BinaryReader reader)
        {
            CollectionAssert.AreEqual(_value, reader.ReadChars(_value.Length));
        }

        #endregion
    }
}