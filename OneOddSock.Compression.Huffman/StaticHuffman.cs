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

namespace OneOddSock.Compression.Huffman
{
    /// <summary>
    /// Static Huffman table implementation based on the predetermined frequencies of <typeparamref name="TSymbolType"/>.
    /// </summary>
    public class StaticHuffman<TSymbolType> where TSymbolType : struct, IComparable<TSymbolType>
    {
        private readonly Entry[] _entries;
        private readonly uint _height;
        private readonly IDictionary<TSymbolType, SymbolInfo> _map;
        private readonly uint _symbolCount;

        /// <summary>
        /// Creates a static Huffman tree for encoding/decoding based on the frequencies for a given set of <typeparamref name="TSymbolType"/>.
        /// </summary>
        public StaticHuffman(IDictionary<TSymbolType, uint> symbolWeights)
        {
            _map = new Dictionary<TSymbolType, SymbolInfo>(symbolWeights.Count);

            List<Entry> symbols = GetSymbolList(symbolWeights);
            _symbolCount = (uint)symbols.Count;
            _entries = new Entry[2 * _symbolCount - 1];

            for (int i = 0; i < symbols.Count; ++i)
            {
                _entries[i] = symbols[i];
            }

            BuildInternalNodes();

            PopulateMap(symbols);

            _height = GetHeight();
        }

        /// <summary>
        /// Creates a static Huffman tree for encoding/decoding based on a tree emitted from WriteTable.
        /// </summary>
        /// <param name="symbolReader">Delegate for reading symbols.</param>
        /// <param name="valueReader">Delegate for reading unsigned integers.</param>
        public StaticHuffman(ReadSymbolDelegate<TSymbolType> symbolReader, ReadUInt32Delegate valueReader)
        {
            _symbolCount = valueReader();
            _entries = new Entry[2 * _symbolCount - 1];
            _map = new Dictionary<TSymbolType, SymbolInfo>((int)_symbolCount);

            for (uint i = 0; i < _symbolCount; ++i)
            {
                TSymbolType symbol = symbolReader();
                _entries[i].Symbol = symbol;
                SymbolInfo info;
                info.Index = i;
                info.Bits = null;
                _map[symbol] = info;
            }

            for (uint i = _symbolCount; i < _entries.Length; ++i)
            {
                uint v1 = valueReader();
                uint v2 = valueReader();
                _entries[i].ChildIndex = new uint[2] { v1, v2 };
            }

            ComputeParents();

            _height = GetHeight();

            Validate();
        }

        /// <summary>
        /// The number of symbols in the Huffman tree.
        /// </summary>
        public uint SymbolCount
        {
            get { return _symbolCount; }
        }

        /// <summary>
        /// The height of the Huffman tree.
        /// </summary>
        public uint Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Reads a symbol using the bits retrieved with <paramref name="bitReader"/>.
        /// </summary>
        public TSymbolType GetSymbol(ReadBitDelegate bitReader)
        {
            uint current = (uint)_entries.Length - 1;
            while (!_entries[current].IsSymbol)
            {
                current = _entries[current].GetChild(bitReader());
            }
            return _entries[current].Symbol;
        }

        /// <summary>
        /// Writes the bit representation of <paramref name="symbol"/> with <paramref name="bitWriter"/>.
        /// </summary>
        public void WriteCode(TSymbolType symbol, WriteBitDelegate bitWriter)
        {
            BitArray bits = GetCachedBits(symbol);
            for (int i = 0; i < bits.Length; ++i)
            {
                bitWriter(bits[i]);
            }
        }

        /// <summary>
        /// Writes the Huffman tree for future decoding use.
        /// </summary>
        public void WriteTable(WriteSymbolDelegate<TSymbolType> symbolWriter, WriteUInt32Delegate valueWriter)
        {
            valueWriter(_symbolCount);

            for (uint i = 0; i < _symbolCount; ++i)
            {
                symbolWriter(_entries[i].Symbol);
            }

            for (uint i = _symbolCount; i < _entries.Length; ++i)
            {
                uint[] entry = _entries[i].ChildIndex!;
                valueWriter(entry[0]);
                valueWriter(entry[1]);
            }
        }

        private BitArray GetCachedBits(TSymbolType symbol)
        {
            SymbolInfo info = _map[symbol];
            if (info.Bits != null)
            {
                return info.Bits;
            }
            var result = new BitArray((int)Height);
            uint currentIndex = 0;
            UpdateCachedBits(info.Index, result, ref currentIndex);
            result.Length = (int)currentIndex;
            info.Bits = result;
            _map[symbol] = info;
            return result;
        }

        private void UpdateCachedBits(uint index, BitArray bits, ref uint currentIndex)
        {
            uint parent = _entries[index].Parent;
            bool output = _entries[parent].GetChildIndex(index);
            if (parent != _entries.Length - 1)
            {
                UpdateCachedBits(parent, bits, ref currentIndex);
            }
            bits[(int)currentIndex] = output;
            ++currentIndex;
        }

        private uint GetHeight()
        {
            var levels = new uint[_entries.Length];
            uint highestLevel = 0;
            var toProcess = new Stack<uint>();
            toProcess.Push((uint)(_entries.Length - 1));
            while (toProcess.Count > 0)
            {
                uint current = toProcess.Pop();
                uint currentLevel = levels[current];
                highestLevel = Math.Max(highestLevel, currentLevel + 1);
                if (!_entries[current].IsSymbol)
                {
                    uint[] children = _entries[current].ChildIndex!;
                    levels[children[0]] = currentLevel + 1;
                    levels[children[1]] = currentLevel + 1;
                    toProcess.Push(children[0]);
                    toProcess.Push(children[1]);
                }
            }
            return highestLevel + 1;
        }

        private void BuildInternalNodes()
        {
            uint symbolIndex = 0;
            uint nodeIndex = uint.MaxValue;
            uint targetIndex = _symbolCount;

            var selectedWeight = new uint[2];

            while (targetIndex < _entries.Length)
            {
                var selectedIndex = new uint[2];
                for (int selection = 0; selection < 2; ++selection)
                {
                    if (symbolIndex >= _symbolCount)
                    {
                        uint index = nodeIndex + _symbolCount;
                        selectedIndex[selection] = index;
                        selectedWeight[selection] = _entries[index].Weight;
                        ++nodeIndex;
                    }
                    else if (nodeIndex > _entries.Length)
                    {
                        selectedIndex[selection] = symbolIndex;
                        selectedWeight[selection] = _entries[symbolIndex].Weight;
                        ++symbolIndex;
                    }
                    else if (_entries[symbolIndex].Weight <= _entries[nodeIndex + _symbolCount].Weight)
                    {
                        selectedIndex[selection] = symbolIndex;
                        selectedWeight[selection] = _entries[symbolIndex].Weight;
                        ++symbolIndex;
                    }
                    else
                    {
                        uint index = nodeIndex + _symbolCount;
                        selectedIndex[selection] = index;
                        selectedWeight[selection] = _entries[index].Weight;
                        ++nodeIndex;
                    }
                }

                _entries[targetIndex].ChildIndex = selectedIndex;
                _entries[targetIndex].Weight = selectedWeight[0] + selectedWeight[1];
                _entries[selectedIndex[0]].Parent = targetIndex;
                _entries[selectedIndex[1]].Parent = targetIndex;
                nodeIndex = nodeIndex == uint.MaxValue ? 0 : nodeIndex;
                ++targetIndex;
            }
        }

        private void ComputeParents()
        {
            var toProcess = new Stack<uint>();
            toProcess.Push((uint)(_entries.Length - 1));
            while (toProcess.Count > 0)
            {
                uint current = toProcess.Pop();
                if (!_entries[current].IsSymbol)
                {
                    uint[] children = _entries[current].ChildIndex!;
                    _entries[children[0]].Parent = current;
                    _entries[children[1]].Parent = current;
                    toProcess.Push(children[0]);
                    toProcess.Push(children[1]);
                }
            }
        }

        /// <summary>
        /// Generates a list of Entry instances for each symbol in the dictionary in sorted order.
        /// </summary>
        /// <param name="symbolWeights"></param>
        /// <returns></returns>
        private List<Entry> GetSymbolList(IDictionary<TSymbolType, uint> symbolWeights)
        {
            var results = new List<Entry>(symbolWeights.Count);

            foreach (var symbolWeight in symbolWeights)
            {
                if (symbolWeight.Value > 0)
                {
                    Entry entry;
                    entry.Symbol = symbolWeight.Key;
                    entry.Weight = symbolWeight.Value;
                    entry.ChildIndex = null;
                    entry.Parent = 0;
                    results.Add(entry);
                }
            }

            results.Sort(results[0]);

            return results;
        }

        private void PopulateMap(List<Entry> sourceEntries)
        {
            for (uint index = 0; index < sourceEntries.Count; ++index)
            {
                SymbolInfo info;
                info.Index = index;
                info.Bits = null;
                _map[sourceEntries[(int)index].Symbol] = info;
            }
        }

        private void Validate()
        {
        }

        #region Nested type: Entry

        private struct Entry : IComparer<Entry>
        {
            public uint[]? ChildIndex;
            public uint Parent;
            public TSymbolType Symbol;
            public uint Weight;

            public bool IsSymbol
            {
                get { return ChildIndex == null; }
            }

            public bool GetChildIndex(uint index)
            {
                return ChildIndex![1] == index;
            }

            public uint GetChild(bool position)
            {
                return ChildIndex![position ? 1 : 0];
            }

            #region IComparer implementation

            public int Compare(Entry x, Entry y)
            {
                int diff = x.Weight.CompareTo(y.Weight);
                if (diff != 0)
                {
                    return diff;
                }
                return x.Symbol.CompareTo(y.Symbol);
            }

            #endregion
        }

        #endregion

        #region Nested type: SymbolInfo

        private struct SymbolInfo
        {
            public BitArray? Bits;
            public uint Index;
        }

        #endregion
    }
}