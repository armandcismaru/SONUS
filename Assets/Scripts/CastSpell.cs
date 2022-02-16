using UnityEngine;

namespace UnityWebGLMicrophone
{
    public class CastSpell : MonoBehaviour
    {
        
#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        void Update()
        {
            Microphone.Update();
        }
#endif
        void FixedUpdate()
        {
            string[] devices = Microphone.devices;

#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
#endif

            for (int index = 0; index < devices.Length; ++index)
            {
                string deviceName = devices[index];
                if (deviceName == null)
                {
                    deviceName = string.Empty;
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                if (volumes[index] >= 0.5)
                {
                    TriggerSpell();
                }
#else
                if(Input.GetKeyDown(KeyCode.T))
                {
                    TriggerSpell();
                }
#endif
            }
        }

        void TriggerSpell()
        {
            gameObject.GetComponentInParent<PlayerController>().Reload();
            Debug.Log("reloaded");
        }
    }
}
