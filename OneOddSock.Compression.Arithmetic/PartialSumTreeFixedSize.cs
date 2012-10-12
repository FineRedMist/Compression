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
    /// <summary>
    /// Provides a simple implementation of the PartialSumTree algorithm using a single small fixed size array.
    /// </summary>
    public class PartialSumTreeFixedSize
    {
        private readonly uint _symbolCount;
        private readonly uint[] _weights;

        /// <summary>
        /// Initializes the partial sum tree for a given maximum symbol value--all values from zero to this
        /// value will have entries in the tree.
        /// O(n).
        /// </summary>
        /// <param name="maxSymbolValue">Maximum symbol value supported.</param>
        public PartialSumTreeFixedSize(uint maxSymbolValue)
        {
            _symbolCount = maxSymbolValue + 1;
            _weights = new uint[2*_symbolCount - 1];
        }

        /// <summary>
        /// Initializes the partial sum tree for a given maximum symbol value--all values from zero to this
        /// value will have entries in the tree.
        /// O(n + m log n) complexity (m == initialSymbolWeights.Count).
        /// </summary>
        /// <param name="maxSymbolValue">Maximum symbol value supported.</param>
        /// <param name="initialSymbolWeights">Initial set of weights to set on the tree.</param>
        public PartialSumTreeFixedSize(uint maxSymbolValue, IEnumerable<KeyValuePair<uint, uint>> initialSymbolWeights)
            : this(maxSymbolValue)
        {
            UpdateWeights(initialSymbolWeights);
        }

        /// <summary>
        /// Initializes the partial sum tree for a given maximum symbol value--all values from zero to this
        /// value will have entries in the tree.
        /// O(n + m log n) complexity (m == initialSymbolWeights.Count).
        /// </summary>
        /// <param name="maxSymbolValue">Maximum symbol value supported.</param>
        /// <param name="initialSymbolWeights">Initial set of weights to set on the tree.</param>
        public PartialSumTreeFixedSize(uint maxSymbolValue, params KeyValuePair<uint, uint>[] initialSymbolWeights)
            : this(maxSymbolValue, (IEnumerable<KeyValuePair<uint, uint>>) initialSymbolWeights)
        {
        }

        /// <summary>
        /// Total weight of elements in the tree.
        /// O(1) complexity.
        /// </summary>
        public uint TotalWeight { get; private set; }

        /// <summary>
        /// Returns the range for a given <paramref name="symbol"/>.
        /// </summary>
        /// O(log n) complexity.
        public Range this[uint symbol]
        {
            get
            {
                if (symbol >= _symbolCount)
                {
                    throw new IndexOutOfRangeException();
                }

                uint current = GetArrayIndexForSymbol(symbol);
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

        /// <summary>
        /// Navigates the tree to identify and return the symbol in the tree that corresponds to a given <paramref name="value"/>.
        /// O(log n) complexity.
        /// </summary>
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

        /// <summary>
        /// Returns the weight of the given <paramref name="symbol"/>.
        /// O(1) complexity.
        /// </summary>
        public uint GetWeight(uint symbol)
        {
            if (symbol >= _symbolCount)
            {
                throw new IndexOutOfRangeException();
            }
            return _weights[GetArrayIndexForSymbol(symbol)];
        }

        /// <summary>
        /// Resets the weights of the tree and reapplies the initial symbol weights.
        /// O(n + m log n) complexity (m == initialSymbolWeights.Count).
        /// </summary>
        /// <param name="initialSymbolWeights">Initial set of weights to set on the tree.</param>
        public void Reset(IEnumerable<KeyValuePair<uint, uint>> initialSymbolWeights)
        {
            for (int i = 0; i < _weights.Length; ++i)
            {
                _weights[i] = 0;
            }
            UpdateWeights(initialSymbolWeights);
        }

        /// <summary>
        /// Resets the weights of the tree and reapplies the initial symbol weights.
        /// O(n + m log n) complexity (m == initialSymbolWeights.Count).
        /// </summary>
        /// <param name="initialSymbolWeights">Initial set of weights to set on the tree.</param>
        public void Reset(params KeyValuePair<uint, uint>[] initialSymbolWeights)
        {
            Reset((IEnumerable<KeyValuePair<uint, uint>>) initialSymbolWeights);
        }

        /// <summary>
        /// Updates multiple weights at once.
        /// O(m log n) complexity (m == symbolWeights.Count).
        /// </summary>
        /// <param name="symbolWeights">Initial set of weights to set on the tree.</param>
        public void UpdateWeights(IEnumerable<KeyValuePair<uint, uint>> symbolWeights)
        {
            foreach (var symbolWeight in symbolWeights)
            {
                int weight;
                checked
                {
                    weight = (int) symbolWeight.Value;
                }
                UpdateWeight(symbolWeight.Key, weight);
            }
        }

        /// <summary>
        /// Updates multiple weights at once.
        /// O(m log n) complexity (m == symbolWeights.Count).
        /// </summary>
        /// <param name="symbolWeights">Initial set of weights to set on the tree.</param>
        public void UpdateWeights(params KeyValuePair<uint, uint>[] symbolWeights)
        {
            UpdateWeights((IEnumerable<KeyValuePair<uint, uint>>) symbolWeights);
        }

        /// <summary>
        /// Updates the weight of the <paramref name="symbol"/> in the tree by <paramref name="delta"/>.
        /// O(log n) complexity.
        /// </summary>
        /// <returns>The previous weight of the <paramref name="symbol"/>.</returns>
        public uint UpdateWeight(uint symbol, int delta)
        {
            if (symbol >= _symbolCount)
            {
                throw new IndexOutOfRangeException();
            }

            uint current = GetArrayIndexForSymbol(symbol);
            uint oldWeight = _weights[current];

            uint child = 2*current + 1; // Compute the left child index

            do
            {
                // Left children increase the current node weight
                if (child == 2*current + 1)
                {
                    checked
                    {
                        _weights[current] = (uint) (_weights[current] + delta);
                    }
                }
                child = current;
                current = (current - 1) >> 1;
            } while (child != 0);

            TotalWeight = (uint) (TotalWeight + delta);

            return oldWeight;
        }

        /// <summary>
        /// Converts the <paramref name="symbol"/> into the corresponding array index in the _weights array.
        /// </summary>
        private uint GetArrayIndexForSymbol(uint symbol)
        {
            return symbol + _symbolCount - 1;
        }

        /// <summary>
        /// Converts the _weights array <paramref name="index"/> into the corresponding symbol.
        /// </summary>
        private uint GetSymbolForArrayIndex(uint index)
        {
            return index - _symbolCount + 1;
        }
    }
}