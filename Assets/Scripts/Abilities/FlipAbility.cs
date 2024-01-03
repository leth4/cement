using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Flip")]
public class FlipAbility : Ability
{
    [SerializeField] private FlipType _flipType;

    public override bool Apply(bool[,] grid, Vector2Int coordinates)
    {
        FlipGrid(grid);
        return true;
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
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size / 2; j++)
                    (grid[i, j], grid[i, Size - j - 1]) = (grid[i, Size - j - 1], grid[i, j]);
        }
        if (_flipType is FlipType.Vertical)
        {
            for (int i = 0; i < Size / 2; i++)
                for (int j = 0; j < Size; j++)
                    (grid[i, j], grid[Size - i - 1, j]) = (grid[Size - i - 1, j], grid[i, j]);
        }
    }

    private enum FlipType
    {
        Horizontal,
        Vertical
    }
}