using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private ClickablePanel _newCardPanel;
    [SerializeField] private Image _newCardImage;
    [SerializeField] private Deck _deck;
    [SerializeField] private SceneTransition _transition;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) HandleMenuClick();
        if (Input.GetKeyDown(KeyCode.R)) OnSolved();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSolved()
    {
        DataManager.GameData.LevelsSolved++;
        Tween.Delay(this, 0.3f, () => GridManager.Instance.MakeDisappear());
        Tween.Delay(this, 1, () =>
        {
            if (DataManager.GameData.LevelsSolved % 3 == 1)
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
        var totalCards = _deck.Abilities.Count;
        if (DataManager.GameData.UnlockedCards.Count == totalCards) return;

        var cardRange = Enumerable.Range(0, totalCards).Where(i => !DataManager.GameData.UnlockedCards.Contains(i));
        var randomCard = cardRange.ElementAt(Random.Range(0, cardRange.Count()));
        DataManager.GameData.UnlockedCards.Add(randomCard);
        DataManager.Save();

        _newCardPanel.gameObject.SetActive(true);
        _newCardImage.sprite = _deck.Abilities[randomCard].Image;
    }

    private void HandleNewCardPanelClick()
    {
        _newCardPanel.gameObject.SetActive(false);
        RestartScene();
    }

    private void HandleMenuClick()
    {
        _transition.GoToMenuScene();
    }

    private void OnEnable()
    {
        GridManager.Solved += OnSolved;
        _menuButton.onClick.AddListener(HandleMenuClick);
        _newCardPanel.OnClick += HandleNewCardPanelClick;
    }

    private void OnDisable()
    {
        GridManager.Solved -= OnSolved;
        _menuButton.onClick.RemoveListener(HandleMenuClick);
        _newCardPanel.OnClick -= HandleNewCardPanelClick;
    }
}
