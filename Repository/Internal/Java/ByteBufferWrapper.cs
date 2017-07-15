using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Java.Nio;
using Repository.Common;

namespace Repository.Internal.Java
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal class ByteBufferWrapper : IDisposable
    {
        private NativeBuffer _buffer;
        private ByteBuffer _javaBuffer;

        public ByteBufferWrapper(int capacity)
        {
            Debug.Assert(capacity > 0);

            _buffer = new NativeBuffer(capacity);
            _javaBuffer = Jni.NewDirectByteBuffer(_buffer.Address, capacity);
        }

        public int ByteCount => _buffer.ByteCount;

        public bool IsFull => _buffer.IsFull;

        private string DebuggerDisplay => _buffer.DebuggerDisplay;

        public void Add(long value) => _buffer.Add(value);

        public void Clear() => _buffer.Clear();

        public void Dispose()
        {
            _buffer?.Dispose();
            _buffer = null;

            _javaBuffer?.Dispose();
            _javaBuffer = null;
        }

        public ByteBuffer Unwrap() => _javaBuffer;
    }
}