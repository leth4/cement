using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<TutorialLevel> _levels;
    [SerializeField] private TMP_Text _tutorialText;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private GameObject _hintButton;

    private int _stage = 0;
    public bool IsLastStage => _stage >= _levels.Count;

    public Level NextStage()
    {
        var level = _levels[_stage];

        _tutorialText.SetText(Application.isMobilePlatform ? level.TextMobile : level.TextComputer);

        _hintButton.SetActive(level.ShowHint);

        _stage++;

        return new Level() { Abilities = level.Abilities, Grid = level.Shape.Grid };
    }

    [System.Serializable]
    private struct TutorialLevel
    {
        public Shape Shape;
        public List<Ability> Abilities;
        [Multiline] public string TextComputer;
        [Multiline] public string TextMobile;
        public bool ShowHint;
    }
}