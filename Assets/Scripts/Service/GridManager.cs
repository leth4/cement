using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static event Action Solved;

    [SerializeField] private CellRender _cellPrefab;
    [SerializeField] private LayerMask _cellLayer;
    [SerializeField] private Vector3 _touchSelectOffset;
    [SerializeField] private int _size;
    [SerializeField] private int _numbersToPreview;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;
    [SerializeField] private Deck _deck;

    [HideInInspector] public int Size;
    [HideInInspector] public CellRender SelectedCell { get; private set; }

    public bool[,] PlayerGrid;
    public bool[,] PlayerPreviewGrid;
    public bool[,] AnswerGrid;

    private List<CellRender> AnswerRenders = new();
    private List<CellRender> PlayerRenders = new();

    [SerializeField] public bool _isTutorialScene = false;

    private void Start()
    {
        if (_isTutorialScene) return;

        InitializeGrids();
        ActivateRandomAbilities();
        AbilityController.Instance.CallChange();
    }

    public void ShowTutorial(Shape tutorialShape, List<Ability> abilities)
    {
        InitializeGrids();

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (tutorialShape.Grid[i, j]) AnswerGrid[i, j] = true;
            }
        }

        Recorder.Instance.Reset();

        HandController.Instance.SaveSortedOrder(abilities);

        if (abilities.Count > 2) abilities.Reverse();

        AudioReceiver.CardAdded();

        foreach (var ability in abilities) HandController.Instance.AddCard(ability);

        AbilityController.Instance.CallChange();
    }

    private void InitializeGrids(bool showNumbers = true)
    {
        Size = _size;

        AnswerGrid = new bool[_size, _size];
        PlayerGrid = new bool[_size, _size];
        PlayerPreviewGrid = new bool[_size, _size];

        for (int i = -1; i <= _size; i++)
        {
            for (int j = -1; j <= _size; j++)
            {
                bool isPhantomCell = i < 0 || i >= _size || j < 0 || j >= _size;

                if (!isPhantomCell)
                {
                    var answerCellRender = Instantiate(_cellPrefab, _answerGridContainer);
                    answerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                    answerCellRender.Initialize(false, new(i, j), i - j);
                    AnswerRenders.Add(answerCellRender);
                }

                var playerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
                playerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                playerCellRender.Initialize(true, new(i, j), i - j);

                if (isPhantomCell)
                {
                    playerCellRender.MakePhantom();
                }
                else
                {
                    PlayerRenders.Add(playerCellRender);
                }
            }
        }

        if (!_isTutorialScene) AnswerGrid[_size / 2, _size / 2] = true;
        PlayerGrid[_size / 2, _size / 2] = true;
    }

    public void MakeDisappear()
    {
        for (int i = 0; i < AnswerRenders.Count; i++)
        {
            AnswerRenders[i].Hide(i);
            PlayerRenders[i].Hide(i);
        }
    }

    private void CheckWin()
    {
        for (int i = 0; i < _size; i++)
            for (int j = 0; j < _size; j++)
                if (AnswerGrid[i, j] != PlayerGrid[i, j]) return;

        Solved?.Invoke();
    }

    private void ActivateRandomAbilities()
    {
        Recorder.Instance.Reset();

        var abilities = new List<Ability>();

        while (true)
        {
            var gridCopy = AnswerGrid.Clone() as bool[,];

            abilities = new List<Ability>();
            var types = new List<Type>();
            var shapes = new List<Shape>();
            for (int i = 0; i < GameManager.LevelCardsCount; i++)
            {
                var ability = GetRandomAbility(i == 0);

                if (abilities.Contains(ability))
                {
                    i--;
                    continue;
                }

                if (types.Contains(ability.GetType()) && UnityEngine.Random.Range(0, 4) != 0)
                {
                    i--;
                    continue;
                }

                if (ability is ApplyShapeAbility && shapes.Contains(((ApplyShapeAbility)ability).Shape) && UnityEngine.Random.Range(0, 3) != 0)
                {
                    i--;
                    continue;
                }

                var applied = ability.ApplyRandom(gridCopy);
                if (!applied)
                {
                    i--;
                    continue;
                }

                if (ability is ApplyShapeAbility) shapes.Add(((ApplyShapeAbility)ability).Shape);
                types.Add(ability.GetType());
                abilities.Add(ability);
            }

            var preTakenCells = new List<CellRender>();
            foreach (var cell in AnswerRenders)
            {
                if (gridCopy[cell.Coordinates.x, cell.Coordinates.y]) preTakenCells.Add(cell);
            }

            if (preTakenCells.Count > 4)
            {
                AnswerGrid = gridCopy.Clone() as bool[,];
                break;
            }
        }

        HandController.Instance.SaveSortedOrder(abilities);

        abilities.Shuffle();

        AudioReceiver.CardAdded();

        foreach (var ability in abilities) HandController.Instance.AddCard(ability);

        AbilityController.Instance.CallChange();
    }

    public void Update()
    {
        var deviceOffset = Application.isMobilePlatform ? _touchSelectOffset : Vector3.zero;

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) + deviceOffset, Vector2.zero, 1000, _cellLayer);

        if (hit.collider == null)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        var cellRender = hit.transform.GetComponent<CellRender>();

        if (!cellRender.IsPlayerCell)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        if (cellRender.IsPhantom && HandController.Instance.ActiveAbility && HandController.Instance.ActiveAbility.IsFullCanvas)
        {
            if (SelectedCell != null) AbilityController.Instance.StopPreview();
            SelectedCell = null;
            return;
        }

        bool isNewCell = SelectedCell != cellRender;
        SelectedCell = cellRender;

        if (isNewCell) AbilityController.Instance.UpdatePreview();
    }

    public void Activate(Ability ability) => ability.ApplyRandom(AnswerGrid);

    private Ability GetRandomAbility(bool isFirst)
    {
        float weightSum = 0;
        foreach (var ability in _deck.Abilities)
            if (!isFirst || ability.CanBeFirst) weightSum += ability.Weight;

        float randomWeight = UnityEngine.Random.Range(0, weightSum);

        float currentWeight = 0;

        foreach (var ability in _deck.Abilities)
        {
            if (isFirst && !ability.CanBeFirst) continue;
            currentWeight += ability.Weight;
            if (randomWeight <= currentWeight)
                return ability;
        }

        return null;
    }

    private void OnEnable()
    {
        AbilityController.MadeChanges += CheckWin;
    }

    private void OnDisable()
    {
        AbilityController.MadeChanges -= CheckWin;
    }

    [System.Serializable]
    private struct AbilityWeight
    {
        public Ability Ability;
        public float Weight;
        public float WeightFirst;
    }
}
