using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OneOddSock.IO;
using System.Linq;

namespace OneOddSock.Compression.Arithmetic.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] data = Encoding.UTF8.GetBytes(TestResources.RFC5_Text);
            int curSymbol = 0;
            MemoryStream stream = new MemoryStream();
            BitStreamWriter writer = new BitStreamWriter(stream);

            ModelOrder0C encoder = new ModelOrder0C();

            encoder.Encode(() => { return data[curSymbol++]; }, writer.Write, (uint)data.Length);
            writer.Flush();
            stream.Position = 0;

            BitStreamReader reader = new BitStreamReader(stream);
            LinkedList<byte> decoded = new LinkedList<byte>();

            ModelOrder0C decoder = new ModelOrder0C();

            decoder.Decode(() => { return reader.BitLength - reader.BitPosition > 0 && reader.ReadBoolean(); },
                           value => decoded.AddLast(value));

            byte [] decodedBytes = decoded.ToArray();

            CollectionAssert.AreEqual(data, decodedBytes);
        }
    }
}
