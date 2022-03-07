#if UNITY_WEBGL && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    public class WebRTC
    {
        [DllImport("__Internal")]
        public static extern void initWebRTC();

        [DllImport("__Internal")]
        public static extern string create();

        [DllImport("__Internal")]
        public static extern void answer(string name);

        [DllImport("__Internal")]
        public static extern void answered();
    }
}
#endif