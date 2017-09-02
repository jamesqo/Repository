using System;
using System.Diagnostics;
using Java.Nio;
using Repository.Common;
using Repository.Common.Validation;
using Repository.Editor.Android.Internal.IO;

namespace Repository.Editor.Android.Internal.JavaInterop
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    internal class NativeByteBuffer : IDisposable
    {
        private NativeBuffer _buffer;
        private ByteBuffer _javaBuffer;

        public NativeByteBuffer(int capacity)
        {
            Verify.InRange(capacity > 0, nameof(capacity));

            _buffer = new NativeBuffer(capacity);
            _javaBuffer = Jni.NewDirectByteBuffer(_buffer.Address, capacity);
        }

        public int ByteCount => _buffer.ByteCount;

        public bool IsFull => _buffer.IsFull;

        private string DebuggerDisplay => _buffer.DebuggerDisplay;

        public void Clear() => _buffer.Clear();

        public void Dispose()
        {
            _buffer?.Dispose();
            _buffer = null;

            _javaBuffer?.Dispose();
            _javaBuffer = null;
        }

        public ByteBuffer Unwrap() => _javaBuffer;

        public void WriteInt32(int value) => _buffer.WriteInt32(value);
    }
}