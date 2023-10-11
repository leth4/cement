using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;

    private float _musicVolume;
    private float _soundVolume;

    private void Start()
    {
        _musicVolume = Data.Load<FloatData>("MusicVolume").Data;
        _soundVolume = Data.Load<FloatData>("SoundVolume").Data;

        _musicSlider.value = _musicVolume;
        _soundSlider.value = _soundVolume;

        AudioManager.Instance.SetChannelVolume(ChannelEnum.Music, _musicVolume);
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Sounds, _soundVolume);
    }

    private void StartPlaying()
    {
        SceneManager.LoadScene("Main");
    }

    private void HandleMusicSliderValueChanged(float value)
    {
        _musicVolume = value;
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Music, value);
        Data.Save(new FloatData(_musicVolume), "MusicVolume");
    }

    private void HandleSoundSliderValueChanged(float value)
    {
        _soundVolume = value;
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Sounds, value);
        Data.Save(new FloatData(_soundVolume), "SoundVolume");
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(StartPlaying);
        _musicSlider.onValueChanged.AddListener(HandleMusicSliderValueChanged);
        _soundSlider.onValueChanged.AddListener(HandleSoundSliderValueChanged);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(StartPlaying);
        _musicSlider.onValueChanged.RemoveListener(HandleMusicSliderValueChanged);
        _soundSlider.onValueChanged.RemoveListener(HandleSoundSliderValueChanged);
    }
}