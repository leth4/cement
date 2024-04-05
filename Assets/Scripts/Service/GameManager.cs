using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _hintButton;
    [SerializeField] private GameObject _extraCardsText;
    [SerializeField] private ClickablePanel _newCardPanel;
    [SerializeField] private Image _newCardImage;
    [SerializeField] private TMP_Text _newCardText;
    [SerializeField] private Deck _deck;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private bool _isTutorial = false;
    [SerializeField] private Tutorial _tutorial;

    public static int LevelCardsCount = 4;

    public bool UsedHint { get; private set; }
    public bool IsLevelSolved { get; private set; }

    private void Start()
    {
        GenerateNewLevel();
    }

    private void GenerateNewLevel()
    {
        IsLevelSolved = false;
        UsedHint = false;
        _hintButton.interactable = true;
        Recorder.Instance.Reset();

        var level = _isTutorial ? _tutorial.NextStage() : _levelGenerator.GenerateRandomLevel(LevelCardsCount);

        GridManager.Instance.InitializeGrids(level.Grid);
        HandController.Instance.SaveSortedOrder(level.Abilities);

        level.Abilities.Shuffle();
        AudioReceiver.CardAdded();

        for (int i = 0; i < level.Abilities.Count; i++)
        {
            HandController.Instance.AddCard(level.Abilities[i], i);
        }
    }

    public void ResetLevel(bool playSound)
    {
        if (playSound) AudioReceiver.AbilityUndone();

        for (int i = 0; i < LevelCardsCount; i++)
        {
            Recorder.Instance.GoBack();
        }
    }

    private void HandleLevelSolved()
    {
        if (IsLevelSolved) return;
        IsLevelSolved = true;

        if (HandController.Instance.HasCardsLeft && !_isTutorial)
        {
            HandController.Instance.RemoveAllCards();
            _extraCardsText.SetActive(true);
        }

        AudioReceiver.LevelSolved();

        if (!UsedHint && !_isTutorial)
        {
            if (LevelCardsCount == 3) DataManager.GameData.Levels3Solved++;
            if (LevelCardsCount == 4) DataManager.GameData.Levels4Solved++;
            if (LevelCardsCount == 5) DataManager.GameData.Levels5Solved++;
        }

        Tween.Delay(this, 0.3f, () => GridManager.Instance.MakeGridsDisappear());
        Tween.Delay(this, 1, () =>
        {
            if (!UsedHint && !_isTutorial && DataManager.GameData.LevelsSolved % 2 == 1 && DataManager.GameData.UnlockedCards.Count != _deck.Abilities.Count)
            {
                UnlockNewCard();
            }
            else
            {
                DataManager.Save();

                if (_isTutorial && _tutorial.IsLastStage)
                {
                    if (!DataManager.GameData.ShownTutorial)
                    {
                        DataManager.GameData.ShownTutorial = true;
                        DataManager.Save();
                    }
                    _transition.GoToMenuScene();
                }
                else
                {
                    GenerateNewLevel();
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
        GenerateNewLevel();
    }

    public void HandleMenuClick()
    {
        AudioReceiver.ButtonPressed();
        _transition.GoToMenuScene();
    }

    private void HandleHintClick()
    {
        AudioReceiver.ButtonPressed();
        UsedHint = true;
        HandController.Instance.SortCards();
        _hintButton.interactable = false;
        ResetLevel(false);
    }

    private void OnEnable()
    {
        GridManager.OnSolved += HandleLevelSolved;
        _menuButton.onClick.AddListener(HandleMenuClick);
        _hintButton.onClick.AddListener(HandleHintClick);
        _newCardPanel.OnClick += HandleNewCardPanelClick;
    }

    private void OnDisable()
    {
        GridManager.OnSolved -= HandleLevelSolved;
        _menuButton.onClick.RemoveListener(HandleMenuClick);
        _hintButton.onClick.RemoveListener(HandleHintClick);
        _newCardPanel.OnClick -= HandleNewCardPanelClick;
    }
}
