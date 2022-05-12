using UnityEngine;

// Custom sound class that enables the Audio Manager to index and customize clips
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;

    [Range(.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;

    [Range(0f, 1f)]
    public float spatialBlend;

    [Range(0f, 5f)]
    public float dopplerLevel;

    [Range(0, 360)]
    public int spread;

    public float minDistance;
    public float maxDistance;

    public AudioRolloffMode rollofMode = AudioRolloffMode.Logarithmic;
}
