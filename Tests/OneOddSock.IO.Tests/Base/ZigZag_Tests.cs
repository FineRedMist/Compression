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
using OneOddSock.IO.Converters;

namespace BitStreamTests
{
    [TestClass]
    public class ZigZag_Tests
    {
        [TestMethod]
        public void Int16()
        {
            var values = new[]
                             {
                                 new Cases_Int16(0, 0),
                                 new Cases_Int16(-1, 1),
                                 new Cases_Int16(1, 2),
                                 new Cases_Int16(short.MaxValue, ushort.MaxValue - 1),
                                 new Cases_Int16(short.MinValue, ushort.MaxValue),
                             };
            foreach (Cases_Int16 pair in values)
            {
                Assert.AreEqual(pair.expected, pair.value.ZigZag());
                Assert.AreEqual(pair.value, pair.value.ZigZag().ZigZag());
            }
        }

        [TestMethod]
        public void Int32()
        {
            var values = new[]
                             {
                                 new Cases_Int32(0, 0),
                                 new Cases_Int32(-1, 1),
                                 new Cases_Int32(1, 2),
                                 new Cases_Int32(short.MaxValue, ushort.MaxValue - 1),
                                 new Cases_Int32(short.MinValue, ushort.MaxValue),
                                 new Cases_Int32(int.MaxValue, uint.MaxValue - 1),
                                 new Cases_Int32(int.MinValue, uint.MaxValue),
                             };
            foreach (Cases_Int32 pair in values)
            {
                Assert.AreEqual(pair.expected, pair.value.ZigZag());
                Assert.AreEqual(pair.value, pair.value.ZigZag().ZigZag());
            }
        }

        [TestMethod]
        public void Int64()
        {
            var values = new[]
                             {
                                 new Cases_Int64(0, 0),
                                 new Cases_Int64(-1, 1),
                                 new Cases_Int64(1, 2),
                                 new Cases_Int64(short.MaxValue, ushort.MaxValue - 1),
                                 new Cases_Int64(short.MinValue, ushort.MaxValue),
                                 new Cases_Int64(int.MaxValue, uint.MaxValue - 1),
                                 new Cases_Int64(int.MinValue, uint.MaxValue),
                                 new Cases_Int64(long.MaxValue, ulong.MaxValue - 1),
                                 new Cases_Int64(long.MinValue, ulong.MaxValue),
                             };
            foreach (Cases_Int64 pair in values)
            {
                Assert.AreEqual(pair.expected, pair.value.ZigZag());
                Assert.AreEqual(pair.value, pair.value.ZigZag().ZigZag());
            }
        }

        #region Nested type: Cases_Int16

        private class Cases_Int16
        {
            public readonly ushort expected;
            public readonly short value;

            public Cases_Int16(short v, ushort e)
            {
                value = v;
                expected = e;
            }
        };

        #endregion

        #region Nested type: Cases_Int32

        private class Cases_Int32
        {
            public readonly uint expected;
            public readonly int value;

            public Cases_Int32(int v, uint e)
            {
                value = v;
                expected = e;
            }
        };

        #endregion

        #region Nested type: Cases_Int64

        private class Cases_Int64
        {
            public readonly ulong expected;
            public readonly long value;

            public Cases_Int64(long v, ulong e)
            {
                value = v;
                expected = e;
            }
        };

        #endregion
    }
}