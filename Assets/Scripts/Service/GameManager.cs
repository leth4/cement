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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) RestartScene();
        if (Input.GetKeyDown(KeyCode.R)) OnSolved();
        if (Input.touches.Length == 3) RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSolved()
    {
        DataManager.GameData.LevelsSolved++;
        if (DataManager.GameData.LevelsSolved % 5 == 0)
        {
            UnlockNewCard();
        }
        else
        {
            DataManager.Save();
            RestartScene();
        }
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
        SceneManager.LoadScene("Menu");
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
