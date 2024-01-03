using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private List<TutorialLevel> _levels;
    [SerializeField] private TMP_Text _tutorialText;
    [SerializeField] private SceneTransition _transition;
    [SerializeField] private GameObject _hintButton;

    private static int _stage = 0;
    public static bool IsLastStage = false;

    public static void Reset()
    {
        _stage = 0;
        IsLastStage = false;
    }

    public void Start()
    {
        var level = _levels[_stage];
        _stage++;

        if (_stage >= _levels.Count) IsLastStage = true;

        _tutorialText.SetText(Application.isMobilePlatform ? level.TextMobile : level.TextComputer);

        if (!level.ShowHint)
        {
            _hintButton.SetActive(false);
        }
        GameManager.LevelCardsCount = level.Abilities.Count;
        GridManager.Instance.ShowTutorial(level.Shape, level.Abilities);
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
