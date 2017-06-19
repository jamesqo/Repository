using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Android.Runtime;
using Java.Nio;
using Repository.Common;
using Repository.JavaInterop;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.EditorServices.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    internal partial class FragmentedByteBuffer : IDisposable
    {
        private const int InitialCapacity = 256;

        // Do not mark this field readonly, ArrayBuilder is a mutable struct.
        private ArrayBuilder<ByteBuffer> _fragments;
        private unsafe byte* _current;
        private int _currentCapacity;
        private int _index;

        public FragmentedByteBuffer()
        {
            Allocate(InitialCapacity);
        }

        private int ByteCount
        {
            get
            {
                if (FragmentCount == 1)
                {
                    return _index;
                }

                Debug.Assert(_fragments.SkipLast(1).Sum(f => f.Capacity()) == _currentCapacity);
                return _currentCapacity + _index;
            }
        }

        // Note: This is for debug use only.
        private int Capacity => GetFragments().Sum(f => f.Capacity());

        private string DebuggerDisplay => $"{nameof(ByteCount)} = {ByteCount}, {nameof(Capacity)} = {Capacity}, {nameof(FragmentCount)} = {FragmentCount}";

        private int FragmentCount => _fragments.Count;

        public unsafe void Add(long value)
        {
            Debug.Assert(IsAligned(sizeof(long)));

            if (_index == _currentCapacity)
            {
                Allocate();
            }

            // It's tempting to use pointer arithmetic here, but we must write the data out in the correct endianness.
            // TODO: What if other devices are little endian?
            _current[_index] = (byte)(value >> 56);
            _current[_index + 1] = (byte)(value >> 48);
            _current[_index + 2] = (byte)(value >> 40);
            _current[_index + 3] = (byte)(value >> 32);
            _current[_index + 4] = (byte)(value >> 24);
            _current[_index + 5] = (byte)(value >> 16);
            _current[_index + 6] = (byte)(value >> 8);
            _current[_index + 7] = (byte)value;
            _index += sizeof(long);
        }

        // TODO: Override Finalize(), have Dispose(bool)?
        // Also prevent double frees and null out everything?
        public void Dispose()
        {
            for (int i = 0; i < _fragments.Count; i++)
            {
                Free(_fragments[i]);
            }
        }

        public ByteBuffer[] GetFragments() => _fragments.ToArray();

        public FragmentedReadStream ToReadStream() => new FragmentedReadStream(GetFragments(), ByteCount);

        private void Allocate()
        {
            Debug.Assert(_index == _currentCapacity);

            int nextCapacity = _fragments.Count == 1
                ? InitialCapacity
                : _currentCapacity * 2;
            Allocate(nextCapacity);
        }

        private unsafe void Allocate(int capacity)
        {
            _index = 0;
            _currentCapacity = capacity;

            var address = Marshal.AllocHGlobal(capacity);
            _current = (byte*)address.ToPointer();

            var handle = JNIEnv.NewDirectByteBuffer(address, capacity);
            _fragments.Add(JavaObject.GetObject<ByteBuffer>(handle, JniHandleOwnership.TransferLocalRef));
        }

        private static void Free(ByteBuffer fragment)
        {
            // TODO: Buffers should be freed ASAP. What if we open a 1GB file and we OOM before we get to free?
            var address = fragment.GetDirectBufferAddress();
            Marshal.FreeHGlobal(address);
        }

        private bool IsAligned(int byteCount)
        {
            Debug.Assert(_currentCapacity % byteCount == 0);
            return _index % byteCount == 0;
        }
    }
}