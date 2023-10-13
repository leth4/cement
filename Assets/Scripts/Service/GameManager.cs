using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _tipButton1;
    [SerializeField] private Button _tipButton2;
    [SerializeField] private ClickablePanel _newCardPanel;
    [SerializeField] private Image _newCardImage;
    [SerializeField] private TMP_Text _newCardText;
    [SerializeField] private Deck _deck;
    [SerializeField] private SceneTransition _transition;

    public static event Action ShowNumbers;
    public static event Action SortCards;

    private bool _usedTips;

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
        if (!_usedTips) DataManager.GameData.LevelsSolved++;
        Tween.Delay(this, 0.3f, () => GridManager.Instance.MakeDisappear());
        Tween.Delay(this, 1, () =>
        {
            if (!_usedTips && DataManager.GameData.LevelsSolved % 3 == 1)
            {
                UnlockNewCard();
            }
            else
            {
                DataManager.Save();
                RestartScene();
            }
        });
    }


    private void UnlockNewCard()
    {
        AudioReceiver.NewCardUnlocked();

        var totalCards = _deck.Abilities.Count;
        if (DataManager.GameData.UnlockedCards.Count == totalCards) return;

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

    private void HandleFirstTipClick()
    {
        AudioReceiver.ButtonPressed();
        ShowNumbers?.Invoke();
        _usedTips = true;
        _tipButton1.interactable = false;
    }

    private void HandleSecondTipClick()
    {
        AudioReceiver.ButtonPressed();
        SortCards?.Invoke();
        _usedTips = true;
        _tipButton2.interactable = false;
    }

    private void OnEnable()
    {
        GridManager.Solved += OnSolved;
        _menuButton.onClick.AddListener(HandleMenuClick);
        _tipButton1.onClick.AddListener(HandleFirstTipClick);
        _tipButton2.onClick.AddListener(HandleSecondTipClick);
        _newCardPanel.OnClick += HandleNewCardPanelClick;
    }

    private void OnDisable()
    {
        GridManager.Solved -= OnSolved;
        _menuButton.onClick.RemoveListener(HandleMenuClick);
        _tipButton1.onClick.RemoveListener(HandleFirstTipClick);
        _tipButton2.onClick.RemoveListener(HandleSecondTipClick);
        _newCardPanel.OnClick -= HandleNewCardPanelClick;
    }
}
