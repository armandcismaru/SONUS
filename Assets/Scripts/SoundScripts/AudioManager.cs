using UnityEngine;
using System;

/* Audio Manager class used to attack Audio Sources at runtime;
   lives attached to the whatever game component needs to brodcast sounds */
public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

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

    // Returns sound clip by name
    public Sound GetSound(string sound)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == sound) return s;
        }
        Debug.Log("Sound" + sound + "not found!");
        return sounds[0];
    }

    // Stops sound by clip name
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

    // Plays sound by clip name
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

    // Checks whether certain sound is already playing 
    public bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.isPlaying;
    }

}
