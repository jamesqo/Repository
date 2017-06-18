using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Android.Runtime;
using Java.Nio;
using Repository.Common;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.EditorServices.Highlighting
{
    [DebuggerDisplay(DebuggerStrings.DisplayFormat)]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    internal partial class FragmentedByteBuffer : IDisposable
    {
        private const int InitialCapacity = 256;

        private readonly ArrayBuilder<ByteBuffer> _previous;

        private ByteBuffer _current;
        private int _currentCapacity;
        private int _index;

        public FragmentedByteBuffer()
        {
            Allocate(InitialCapacity);
        }

        private int ByteCount => _previous.Sum(f => f.Capacity()) + _index;

        private int Capacity => GetFragments().Sum(f => f.Capacity());

        private string DebuggerDisplay => $"{nameof(ByteCount)} = {ByteCount}, {nameof(Capacity)} = {Capacity}";

        public void Add(long value)
        {
            Debug.Assert(IsAligned(sizeof(long)));

            if (_index == _currentCapacity)
            {
                Allocate();
            }

            _current.PutLong(_index, value);
            _index += sizeof(long);
        }

        // TODO: Override Finalize(), have Dispose(bool)?
        // Also prevent double frees and null out everything?
        public void Dispose()
        {
            for (int i = 0; i < _previous.Count; i++)
            {
                Free(_previous[i]);
            }

            Free(_current);
        }

        public ByteBuffer[] GetFragments()
        {
            var fragments = new ByteBuffer[_previous.Count + 1];
            _previous.CopyTo(fragments, 0);
            fragments[_previous.Count] = _current;
            return fragments;
        }

        private void Allocate()
        {
            Debug.Assert(_index == _currentCapacity);
            int nextCapacity = _currentCapacity * 2;
            Allocate(nextCapacity);
        }

        private void Allocate(int capacity)
        {
            _previous.Add(_current);
            _currentCapacity = capacity;
            _index = 0;

            // TODO: Refactor into static method?
            IntPtr handle = Marshal.AllocHGlobal(capacity);
            var objectHandle = JNIEnv.NewDirectByteBuffer(handle, capacity);
            // TODO: Is this necessary? Store the raw IntPtrs directly?
            _current = JavaObject.GetObject<ByteBuffer>(objectHandle, JniHandleOwnership.TransferLocalRef);
        }

        private static void Free(ByteBuffer fragment)
        {
            IntPtr handle = fragment.GetDirectBufferAddress();
            Marshal.FreeHGlobal(handle);
        }

        private bool IsAligned(int byteCount)
        {
            Debug.Assert(_currentCapacity % byteCount == 0);
            return _index % byteCount == 0;
        }
    }
}