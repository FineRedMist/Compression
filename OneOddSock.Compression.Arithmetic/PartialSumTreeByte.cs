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

using System;
using System.Collections.Generic;

namespace OneOddSock.Compression.Arithmetic
{
    internal class PartialSumTreeByte
    {
        private readonly uint _symbolCount;
        private readonly uint[] _weights;

        public PartialSumTreeByte(uint maxSymbolValue)
        {
            _symbolCount = maxSymbolValue + 1;
            _weights = new uint[2*_symbolCount - 1];
        }

        public PartialSumTreeByte(uint maxSymbolValue, IEnumerable<KeyValuePair<uint, uint>> initialSymbolWeights)
            : this(maxSymbolValue)
        {
            UpdateWeights(initialSymbolWeights);
        }

        public PartialSumTreeByte(uint maxSymbolValue, params KeyValuePair<uint, uint>[] initialSymbolWeights)
            : this(maxSymbolValue, (IEnumerable<KeyValuePair<uint, uint>>) initialSymbolWeights)
        {
        }

        public uint Total { get; private set; }

        public Range this[uint symbolIndex]
        {
            get
            {
                if (symbolIndex >= _symbolCount)
                {
                    throw new IndexOutOfRangeException();
                }

                uint current = GetArrayIndexForSymbol(symbolIndex);
                uint currentWeight = _weights[current];

                uint child = 0; // We aren't including the symbol node's weight in the sum.
                uint low = 0;

                do
                {
                    // Right children include the current node in the sum
                    if (child == 2*current + 2)
                    {
                        low += _weights[current];
                    }
                    child = current;
                    current = (current - 1) >> 1;
                } while (child != 0);

                return new Range {Low = low, High = low + currentWeight};
            }
        }

        public uint GetSymbol(uint value)
        {
            uint current = 0;

            // Until we hit a leaf node
            while (current < _symbolCount - 1)
            {
                uint leftChild = 2*current + 1;
                uint rightChild = leftChild + 1;
                if (value < _weights[current])
                {
                    current = leftChild;
                }
                else
                {
                    value -= _weights[current];
                    current = rightChild;
                }
            }
            if (_weights[current] <= value)
            {
                throw new BadImageFormatException();
            }

            return GetSymbolForArrayIndex(current);
        }

        public uint GetWeight(uint symbolIndex)
        {
            if (symbolIndex >= _symbolCount)
            {
                throw new IndexOutOfRangeException();
            }
            return _weights[GetArrayIndexForSymbol(symbolIndex)];
        }

        public void Reset(IEnumerable<KeyValuePair<uint, uint>> initialSymbolWeights)
        {
            for (int i = 0; i < _weights.Length; ++i)
            {
                _weights[i] = 0;
            }
            UpdateWeights(initialSymbolWeights);
        }

        public void Reset(params KeyValuePair<uint, uint>[] initialSymbolWeights)
        {
            Reset((IEnumerable<KeyValuePair<uint, uint>>) initialSymbolWeights);
        }

        public void UpdateWeights(IEnumerable<KeyValuePair<uint, uint>> initialSymbolWeights)
        {
            foreach (var symbolWeight in initialSymbolWeights)
            {
                int weight;
                checked
                {
                    weight = (int) symbolWeight.Value;
                }
                UpdateWeight(symbolWeight.Key, weight);
            }
        }

        public void UpdateWeights(params KeyValuePair<uint, uint>[] initialSymbolWeights)
        {
            UpdateWeights((IEnumerable<KeyValuePair<uint, uint>>) initialSymbolWeights);
        }

        public uint UpdateWeight(uint symbolIndex, int change)
        {
            if (symbolIndex >= _symbolCount)
            {
                throw new IndexOutOfRangeException();
            }

            uint current = GetArrayIndexForSymbol(symbolIndex);
            uint oldWeight = _weights[current];

            uint child = 2*current + 1; // Compute the left child index

            do
            {
                // Left children increase the current node weight
                if (child == 2*current + 1)
                {
                    checked
                    {
                        _weights[current] = (uint) (_weights[current] + change);
                    }
                }
                child = current;
                current = (current - 1) >> 1;
            } while (child != 0);

            Total = (uint) (Total + change);

            return oldWeight;
        }

        private uint GetArrayIndexForSymbol(uint symbol)
        {
            return symbol + _symbolCount - 1;
        }

        private uint GetSymbolForArrayIndex(uint index)
        {
            return index - _symbolCount + 1;
        }
    }
}