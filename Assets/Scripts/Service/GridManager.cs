using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static int Size;

    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private int _size;
    [SerializeField] private int _randomApplied;

    [SerializeField] private List<AbilityWeight> _abilities;

    [SerializeField] private Transform _answerGridContainer;
    [SerializeField] private Transform _playerGridContainer;

    private Cell[,] _grid;
    private Cell[,] _playerGrid;

    private void Start()
    {
        InitializeGrids();
    }

    private void InitializeGrids()
    {
        Size = _size;

        _grid = new Cell[_size, _size];
        _playerGrid = new Cell[_size, _size];

        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _grid[i, j] = Instantiate(_cellPrefab, _answerGridContainer);
                _grid[i, j].transform.localPosition = new(i, j);
                _grid[i, j].Initialize(false);
                _playerGrid[i, j] = Instantiate(_cellPrefab, _playerGridContainer);
                _playerGrid[i, j].transform.localPosition = new(i, j);
                _grid[i, j].Initialize(true);
            }
        }

        _grid[_size / 2, _size / 2].IsTaken = true;
        _playerGrid[_size / 2, _size / 2].IsTaken = true;
    }

    private void ActivateRandomAbilities()
    {
        List<string> abilities = new();
        for (int i = 0; i < _randomApplied; i++)
        {
            var ability = GetRandomAbility(i == 0);
            abilities.Add(ability.ToString());
            Ability.Activate(ability, _grid);
        }

        abilities.Shuffle();
        Debug.Log(abilities.ToString("|"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) ActivateRandomAbilities();
    }

    public void Activate(AbilityType abilityType) => Ability.Activate(abilityType, _grid);

    private AbilityType GetRandomAbility(bool isFirst)
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
        public AbilityType Ability;
        public float Weight;
        public float WeightFirst;
    }
}
