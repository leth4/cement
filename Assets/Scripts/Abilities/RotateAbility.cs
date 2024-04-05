using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Rotate")]
public class RotateAbility : Ability
{
    [SerializeField] private RotateType _rotateType;

    public override void Apply(bool[,] grid, Vector2Int coordinates)
    {
        RotateGrid(grid);
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        RotateGrid(grid);
        return true;
    }

    private void RotateGrid(bool[,] grid)
    {
        for (int i = 0; i < GridSize / 2; i++)
        {
            for (int j = i; j < GridSize - i - 1; j++)
            {
                if (_rotateType is RotateType.Clockwise)
                {
                    bool isTakenTemp = grid[i, j];
                    grid[i, j] = grid[GridSize - 1 - j, i];
                    grid[GridSize - 1 - j, i] = grid[GridSize - 1 - i, GridSize - 1 - j];
                    grid[GridSize - 1 - i, GridSize - 1 - j] = grid[j, GridSize - 1 - i];
                    grid[j, GridSize - 1 - i] = isTakenTemp;
                }
                if (_rotateType is RotateType.Anticlockwise)
                {
                    bool isTakenTemp = grid[i, j];
                    grid[i, j] = grid[j, GridSize - 1 - i];
                    grid[j, GridSize - 1 - i] = grid[GridSize - 1 - i, GridSize - 1 - j];
                    grid[GridSize - 1 - i, GridSize - 1 - j] = grid[GridSize - 1 - j, i];
                    grid[GridSize - 1 - j, i] = isTakenTemp;
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