using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{

    [SerializeField] private CellRender _cellPrefab;
    [SerializeField] private int _size;
    [SerializeField] private int _randomApplied;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;
    [SerializeField] private List<AbilityWeight> _abilities;

    [HideInInspector] public int Size;
    [HideInInspector] public CellRender SelectedCell;

    public Cell[,] PlayerGrid;
    public Cell[,] PlayerPreviewGrid;
    public Cell[,] AnswerGrid;

    private void Start()
    {
        InitializeGrids();
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

                var cellRender = Instantiate(_cellPrefab, _answerGridContainer);
                cellRender.transform.localPosition = new(i, j);
                cellRender.Initialize(false, new(i, j));

                var playerCellRender = Instantiate(_cellPrefab, _playerGridContainer);
                playerCellRender.transform.localPosition = new(i, j);
                playerCellRender.Initialize(true, new(i, j));
            }
        }

        AnswerGrid[_size / 2, _size / 2].SetTaken(true, false);
        PlayerGrid[_size / 2, _size / 2].SetTaken(true, false);

        AbilityController.Instance.CallChange();
    }

    private void ActivateRandomAbilities()
    {
        Recorder.Instance.Reset();

        var abilities = new List<string>();

        for (int i = 0; i < _randomApplied; i++)
        {
            var ability = GetRandomAbility(i == 0);
            abilities.Add(ability.Name);
            AbilityController.Instance.AddAbility(ability);
            ability.ApplyRandom(AnswerGrid);
        }

        AbilityController.Instance.CallChange();

        Debug.Log(abilities.ToString(", "));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ActivateRandomAbilities();
    }

    public void Activate(Ability ability) => ability.ApplyRandom(AnswerGrid);

    private Ability GetRandomAbility(bool isFirst)
    {
        float weightSum = 0;
        foreach (var ability in _abilities) weightSum += isFirst ? ability.WeightFirst : ability.Weight;

        float randomWeight = Random.Range(0, weightSum);

        float currentWeight = 0;

        foreach (var ability in _abilities)
        {
            currentWeight += isFirst ? ability.WeightFirst : ability.Weight;
            if (randomWeight <= currentWeight)
                return ability.Ability;
        }

        return default;
    }

    [System.Serializable]
    private struct AbilityWeight
    {
        public Ability Ability;
        public float Weight;
        public float WeightFirst;
    }
}
