using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button hintButton;
    [SerializeField] private GameObject _extraCardsText;
    [SerializeField] private ClickablePanel _newCardPanel;
    [SerializeField] private Image _newCardImage;
    [SerializeField] private TMP_Text _newCardText;
    [SerializeField] private Deck _deck;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private bool _isTutorial = false;
    [SerializeField] private float _doubleTapThreshold = 0.3f;

    public static int LevelCardsCount = 4;

    public static event Action SortCards;

    private bool _usedHint;

    public bool IsSolved { get; private set; } = false;

    private float _timeSinceLastTap = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleMenuClick();
        if (Input.GetKeyDown(KeyCode.R)) ResetState();

        _timeSinceLastTap += Time.unscaledDeltaTime;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (_timeSinceLastTap < _doubleTapThreshold) ResetState();
            _timeSinceLastTap = 0;
        }
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetState()
    {
        Recorder.Instance.GoBack();
        Recorder.Instance.GoBack();
        Recorder.Instance.GoBack();
        Recorder.Instance.GoBack();
        Recorder.Instance.GoBack();
    }

    private void OnSolved()
    {
        if (IsSolved) return;
        IsSolved = true;

        if (HandController.Instance.HasCardsLeft && !_isTutorial)
        {
            HandController.Instance.RemoveAllCards();
            _extraCardsText.SetActive(true);
        }

        AudioReceiver.LevelSolved();
        if (!_usedHint && !_isTutorial)
        {
            if (LevelCardsCount == 3) DataManager.GameData.Levels3Solved++;
            if (LevelCardsCount == 4) DataManager.GameData.Levels4Solved++;
            if (LevelCardsCount == 5) DataManager.GameData.Levels5Solved++;
        }
        Tween.Delay(this, 0.3f, () => GridManager.Instance.MakeDisappear());
        Tween.Delay(this, 1, () =>
        {
            if (!_usedHint && !_isTutorial && DataManager.GameData.LevelsSolved % 2 == 1 && DataManager.GameData.UnlockedCards.Count != _deck.Abilities.Count)
            {
                UnlockNewCard();
            }
            else
            {
                DataManager.Save();

                if (_isTutorial && Tutorial.IsLastStage)
                {
                    if (!DataManager.GameData.ShownTutorial)
                    {
                        DataManager.GameData.ShownTutorial = true;
                        DataManager.Save();
                    }
                    Tutorial.Reset();
                    _transition.GoToMenuScene();
                }
                else
                {
                    RestartScene();
                }
            }
        });
    }

    private void UnlockNewCard()
    {
        AudioReceiver.NewCardUnlocked();

        var totalCards = _deck.Abilities.Count;

        var cardRange = Enumerable.Range(0, totalCards).Where(i => !DataManager.GameData.UnlockedCards.Contains(i));
        var randomCard = cardRange.ElementAt(UnityEngine.Random.Range(0, cardRange.Count()));
        DataManager.GameData.UnlockedCards.Add(randomCard);
        DataManager.Save();

        _newCardPanel.gameObject.SetActive(true);
        _newCardImage.sprite = _deck.Abilities[randomCard].Image;
        _newCardText.SetText($"{DataManager.GameData.UnlockedCards.Count}/{_deck.Abilities.Count}");
    }

    private void HandleNewCardPanelClick()
    {
        _newCardPanel.gameObject.SetActive(false);
        RestartScene();
    }

    private void HandleMenuClick()
    {
        AudioReceiver.ButtonPressed();
        _transition.GoToMenuScene();
    }

    private void HandleHintClick()
    {
        AudioReceiver.ButtonPressed();
        SortCards?.Invoke();
        _usedHint = true;
        hintButton.interactable = false;
        ResetState();
    }

    private void OnEnable()
    {
        GridManager.Solved += OnSolved;
        _menuButton.onClick.AddListener(HandleMenuClick);
        hintButton.onClick.AddListener(HandleHintClick);
        _newCardPanel.OnClick += HandleNewCardPanelClick;
    }

    private void OnDisable()
    {
        GridManager.Solved -= OnSolved;
        _menuButton.onClick.RemoveListener(HandleMenuClick);
        hintButton.onClick.RemoveListener(HandleHintClick);
        _newCardPanel.OnClick -= HandleNewCardPanelClick;
    }
}
