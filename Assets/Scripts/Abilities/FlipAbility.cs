using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Flip")]
public class FlipAbility : Ability
{
    [SerializeField] private FlipType _flipType;

    public override void Apply(bool[,] grid, Vector2Int coordinates)
    {
        FlipGrid(grid);
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        FlipGrid(grid);
        return true;
    }

    private void FlipGrid(bool[,] grid)
    {
        if (_flipType is FlipType.Horizontal)
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize / 2; j++)
                    (grid[i, j], grid[i, GridSize - j - 1]) = (grid[i, GridSize - j - 1], grid[i, j]);
        }
        if (_flipType is FlipType.Vertical)
        {
            for (int i = 0; i < GridSize / 2; i++)
                for (int j = 0; j < GridSize; j++)
                    (grid[i, j], grid[GridSize - i - 1, j]) = (grid[GridSize - i - 1, j], grid[i, j]);
        }
    }

    private enum FlipType
    {
        Horizontal,
        Vertical
    }
}