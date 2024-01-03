using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Rotate")]
public class RotateAbility : Ability
{
    [SerializeField] private RotateType _rotateType;

    public override bool Apply(bool[,] grid, Vector2Int coordinates)
    {
        RotateGrid(grid);
        return true;
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        RotateGrid(grid);
        return true;
    }

    private void RotateGrid(bool[,] grid)
    {
        for (int i = 0; i < Size / 2; i++)
        {
            for (int j = i; j < Size - i - 1; j++)
            {
                if (_rotateType is RotateType.Clockwise)
                {
                    bool isTakenTemp = grid[i, j];
                    grid[i, j] = grid[Size - 1 - j, i];
                    grid[Size - 1 - j, i] = grid[Size - 1 - i, Size - 1 - j];
                    grid[Size - 1 - i, Size - 1 - j] = grid[j, Size - 1 - i];
                    grid[j, Size - 1 - i] = isTakenTemp;
                }
                if (_rotateType is RotateType.Anticlockwise)
                {
                    bool isTakenTemp = grid[i, j];
                    grid[i, j] = grid[j, Size - 1 - i];
                    grid[j, Size - 1 - i] = grid[Size - 1 - i, Size - 1 - j];
                    grid[Size - 1 - i, Size - 1 - j] = grid[Size - 1 - j, i];
                    grid[Size - 1 - j, i] = isTakenTemp;
                }
            }
        }
    }

    private enum RotateType
    {
        Clockwise,
        Anticlockwise
    }
}