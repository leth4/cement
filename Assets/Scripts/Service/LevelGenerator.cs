using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private int _minTakenCells = 5;
    [Range(0, 1)][SerializeField] private float _repeatTypeChance = .25f;
    [Range(0, 1)][SerializeField] private float _repeatShapeChance = .33f;

    private int _gridSize => GridManager.Instance.Size;

    public Level GenerateRandomLevel(int abilityCount)
    {
        var abilities = new List<Ability>();
        var grid = new bool[_gridSize, _gridSize];

        while (true)
        {
            grid = new bool[_gridSize, _gridSize];
            grid[_gridSize / 2, _gridSize / 2] = true;

            abilities = new List<Ability>();
            var types = new List<Type>();
            var shapes = new List<Shape>();

            for (int i = 0; i < abilityCount; i++)
            {
                var ability = GetRandomAbility(i == 0);

                if (abilities.Contains(ability))
                {
                    i--;
                    continue;
                }

                if (types.Contains(ability.GetType()) && UnityEngine.Random.Range(0f, 1f) > _repeatTypeChance)
                {
                    i--;
                    continue;
                }

                if (ability is ApplyShapeAbility && shapes.Contains(((ApplyShapeAbility)ability).Shape) && UnityEngine.Random.Range(0f, 1f) > _repeatShapeChance)
                {
                    i--;
                    continue;
                }

                var applied = ability.ApplyRandom(grid);
                if (!applied)
                {
                    i--;
                    continue;
                }

                if (ability is ApplyShapeAbility) shapes.Add(((ApplyShapeAbility)ability).Shape);
                types.Add(ability.GetType());
                abilities.Add(ability);
            }

            var takenCells = 0;
            foreach (var cell in grid)
            {
                if (cell) takenCells++;
            }
            if (takenCells >= _minTakenCells) break;
        }

        return new Level() { Abilities = abilities, Grid = grid };
    }

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
}