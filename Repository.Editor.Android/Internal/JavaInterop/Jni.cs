using System;
using Android.Runtime;
using Java.Nio;
using JavaObject = Java.Lang.Object;

namespace Repository.Editor.Android.Internal.JavaInterop
{
    internal static class Jni
    {
        public static ByteBuffer NewDirectByteBuffer(IntPtr address, long capacity)
        {
            var handle = JNIEnv.NewDirectByteBuffer(address, capacity);
            return JavaObject.GetObject<ByteBuffer>(handle, JniHandleOwnership.TransferLocalRef);
        }
    }
}