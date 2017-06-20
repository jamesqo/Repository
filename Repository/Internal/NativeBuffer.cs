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
        private readonly int _capacity;

        private unsafe byte* _address;
        private int _count;

        public unsafe NativeBuffer(int capacity)
        {
            _address = (byte*)Marshal.AllocHGlobal(capacity).ToPointer();
            _capacity = capacity;
        }

        public unsafe IntPtr Address => new IntPtr(_address);

        public int ByteCount => _count;

        public int Capacity => _capacity;

        public bool IsFull => _count == _capacity;

        public unsafe bool IsInvalid => _address == null;

        internal string DebuggerDisplay => $"{nameof(ByteCount)} = {ByteCount}, {nameof(Capacity)} = {Capacity}";

        public unsafe void Add(long value)
        {
            Verify.State(HasRoom(sizeof(long)));

            // It's tempting to use pointer arithmetic here, but we must write the data out in the correct endianness.
            // TODO: What if other devices are little endian?
            _address[_count] = (byte)(value >> 56);
            _address[_count + 1] = (byte)(value >> 48);
            _address[_count + 2] = (byte)(value >> 40);
            _address[_count + 3] = (byte)(value >> 32);
            _address[_count + 4] = (byte)(value >> 24);
            _address[_count + 5] = (byte)(value >> 16);
            _address[_count + 6] = (byte)(value >> 8);
            _address[_count + 7] = (byte)value;
            _count += sizeof(long);
        }

        public void Clear() => _count = 0;

        public unsafe void Dispose()
        {
            Verify.State(!IsInvalid);

            Marshal.FreeHGlobal(Address);
            _address = null;
        }

        public byte[] ToByteArray()
        {
            var array = new byte[_count];
            Marshal.Copy(Address, array, 0, _count);
            return array;
        }

        private bool HasRoom(int byteCount) => _count + byteCount <= _capacity;
    }
}