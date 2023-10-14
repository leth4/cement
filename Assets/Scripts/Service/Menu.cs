using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private TMP_Text _levelCounterText;
    [SerializeField] private Button _play3Button;
    [SerializeField] private Button _play4Button;
    [SerializeField] private Button _play5Button;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private Deck _deck;

    private float _musicVolume;
    private float _soundVolume;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        Tutorial.Reset();
        AudioReceiver.StartMusic();

        _musicVolume = DataManager.GameData.MusicVolume;
        _soundVolume = DataManager.GameData.SoundVolume;

        _musicSlider.value = _musicVolume;
        _soundSlider.value = _soundVolume;

        AudioManager.Instance.SetChannelVolume(ChannelEnum.Music, _musicVolume);
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Sounds, _soundVolume);

        _levelCounterText.SetText($"Cement {DataManager.GameData.Levels3Solved}×{DataManager.GameData.Levels4Solved}×{DataManager.GameData.Levels5Solved}");
    }

    private void StartPlaying(int number)
    {
        GameManager.LevelCardsCount = number;
        AudioReceiver.ButtonPressed();
        if (!DataManager.GameData.ShownTutorial)
        {
            _transition.GoToTutorialScene();
        }
        else
        {
            _transition.GoToMainScene();
        }
    }

    private void StartPlaying3() => StartPlaying(3);
    private void StartPlaying4() => StartPlaying(4);
    private void StartPlaying5() => StartPlaying(5);

    private void StartTutorial()
    {
        _transition.GoToTutorialScene();
    }

    private void HandleMusicSliderValueChanged(float value)
    {
        DataManager.GameData.MusicVolume = value;
        DataManager.Save();
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Music, value);
    }

    private void HandleSoundSliderValueChanged(float value)
    {
        _soundVolume = value;
        DataManager.GameData.SoundVolume = value;
        DataManager.Save();
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Sounds, value);
    }

    private void OnEnable()
    {
        _play3Button.onClick.AddListener(StartPlaying3);
        _play4Button.onClick.AddListener(StartPlaying4);
        _play5Button.onClick.AddListener(StartPlaying5);
        _tutorialButton.onClick.AddListener(StartTutorial);
        _musicSlider.onValueChanged.AddListener(HandleMusicSliderValueChanged);
        _soundSlider.onValueChanged.AddListener(HandleSoundSliderValueChanged);
    }

    private void OnDisable()
    {
        _play4Button.onClick.RemoveListener(StartPlaying3);
        _play4Button.onClick.RemoveListener(StartPlaying4);
        _play5Button.onClick.RemoveListener(StartPlaying5);
        _tutorialButton.onClick.RemoveListener(StartTutorial);
        _musicSlider.onValueChanged.RemoveListener(HandleMusicSliderValueChanged);
        _soundSlider.onValueChanged.RemoveListener(HandleSoundSliderValueChanged);
    }
}