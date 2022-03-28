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

        [DllImport("__Internal")]
        public static extern void setPosition(int index, float x, float y, float z);

        [DllImport("__Internal")]
        public static extern void setMyOrientation(float x, float y, float z);

        [DllImport("__Internal")]
        public static extern void setPlayerVolume(int index, float value);
    }
}
#endif