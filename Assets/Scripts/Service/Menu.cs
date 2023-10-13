using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private TMP_Text _levelCounterText;
    [SerializeField] private Button _button;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private Deck _deck;

    private float _musicVolume;
    private float _soundVolume;

    private void Start()
    {
        AudioReceiver.StartMusic();

        _musicVolume = DataManager.GameData.MusicVolume;
        _soundVolume = DataManager.GameData.SoundVolume;

        _musicSlider.value = _musicVolume;
        _soundSlider.value = _soundVolume;

        AudioManager.Instance.SetChannelVolume(ChannelEnum.Music, _musicVolume);
        AudioManager.Instance.SetChannelVolume(ChannelEnum.Sounds, _soundVolume);

        _levelCounterText.SetText($"{DataManager.GameData.LevelsSolved} solved");
    }

    private void StartPlaying()
    {
        AudioReceiver.ButtonPressed();
        if (!DataManager.GameData.ShownTutorial)
        {
            DataManager.GameData.ShownTutorial = true;
            DataManager.Save();
            _tutorial.ActivateThenPlay();
        }
        else
        {
            _transition.GoToMainScene();
        }
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