using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Runtime;
// TODO: Use this defn. in other places, too
using JavaObject = Java.Lang.Object;

namespace Repository.Internal.EditorServices.Highlighting
{
    [Register("android/text/style/ForegroundColorSpan", DoNotGenerateAcw = true)]
    internal class FastForegroundColorSpan : JavaObject
    {
        private static readonly IntPtr class_ref = JNIEnv.FindClass("android/text/style/ForegroundColorSpan");

        private static IntPtr id_ctor_I;

        [Register(".ctor", "(I)V", "")]
        public unsafe FastForegroundColorSpan(int color)
            : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
        {
            if (Handle != IntPtr.Zero)
            {
                return;
            }

            if (id_ctor_I == IntPtr.Zero)
            {
                id_ctor_I = JNIEnv.GetMethodID(class_ref, "<init>", "(I)V");
            }

            var args = stackalloc JValue[1];
            args[0] = new JValue(color);
            var handle = JNIEnv.NewObject(class_ref, id_ctor_I, args);
            SetHandle(handle, JniHandleOwnership.TransferLocalRef);
        }
    }
}