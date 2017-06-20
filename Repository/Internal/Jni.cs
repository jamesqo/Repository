﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
using Java.Nio;
using JavaObject = Java.Lang.Object;

namespace Repository.Internal
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