using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private GameObject _tutorialPanel;
    [SerializeField] private ClickablePanel _clickablePanel;
    [SerializeField] private TMP_Text _tutorialText;
    [SerializeField] private SceneTransition _transition;

    [SerializeField][TextArea] private List<string> _texts;
    [SerializeField][TextArea] private List<string> _mobileTexts;

    private bool _startGameAfterEnd = false;
    private int _currentTutorialPage = -1;

    private void Activate()
    {
        DataManager.GameData.ShownTutorial = true;
        DataManager.Save();
        _tutorialText.SetText("");
        _startGameAfterEnd = false;
        _tutorialPanel.SetActive(true);
        _currentTutorialPage = 0;
        HandlePanelClick();
    }

    public void ActivateThenPlay()
    {
        _tutorialText.SetText("");
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
            if (_startGameAfterEnd) _transition.GoToMainScene();
            return;
        }

        if (Application.isMobilePlatform)
        {
            _tutorialText.SetText(_tutorialText.text + _mobileTexts[_currentTutorialPage] + "\n\n");
        }
        else
        {
            _tutorialText.SetText(_tutorialText.text + _texts[_currentTutorialPage] + "\n\n");
        }

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
