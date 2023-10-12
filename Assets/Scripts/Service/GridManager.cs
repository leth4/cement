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

        var abilities = new List<string>();

        for (int i = 0; i < _abilitiesApplied; i++)
        {
            var ability = GetRandomAbility(i == 0);
            var applied = ability.ApplyRandom(AnswerGrid);
            if (applied)
            {
                HandController.Instance.AddCard(ability);
            }
            else
            {
                i--;
            }
        }

        HandController.Instance.ShuffleCards();

        AbilityController.Instance.CallChange();

        var takenCells = new List<CellRender>();
        foreach (var cell in AnswerRenders)
        {
            if (AnswerGrid[cell.Coordinates.x, cell.Coordinates.y].IsTaken) takenCells.Add(cell);
        }

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
        var abilityOffset = Vector3.zero;
        if (HandController.Instance.ActiveAbility != null) abilityOffset = HandController.Instance.ActiveAbility.SelectionOffset.ToVector3();

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) + deviceOffset + abilityOffset, Vector2.zero, 1000, _cellLayer);

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
