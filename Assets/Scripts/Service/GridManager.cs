using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public int Size;

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private int _size;
    [SerializeField] private int _randomApplied;
    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;

    [SerializeField] private List<AbilityWeight> _abilities;

    [HideInInspector] public Cell SelectedCell;
    public Cell[,] PlayerGrid;

    private Cell[,] _answerGrid;

    private void Start()
    {
        InitializeGrids();
    }

    private void InitializeGrids()
    {
        Size = _size;

        _answerGrid = new Cell[_size, _size];
        PlayerGrid = new Cell[_size, _size];

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _answerGrid[i, j] = Instantiate(_cellPrefab, _answerGridContainer);
                _answerGrid[i, j].transform.localPosition = new(i, j);
                _answerGrid[i, j].Initialize(false);
                PlayerGrid[i, j] = Instantiate(_cellPrefab, _playerGridContainer);
                PlayerGrid[i, j].transform.localPosition = new(i, j);
                PlayerGrid[i, j].Initialize(true);
            }
        }

        _answerGrid[_size / 2, _size / 2].SetTaken(true, false);
        PlayerGrid[_size / 2, _size / 2].SetTaken(true, false);
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
            ability.ApplyRandom(_answerGrid);
        }

        Debug.Log(abilities.ToString(", "));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ActivateRandomAbilities();
    }

    public void Activate(Ability ability) => ability.ApplyRandom(_answerGrid);

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
