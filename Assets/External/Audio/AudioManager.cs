using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioObject _audioObject;

    public static AudioManager Instance { get; private set; }

    public float MasterVolume { get; private set; } = 1;
    public Dictionary<ChannelEnum, float> ChannelVolumeDictionary { get; private set; } = new();

    private Dictionary<SoundEnum, Sound> _soundEnumDictionary = new();
    private Dictionary<ChannelEnum, Channel> _channelEnumDictionary = new();
    private Dictionary<Sound, ChannelEnum> _channelDictionary = new();
    private Dictionary<AudioSource, AudioJob> _jobDictionary = new();

    private class AudioJob
    {
        public Sound Sound;
        public Sound.Clip Clip;
        public Coroutine Routine;
    }

    private enum AudioAction
    {
        Start,
        Stop,
        Restart
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        while (gameObject.GetComponent<AudioSource>())
            DestroyImmediate(gameObject.GetComponent<AudioSource>());

        for (int i = 0; i < 10; i++)
        {
            _jobDictionary.Add(gameObject.AddComponent<AudioSource>(), null);
        }

        foreach (var channel in _audioObject.Channels)
        {
            ChannelVolumeDictionary.Add(channel.Enum, 1);
            _channelEnumDictionary.Add(channel.Enum, channel);
            foreach (var sound in channel.Sounds)
            {
                _soundEnumDictionary.Add(sound.Enum, sound);
                _channelDictionary.Add(sound, channel.Enum);
            }
        }
    }

    public void SetChannelVolume(ChannelEnum channelEnum, float volume)
    {
        ChannelVolumeDictionary[channelEnum] = volume;
        UpdateVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        foreach (var job in _jobDictionary)
        {
            if (job.Value == null) continue;
            job.Key.volume = job.Value.Clip.Volume * GetExponentialVolume(MasterVolume) * GetExponentialVolume(ChannelVolumeDictionary[_channelDictionary[job.Value.Sound]]);
        }
    }

    public void Play(SoundEnum soundEnum, float fadeTime = 0, bool loops = false)
    {
        StartAudioAction(AudioAction.Start, _soundEnumDictionary[soundEnum], fadeTime, loops);
    }

    public void Stop(SoundEnum soundEnum, float fadeTime = 0)
    {
        StartAudioAction(AudioAction.Stop, _soundEnumDictionary[soundEnum], fadeTime, false);
    }

    public void Restart(SoundEnum soundEnum, float fadeTime = 0, bool loops = false)
    {
        StartAudioAction(AudioAction.Restart, _soundEnumDictionary[soundEnum], fadeTime, loops);
    }

    public void Stop(ChannelEnum channelEnum, float fadeTime = 0)
    {
        var channel = _channelEnumDictionary[channelEnum];

        foreach (var entry in _channelDictionary)
        {
            if (entry.Value == channel.Enum)
            {
                StartAudioAction(AudioAction.Stop, entry.Key, fadeTime, false);
            }
        }
    }

    public void Stop(params ChannelEnum[] channelEnums)
    {
        foreach (var channelEnum in channelEnums)
        {
            Stop(channelEnum);
        }
    }

    public void Stop(params SoundEnum[] soundEnums)
    {
        foreach (var soundEnum in soundEnums)
        {
            Stop(soundEnum);
        }
    }

    public bool IsPlaying(SoundEnum sound)
    {
        return FindSoundSources(_soundEnumDictionary[sound]).Count != 0;
    }

    private void StartAudioAction(AudioAction action, Sound sound, float fadeTime, bool loops)
    {
        var soundClip = sound.GetClip();

        if (action is AudioAction.Restart)
        {
            StartAudioAction(AudioAction.Stop, sound, fadeTime, loops);
            StartAudioAction(AudioAction.Start, sound, fadeTime, loops);
            return;
        }

        if (action is AudioAction.Start)
        {
            var audioSource = GetEmptySource();
            _jobDictionary[audioSource] = new AudioJob()
            {
                Sound = sound,
                Routine = StartCoroutine(AudioActionCoroutine(action, audioSource, sound, soundClip, fadeTime, loops)),
                Clip = soundClip
            };
        }
        if (action is AudioAction.Stop)
        {
            var audioSources = FindSoundSources(sound);
            foreach (var audioSource in audioSources)
            {
                if (_jobDictionary[audioSource] == null) continue;
                if (_jobDictionary[audioSource].Routine != null) StopCoroutine(_jobDictionary[audioSource].Routine);
                _jobDictionary[audioSource].Routine = StartCoroutine(AudioActionCoroutine(action, audioSource, sound, soundClip, fadeTime, loops));
            }
        }
    }

    private IEnumerator AudioActionCoroutine(AudioAction action, AudioSource source, Sound sound, Sound.Clip clip, float fadeTime, bool loops)
    {
        if (action == AudioAction.Start)
        {
            var soundClip = sound.GetClip();
            source.clip = soundClip.AudioClip;
            source.volume = soundClip.Volume * GetExponentialVolume(MasterVolume) * GetExponentialVolume(ChannelVolumeDictionary[_channelDictionary[sound]]);
            source.pitch = sound.HasPitchVariation ? Random.Range(1 - sound.PitchVariation, 1 + sound.PitchVariation) : 1;
            source.loop = loops;
            source.Play();
        }

        float initialVolume = source.volume;
        float time = 0;
        while (time < fadeTime)
        {
            var volumeCoefficent = (action == AudioAction.Stop) ? 1 - time / fadeTime : time / fadeTime;
            source.volume = initialVolume * volumeCoefficent;
            time += Time.deltaTime;
            yield return null;
        }

        if (action == AudioAction.Stop)
        {
            source.Stop();
        }

        _jobDictionary[source] = null;
    }

    private AudioSource GetEmptySource()
    {
        foreach (var entry in _jobDictionary)
        {
            if (entry.Value == null && entry.Key != null && !entry.Key.isPlaying) return entry.Key;
        }
        var newSource = gameObject.AddComponent<AudioSource>();
        _jobDictionary.Add(newSource, null);
        return newSource;
    }

    private List<AudioSource> FindSoundSources(Sound sound)
    {
        var sources = new List<AudioSource>();
        foreach (var entry in _jobDictionary)
        {
            if (entry.Value != null && entry.Value.Sound == sound && entry.Key.isPlaying) sources.Add(entry.Key);
        }
        return sources;
    }

    private float GetExponentialVolume(float volume) => Mathf.Pow(volume, 3);

#if UNITY_EDITOR

    public void PlaySoundInEditor(Sound.Clip clip, Sound sound)
    {
        var audioSources = gameObject.GetComponents<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                DestroyImmediate(audioSources[i]);
                continue;
            }
            if (clip.AudioClip == audioSources[i].clip)
            {
                DestroyImmediate(audioSources[i]);
                return;
            }
        }
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip.AudioClip;
        audioSource.volume = clip.Volume;
        if (sound.HasPitchVariation)
            audioSource.pitch = Random.Range(1 - sound.PitchVariation, 1 + sound.PitchVariation);
        audioSource.Play();
    }

    public bool IsPlayingInEditor(Sound.Clip clip)
    {
        var audioSources = gameObject.GetComponents<AudioSource>();
        foreach (var audioSource in audioSources)
            if (clip.AudioClip == audioSource.clip && audioSource.isPlaying)
                return true;
        return false;
    }

    public void SetClipVolume(Sound.Clip clip)
    {
        var audioSources = gameObject.GetComponents<AudioSource>();
        foreach (var audioSource in audioSources)
            if (clip.AudioClip == audioSource.clip && audioSource.isPlaying)
                audioSource.volume = clip.Volume;
    }

#endif

}
