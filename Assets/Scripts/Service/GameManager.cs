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
    [SerializeField] private ClickablePanel _newCardPanel;
    [SerializeField] private Image _newCardImage;
    [SerializeField] private TMP_Text _newCardText;
    [SerializeField] private Deck _deck;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private bool _isTutorial = false;

    public static int LevelCardsCount;

    public static event Action SortCards;

    private bool _usedHint;

    public bool IsSolved { get; private set; } = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleMenuClick();
        if (Input.GetKeyDown(KeyCode.M)) OnSolved();
        if (Input.GetKeyDown(KeyCode.R)) RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSolved()
    {
        if (IsSolved) return;
        IsSolved = true;
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
