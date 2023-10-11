using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "ScriptableObjects/AudioCollection")]
public class AudioObject : ScriptableObject
{
    public List<Channel> Channels = new List<Channel>() { new("Music"), new("SFX") };
    public bool EnumsAssigned = true;
}

[System.Serializable]
public class Channel
{
    public string Name;
    public List<Sound> Sounds;
    public ChannelEnum Enum;

    public Channel(string name)
    {
        Name = name;
        Sounds = new List<Sound>();
    }
}

[System.Serializable]
public class Sound
{
    public string Name;
    public List<Clip> Clips;
    public bool HasPitchVariation;
    public float PitchVariation;
    public SoundEnum Enum;

    private int _lastPlayedIndex = -1;
    public Clip GetClip()
    {
        if (Clips.Count == 0) return null;
        if (Clips.Count == 1) return Clips[0];

        var randomIndex = UnityEngine.Random.Range(0, Clips.Count);
        while (randomIndex == _lastPlayedIndex)
            randomIndex = UnityEngine.Random.Range(0, Clips.Count);

        _lastPlayedIndex = randomIndex;
        return Clips[randomIndex];
    }

    public Sound(string name)
    {
        Name = name;
        Clips = new();
        PitchVariation = 0f;
    }

    [System.Serializable]
    public class Clip
    {
        public AudioClip AudioClip;
        public float Volume;

        public Clip(AudioClip clip, float volume)
        {
            AudioClip = clip;
            Volume = volume;
        }
    }
}
