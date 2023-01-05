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

namespace OneOddSock.Compression.Huffman.Tests
{
    public enum TreeEntryType
    {
        Symbol,
        UInt32,
        Boolean
    }

    public struct TreeEntry : IEquatable<TreeEntry>
    {
        public bool _boolean;
        public uint _number;
        public char _symbol;
        public TreeEntryType _type;

        public static implicit operator TreeEntry(char symbol)
        {
            TreeEntry result;
            result._type = TreeEntryType.Symbol;
            result._symbol = symbol;
            result._number = 0;
            result._boolean = false;
            return result;
        }

        public static implicit operator TreeEntry(uint value)
        {
            TreeEntry result;
            result._type = TreeEntryType.UInt32;
            result._number = value;
            result._symbol = char.MinValue;
            result._boolean = false;
            return result;
        }

        public static implicit operator TreeEntry(bool value)
        {
            TreeEntry result;
            result._type = TreeEntryType.Boolean;
            result._boolean = value;
            result._number = 0;
            result._symbol = char.MinValue;
            return result;
        }

        public override string ToString()
        {
            switch (_type)
            {
                case TreeEntryType.Boolean:
                    return _boolean.ToString();
                case TreeEntryType.Symbol:
                    return "'" + _symbol.ToString() + "'";
                case TreeEntryType.UInt32:
                    return _number.ToString();
            }
            ;
            return "<invalid>";
        }

        #region IEquatable implementation

        public bool Equals(TreeEntry other)
        {
            return _type == other._type
                   && ((_type == TreeEntryType.Symbol && _symbol == other._symbol)
                       || (_type == TreeEntryType.UInt32 && _number == other._number)
                       || (_type == TreeEntryType.Boolean && _boolean == other._boolean));
        }

        #endregion
    };

    internal class TreeState
    {
        private readonly TreeEntry[] entries;
        private int currentIndex;

        public TreeState(params TreeEntry[] args)
        {
            entries = args;
            currentIndex = 0;
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        public char ReadSymbol()
        {
            Assert.IsTrue(currentIndex < entries.Length);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.Symbol,
                            "Attempting to read a symbol while the current state is: {0}",
                            entries[currentIndex].ToString());
            return entries[currentIndex++]._symbol;
        }

        public uint ReadUInt32()
        {
            Assert.IsTrue(currentIndex < entries.Length);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.UInt32,
                            "Attempting to read a uint while the current state is: {0}",
                            entries[currentIndex].ToString());
            return entries[currentIndex++]._number;
        }

        public void WriteSymbol(char symbol)
        {
            Assert.IsTrue(currentIndex < entries.Length);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.Symbol, "Received {0} instead of {1}", symbol,
                            entries[currentIndex].ToString());
            Assert.AreEqual(entries[currentIndex]._symbol, symbol, "Received {0} instead of {1}", symbol,
                            entries[currentIndex].ToString());
            ++currentIndex;
        }

        public void WriteUInt32(uint value)
        {
            Assert.IsTrue(currentIndex < entries.Length);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.UInt32, "Received {0} instead of {1}", value,
                            entries[currentIndex].ToString());
            Assert.AreEqual(entries[currentIndex]._number, value, "Received {0} instead of {1}", value,
                            entries[currentIndex].ToString());
            ++currentIndex;
        }

        public void WriteNotYetTransmitted()
        {
            Assert.IsTrue(currentIndex < entries.Length);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.Boolean, "Received {0} instead of {1}", true,
                            entries[currentIndex].ToString());
            Assert.AreEqual(entries[currentIndex]._boolean, true, "Received {0} instead of {1}", true,
                            entries[currentIndex].ToString());
            ++currentIndex;
        }

        public void Final()
        {
            Assert.AreEqual(currentIndex, entries.Length);
        }
    }
}