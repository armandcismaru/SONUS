using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;


    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.spatialBlend = s.spatialBlend;

            // 3D Sound settings
            s.source.minDistance = s.minDistance;
            s.source.maxDistance = s.maxDistance;
            s.source.dopplerLevel = s.dopplerLevel;
            s.source.spread = s.spread;

            s.source.rolloffMode = s.rollofMode;
        }
    }

    public Sound GetSound(string sound)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == sound) return s;
        }
        Debug.Log("Sound" + sound + "not found!");
        return sounds[0];
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound" + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound" + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.isPlaying;
    }

}
