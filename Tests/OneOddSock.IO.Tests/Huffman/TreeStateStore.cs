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

using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OneOddSock.Compression.Huffman.Tests
{
    internal class TreeStateStore : ICollection<TreeEntry>, ICollection
    {
        private readonly List<TreeEntry> entries = new List<TreeEntry>();
        private int currentIndex;

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            (entries as ICollection).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return new object(); }
        }

        #endregion

        #region ICollection<TreeEntry> Members

        public IEnumerator<TreeEntry> GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return entries.GetEnumerator();
        }

        public void Add(TreeEntry item)
        {
            entries.Add(item);
        }

        public void Clear()
        {
            entries.Clear();
        }

        public bool Contains(TreeEntry item)
        {
            return entries.Contains(item);
        }

        public void CopyTo(TreeEntry[] array, int arrayIndex)
        {
            entries.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return entries.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TreeEntry item)
        {
            return entries.Remove(item);
        }

        #endregion

        public void Reset()
        {
            currentIndex = 0;
        }

        public char ReadSymbol()
        {
            Assert.IsTrue(currentIndex < entries.Count);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.Symbol,
                            "Attempting to read a symbol while the current state is: {0}",
                            entries[currentIndex].ToString());
            return entries[currentIndex++]._symbol;
        }

        public uint ReadUInt32()
        {
            Assert.IsTrue(currentIndex < entries.Count);
            Assert.AreEqual(entries[currentIndex]._type, TreeEntryType.UInt32,
                            "Attempting to read a uint while the current state is: {0}",
                            entries[currentIndex].ToString());
            return entries[currentIndex++]._number;
        }

        public void WriteSymbol(char symbol)
        {
            entries.Add(symbol);
            ++currentIndex;
        }

        public void WriteUInt32(uint value)
        {
            entries.Add(value);
            ++currentIndex;
        }

        public void WriteNotYetTransmitted()
        {
            entries.Add(true);
            ++currentIndex;
        }
    }
}