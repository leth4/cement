using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static event Action Solved;

    [SerializeField] private CellRender _cellPrefab;
    [SerializeField] private LayerMask _cellLayer;
    [SerializeField] private Vector3 _touchSelectOffset;
    [SerializeField] private int _size;
    [SerializeField] private int _abilitiesApplied;
    [SerializeField] private int _numbersToPreview;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;
    [SerializeField] private Deck _deck;

    [HideInInspector] public int Size;
    [HideInInspector] public CellRender SelectedCell { get; private set; }

    public Cell[,] PlayerGrid;
    public Cell[,] PlayerPreviewGrid;
    public Cell[,] AnswerGrid;

    private List<CellRender> AnswerRenders = new();
    private List<CellRender> PlayerRenders = new();

    private void Start()
    {
        InitializeGrids();
        ActivateRandomAbilities();
        AbilityController.Instance.CallChange();
    }

    private void InitializeGrids()
    {
        Size = _size;

        AnswerGrid = new Cell[_size, _size];
        PlayerGrid = new Cell[_size, _size];
        PlayerPreviewGrid = new Cell[_size, _size];

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                AnswerGrid[i, j] = new Cell();
                PlayerGrid[i, j] = new Cell();
                PlayerPreviewGrid[i, j] = new Cell();

                var answerCellRender = Instantiate(_cellPrefab, _answerGridContainer);
                answerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                answerCellRender.Initialize(false, new(i, j), i - j);
                AnswerRenders.Add(answerCellRender);

                var playerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
                playerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                playerCellRender.Initialize(true, new(i, j), i - j);
                PlayerRenders.Add(playerCellRender);
            }
        }

        var phantomCells = new Vector2Int[] {new(-1,-1), new(-1, 0), new(-1, 1), new(-1, 2), new(-1, 3), new(-1, 4), new(-1, 5),
                                             new(0,-1), new(1, -1), new(2, -1), new(3, -1), new(4, -1), new(5, -1), new(5,0),
                                             new(5, 1), new(5, 2), new(5, 3), new(5, 4), new(5, 5), new(0,5), new(1, 5),
                                             new(2, 5), new(3, 5), new(4, 5)};

        foreach (var phantomCell in phantomCells)
        {
            var phantomPlayerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
            phantomPlayerCellRender.transform.localPosition = new(phantomCell.x - (float)Size / 2 + 0.5f, phantomCell.y - (float)Size / 2 + 0.5f);
            phantomPlayerCellRender.Initialize(true, phantomCell, 0);
            phantomPlayerCellRender.MakePhantom();
        }

        AnswerGrid[_size / 2, _size / 2].SetTaken(true, false);
        PlayerGrid[_size / 2, _size / 2].SetTaken(true, false);
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
                if (AnswerGrid[i, j].IsTaken != PlayerGrid[i, j].IsTaken) return;

        Solved?.Invoke();
    }

    private void ActivateRandomAbilities()
    {
        Recorder.Instance.Reset();

        var abilities = new List<Ability>();

        while (true)
        {
            var gridCopy = AnswerGrid.Clone() as Cell[,];

            abilities = new List<Ability>();
            var types = new List<Ability.AbilityType>();
            var shapes = new List<Shape>();
            for (int i = 0; i < _abilitiesApplied; i++)
            {
                var ability = GetRandomAbility(i == 0);

                if (abilities.Contains(ability))
                {
                    i--;
                    continue;
                }

                if (types.Contains(ability.Type) && UnityEngine.Random.Range(0, 4) != 0)
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
                types.Add(ability.Type);
                abilities.Add(ability);
            }

            var preTakenCells = new List<CellRender>();
            foreach (var cell in AnswerRenders)
            {
                if (gridCopy[cell.Coordinates.x, cell.Coordinates.y].IsTaken) preTakenCells.Add(cell);
            }

            if (preTakenCells.Count > 4)
            {
                AnswerGrid = gridCopy.Clone() as Cell[,];
                break;
            }
        }

        HandController.Instance.SaveSortedOrder(abilities);

        abilities.Shuffle();

        foreach (var ability in abilities) HandController.Instance.AddCard(ability);

        AbilityController.Instance.CallChange();

        var takenCells = new List<CellRender>();
        foreach (var cell in AnswerRenders)
        {
            if (AnswerGrid[cell.Coordinates.x, cell.Coordinates.y].IsTaken) takenCells.Add(cell);
        }

        takenCells.Shuffle();
        takenCells.OrderByDescending(cell => AnswerGrid[cell.Coordinates.x, cell.Coordinates.y].Number);

        for (int i = 0; i < Mathf.Min(takenCells.Count, _numbersToPreview); i++)
        {
            if (AnswerGrid[takenCells[i].Coordinates.x, takenCells[i].Coordinates.y].Number == 0) continue;
            takenCells[i].ShowNumber();
        }
    }

    public void Update()
    {
        var deviceOffset = Input.touchCount > 0 ? _touchSelectOffset : Vector3.zero;

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

        Log.Message(randomWeight, currentWeight);

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
