using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private ClickablePanel _clickablePanel;
    [SerializeField] private TMP_Text _tutorialText;

    [SerializeField][TextArea] private List<string> _texts;

    private bool _startGameAfterEnd = false;
    private int _currentTutorialPage = -1;

    private void Activate()
    {
        DataManager.GameData.ShownTutorial = true;
        DataManager.Save();
        _startGameAfterEnd = false;
        _tutorialPanel.SetActive(true);
        _currentTutorialPage = 0;
        HandlePanelClick();
    }

    public void ActivateThenPlay()
    {
        _startGameAfterEnd = true;
        _tutorialPanel.SetActive(true);
        _currentTutorialPage = 0;
        HandlePanelClick();
    }

    private void HandlePanelClick()
    {
        if (_currentTutorialPage == -1) return;

        if (_currentTutorialPage > _texts.Count - 1)
        {
            _tutorialPanel.SetActive(false);
            _currentTutorialPage = -1;
            if (_startGameAfterEnd) SceneManager.LoadScene("Main");
            return;
        }

        Debug.Log(_currentTutorialPage);

        _tutorialText.SetText(_texts[_currentTutorialPage]);

        _currentTutorialPage++;
    }

    private void OnEnable()
    {
        _tutorialButton.onClick.AddListener(Activate);
        _clickablePanel.OnClick += HandlePanelClick;
    }

    private void OnDestroy()
    {
        _tutorialButton.onClick.RemoveListener(Activate);
        _clickablePanel.OnClick -= HandlePanelClick;
    }

}