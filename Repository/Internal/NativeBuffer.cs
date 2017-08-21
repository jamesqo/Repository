using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Repository.Common;

namespace Repository.Internal
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    internal partial class NativeBuffer : IDisposable
    {
        private unsafe byte* _address;
        private int _capacity;
        private int _byteCount;

        public unsafe NativeBuffer(int capacity)
        {
            _address = (byte*)Marshal.AllocHGlobal(capacity).ToPointer();
            _capacity = capacity;
        }

        public unsafe IntPtr Address => new IntPtr(_address);

        public int ByteCount => _byteCount;

        public int Capacity => _capacity;

        public bool IsFull => _byteCount == _capacity;

        public unsafe bool IsValid => _address != null;

        internal string DebuggerDisplay => $"{nameof(ByteCount)} = {ByteCount}, {nameof(Capacity)} = {Capacity}";

        public void Clear() => _byteCount = 0;

        public unsafe void Dispose()
        {
            Verify.ValidState(IsValid);

            Marshal.FreeHGlobal(Address);
            _address = null;
            _capacity = 0;
            _byteCount = 0;
        }

        public byte[] ToByteArray()
        {
            Verify.ValidState(IsValid);

            var array = new byte[_byteCount];
            Marshal.Copy(Address, array, 0, _byteCount);
            return array;
        }

        public unsafe void WriteInt32(int value)
        {
            Verify.ValidState(IsValid && HasRoom(4));

            _address[_byteCount] = (byte)(value >> 24);
            _address[_byteCount + 1] = (byte)(value >> 16);
            _address[_byteCount + 2] = (byte)(value >> 8);
            _address[_byteCount + 3] = (byte)value;
            _byteCount += 4;
        }

        private bool HasRoom(int byteCount) => _byteCount + byteCount <= _capacity;
    }
}