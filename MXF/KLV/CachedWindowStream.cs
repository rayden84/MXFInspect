using System;
using System.Buffers;
using System.IO;

namespace Myriadbits.MXF.KLV
{
    /// <summary>
    /// Buffered stream that keeps a sliding, alignment-aligned window into an underlying seekable stream.
    /// Reduces the number of underlying Read syscalls by reading large windows (multiples of alignment).
    /// </summary>
    public sealed class CachedWindowStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly int _alignment;
        private readonly int _windowSize;
        private readonly byte[] _buffer;
        private readonly bool _leaveOpen;

        private long _windowStart;   // absolute offset of buffer[0]
        private int _bufferLength;   // number of valid bytes currently in buffer
        private long _position;      // logical position within the stream
        private bool _disposed;

        public CachedWindowStream(Stream baseStream, int windowSize = 128 * 1024, int alignment = 512, bool leaveOpen = false)
        {
            if (baseStream == null) throw new ArgumentNullException(nameof(baseStream));
            if (!baseStream.CanRead) throw new ArgumentException("Base stream must be readable.", nameof(baseStream));
            if (!baseStream.CanSeek) throw new ArgumentException("Base stream must be seekable.", nameof(baseStream));
            if (alignment <= 0) throw new ArgumentOutOfRangeException(nameof(alignment));
            if (windowSize < alignment) throw new ArgumentException("windowSize must be >= alignment", nameof(windowSize));
            if (windowSize % alignment != 0) throw new ArgumentException("windowSize must be a multiple of alignment", nameof(windowSize));

            _baseStream = baseStream;
            _alignment = alignment;
            _windowSize = windowSize;
            _buffer = ArrayPool<byte>.Shared.Rent(windowSize);
            _leaveOpen = leaveOpen;

            _windowStart = baseStream.Position;
            _position = baseStream.Position;
            _bufferLength = 0; // not filled yet
        }

        private void EnsureWindowContains(long absoluteOffset)
        {
            if (absoluteOffset >= _windowStart && absoluteOffset < _windowStart + _bufferLength)
                return;

            long alignedStart = (absoluteOffset / _alignment) * _alignment;
            if (alignedStart < 0) alignedStart = 0;

            // Seek and fill buffer
            _baseStream.Seek(alignedStart, SeekOrigin.Begin);
            int toRead = _windowSize;
            int read = 0;
            while (read < toRead)
            {
                int r = _baseStream.Read(_buffer, read, toRead - read);
                if (r == 0) break;
                read += r;
            }

            _windowStart = alignedStart;
            _bufferLength = read;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CachedWindowStream));
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0 || count < 0 || offset + count > buffer.Length) throw new ArgumentOutOfRangeException();

            int totalRead = 0;
            while (count > 0)
            {
                if (_position >= Length) break;

                EnsureWindowContains(_position);

                long relative = _position - _windowStart;
                if (relative < 0 || relative >= _bufferLength)
                {
                    // nothing available in current window (EOF or small leftover)
                    break;
                }

                int available = Math.Min(_bufferLength - (int)relative, count);
                Buffer.BlockCopy(_buffer, (int)relative, buffer, offset, available);

                offset += available;
                count -= available;
                totalRead += available;
                _position += available;
            }

            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(CachedWindowStream));
            long newPos;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPos = offset;
                    break;
                case SeekOrigin.Current:
                    newPos = _position + offset;
                    break;
                case SeekOrigin.End:
                    newPos = Length + offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            if (newPos < 0) throw new IOException("Attempt to seek before beginning of stream.");
            _position = newPos;
            return _position;
        }

        public override void Flush() => _baseStream.Flush();

        public override bool CanRead => !_disposed && _baseStream.CanRead;
        public override bool CanSeek => !_disposed && _baseStream.CanSeek;
        public override bool CanWrite => false;

        public override long Length => _baseStream.Length;

        public override long Position
        {
            get => _position;
            set => Seek(value, SeekOrigin.Begin);
        }

        // Not supported write path
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ArrayPool<byte>.Shared.Return(_buffer);
                    if (!_leaveOpen)
                    {
                        _baseStream.Dispose();
                    }
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
