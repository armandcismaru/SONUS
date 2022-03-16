#if UNITY_WEBGL && !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine
{
    public class VoiceChat
    {
        [DllImport("__Internal")]
        public static extern void initializeVoiceChat();

        [DllImport("__Internal")]
        public static extern string getIds();

        [DllImport("__Internal")]
        public static extern void makeCall(int index, string id);
    }
}
#endif