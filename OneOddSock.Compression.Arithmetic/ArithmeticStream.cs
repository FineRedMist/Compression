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
using System.IO;
using OneOddSock.IO;

namespace OneOddSock.Compression.Arithmetic
{
    /// <summary>
    /// The compression mode to use for the ArithmeticStream.
    /// </summary>
    public enum CompressionMode
    {
        /// <summary>
        /// Decompress the source stream.
        /// </summary>
        Decompress,

        /// <summary>
        /// Compress the source stream.
        /// </summary>
        Compress
    }

    /// <summary>
    /// Performs arithmetic compression of the base stream.
    /// Please note--when decompressing data embedded within a stream the 
    /// read pointer may be advanced beyond just the compressed data.
    /// </summary>
    public class ArithmeticStream : Stream, IDisposable
    {
        private readonly ZeroOrderAdaptiveByteModel _adaptiveModel = new ZeroOrderAdaptiveByteModel();
        private readonly ArithmeticCoder _coder = new ArithmeticCoder();
        private readonly bool _leaveOpen;
        private readonly CompressionMode _mode;
        private readonly NewCharacterByteModel _newCharacterModel = new NewCharacterByteModel();
        private readonly BitStream _stream;
        private bool _eof;

        /// <summary>
        /// Creates a new stream using arithmetic compression on the provided <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Underlying stream to compress or decompress.</param>
        /// <param name="mode">Compression mode (compress or decompress).</param>
        /// <param name="leaveOpen">Whether to leave the underlying stream open on dispose.</param>
        public ArithmeticStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            _stream = mode == CompressionMode.Compress
                          ? new BitStreamWriter(stream)
                          : new BitStreamReader(stream) as BitStream;
            _mode = mode;
            _leaveOpen = leaveOpen;

            if (mode == CompressionMode.Decompress)
            {
                _coder.DecodeStart(_stream.ReadBoolean);
            }
        }

        /// <summary>
        /// Creates a new stream using arithmetic compression on the provided <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Underlying stream to compress or decompress.</param>
        /// <param name="mode">Compression mode (compress or decompress).</param>
        public ArithmeticStream(Stream stream, CompressionMode mode)
            : this(stream, mode, false)
        {
        }

        /// <summary>
        /// Compression mode of the stream.
        /// </summary>
        public CompressionMode Mode
        {
            get { return _mode; }
        }

        /// <summary>
        /// Whether the stream can be read (only during decompression).
        /// </summary>
        public override bool CanRead
        {
            get { return Mode == CompressionMode.Decompress; }
        }

        /// <summary>
        /// Whether the stream supports seeking (false).
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Whether the stream can be written to (only during compression).
        /// </summary>
        public override bool CanWrite
        {
            get { return Mode == CompressionMode.Compress; }
        }

        /// <summary>
        /// Current length of the stream in bytes.
        /// </summary>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// Position in the underlying stream.  This may be inaccurate due to 
        /// pending bits in the encoder/decoder.
        /// </summary>
        public override long Position
        {
            get { return _stream.Position; }
            set { throw new NotSupportedException(); }
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Flush();
            if (_stream != null && !_leaveOpen)
            {
                _stream.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Flushes any remaining bits to the stream.
        /// </summary>
        public override void Flush()
        {
            if (Mode == CompressionMode.Compress)
            {
                _coder.Encode(ZeroOrderAdaptiveByteModel.StreamTerminator, _adaptiveModel, _stream.Write);
                _coder.EncodeFinish(_stream.Write);
            }
            _stream.Flush();
        }

        /// <summary>
        /// Decompresses <paramref name="count"/> bytes into <paramref name="buffer"/>
        /// starting at <paramref name="offset"/>.
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new InvalidOperationException();
            }
            if (_eof)
            {
                return 0;
            }
            for (int i = 0; i < count; ++i)
            {
                uint adaptiveSymbol = _coder.Decode(_adaptiveModel, ReadBoolean);
                switch (adaptiveSymbol)
                {
                    case ZeroOrderAdaptiveByteModel.NewCharacter:
                        byte symbol = _coder.Decode(_newCharacterModel, ReadBoolean);
                        buffer[offset + i] = symbol;
                        _adaptiveModel.Update(symbol);
                        break;
                    case ZeroOrderAdaptiveByteModel.StreamTerminator:
                        _eof = true;
                        return i;
                    default:
                        buffer[offset + i] = (byte) adaptiveSymbol;
                        break;
                }
            }
            return count;
        }

        private bool ReadBoolean()
        {
            if (!_stream.CanSeek
                || _stream.BitPosition < _stream.BitLength)
            {
                return _stream.ReadBoolean();
            }
            return false;
        }

        /// <summary>
        /// Seeks to a new position in the file.  Not supported.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream.  Not supported.
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes <paramref name="count"/> bytes from <paramref name="buffer"/>
        /// starting at <paramref name="offset"/>.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException();
            }
            for (int i = 0; i < count; ++i)
            {
                byte b = buffer[offset + i];
                if (_newCharacterModel.Emitted(b))
                {
                    _coder.Encode(b, _adaptiveModel, _stream.Write);
                }
                else
                {
                    _coder.Encode(ZeroOrderAdaptiveByteModel.NewCharacter, _adaptiveModel, _stream.Write);
                    _coder.Encode(b, _newCharacterModel, _stream.Write);
                    _adaptiveModel.Update(b);
                }
            }
        }
    }
}