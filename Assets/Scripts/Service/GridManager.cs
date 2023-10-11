using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public static event Action Solved;

    [SerializeField] private CellRender _cellPrefab;
    [SerializeField] private int _size;
    [SerializeField] private int _abilitiesApplied;
    [SerializeField] private int _numbersToPreview;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;
    [SerializeField] private List<AbilityWeight> _abilities;

    [HideInInspector] public int Size;
    [HideInInspector] public CellRender SelectedCell;

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
                answerCellRender.Initialize(false, new(i, j));
                AnswerRenders.Add(answerCellRender);

                var playerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
                playerCellRender.transform.localPosition = new(i - (float)Size / 2 + 0.5f, j - (float)Size / 2 + 0.5f);
                playerCellRender.Initialize(true, new(i, j));
                PlayerRenders.Add(playerCellRender);
            }
        }

        AnswerGrid[_size / 2, _size / 2].SetTaken(true, false);
        PlayerGrid[_size / 2, _size / 2].SetTaken(true, false);
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
            abilities.Add(ability.Name);
            HandController.Instance.AddCard(ability);
            ability.ApplyRandom(AnswerGrid);
        }

        AbilityController.Instance.CallChange();

        Debug.Log(abilities.ToString(", "));

        var takenCells = new List<CellRender>();
        foreach (var cell in AnswerRenders)
        {
            if (AnswerGrid[cell.Coordinates.x, cell.Coordinates.y].IsTaken) takenCells.Add(cell);
        }

        takenCells.OrderByDescending(cell => AnswerGrid[cell.Coordinates.x, cell.Coordinates.y].Number);

        for (int i = 0; i < Mathf.Min(takenCells.Count, _numbersToPreview); i++)
        {
            takenCells[i].ShowNumber();
        }
    }

    public void Activate(Ability ability) => ability.ApplyRandom(AnswerGrid);

    private Ability GetRandomAbility(bool isFirst)
    {
        float weightSum = 0;
        foreach (var ability in _abilities) weightSum += isFirst ? ability.WeightFirst : ability.Weight;

        float randomWeight = UnityEngine.Random.Range(0, weightSum);

        float currentWeight = 0;

        foreach (var ability in _abilities)
        {
            currentWeight += isFirst ? ability.WeightFirst : ability.Weight;
            if (randomWeight <= currentWeight)
                return ability.Ability;
        }

        return default;
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
