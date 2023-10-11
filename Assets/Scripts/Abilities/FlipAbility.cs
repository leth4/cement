using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Flip")]
public class FlipAbility : Ability
{
    [SerializeField] private FlipType _flipType;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        FlipGrid(grid);
        return true;
    }

    public override bool ApplyRandom(Cell[,] grid)
    {
        FlipGrid(grid);
        return true;
    }

    private void FlipGrid(Cell[,] grid)
    {
        if (_flipType is FlipType.Horizontal)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size / 2; j++)
                    grid[i, j].SwapValuesWith(ref grid[i, Size - j - 1]);
        }
        if (_flipType is FlipType.Vertical)
        {
            for (int i = 0; i < Size / 2; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j].SwapValuesWith(ref grid[Size - i - 1, j]);
        }
    }

    private enum FlipType
    {
        Horizontal,
        Vertical
    }
}