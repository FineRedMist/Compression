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

namespace OneOddSock.IO
{
    /// <summary>
    /// Support for identifying the Endian of data based on the marker.
    /// </summary>
    public static class EndianMarker
    {
        /// <summary>
        /// Processes the provided <paramref name="marker"/> in the stream held by
        /// <paramref name="reader"/> to determine the Endian setting of the stream.
        /// 
        /// Advances the position in the stream by four bytes and returns a new 
        /// binary reader to read the stream with the correct Endian setting.
        /// </summary>
        /// <returns>
        /// New BinaryReader for the stream.
        /// </returns>
        public static BinaryReader ProcessEndianMarker(this BinaryReader reader, uint marker)
        {
            uint streamMarker = reader.ReadUInt32();
            if (streamMarker == marker)
            {
                return reader;
            }
            uint swappedStreamMarker = (streamMarker << 24)
                                       | ((streamMarker & 0x0000FF00) << 8)
                                       | ((streamMarker & 0x00FF0000) >> 8)
                                       | (streamMarker >> 24);
            if (swappedStreamMarker == marker)
            {
                var result = new EndianReader(reader);
                var source = reader as EndianReader;
                result.Endian = source != null ? source.Endian.Switch : Endian.NonNative;
                return result;
            }
            throw new IOException(string.Format("Endian stream marker mismatch: received 0x{0}, expected 0x{1}.",
                                                streamMarker.ToString("x8"), marker.ToString("x8")));
        }

        /// <summary>
        /// Creates a BinaryWriter to write to the stream managed by <paramref name="writer"/>
        /// using the provided Endian setting.  <paramref name="marker"/> is written to the 
        /// stream to mark the Endian setting used.
        /// </summary>
        public static BinaryWriter WriteEndianMarker(this BinaryWriter writer, Endian endian, uint marker)
        {
            BinaryWriter result = writer;
            if (endian != Endian.Native)
            {
                var endianWriter = new EndianWriter(writer) {Endian = endian};
                result = endianWriter;
            }
            writer.Write(marker);
            return result;
        }
    }
}