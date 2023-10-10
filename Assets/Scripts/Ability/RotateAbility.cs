using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Rotate")]
public class RotateAbility : Ability
{
    [SerializeField] private RotateType _rotateType;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        RotateGrid(grid);
        return true;
    }

    public override void ApplyRandom(Cell[,] grid)
    {
        RotateGrid(grid);
    }

    private void RotateGrid(Cell[,] grid)
    {
        for (int i = 0; i < Size / 2; i++)
        {
            for (int j = i; j < Size - i - 1; j++)
            {
                if (_rotateType is RotateType.Clockwise)
                {
                    bool isTaken = grid[i, j].IsTaken;
                    grid[i, j].IsTaken = grid[Size - 1 - j, i].IsTaken;
                    grid[Size - 1 - j, i].IsTaken = grid[Size - 1 - i, Size - 1 - j].IsTaken;
                    grid[Size - 1 - i, Size - 1 - j].IsTaken = grid[j, Size - 1 - i].IsTaken;
                    grid[j, Size - 1 - i].IsTaken = isTaken;
                }
                if (_rotateType is RotateType.Anticlockwise)
                {
                    bool isTakenTemp = grid[i, j].IsTaken;
                    grid[i, j].IsTaken = grid[j, Size - 1 - i].IsTaken;
                    grid[j, Size - 1 - i].IsTaken = grid[Size - 1 - i, Size - 1 - j].IsTaken;
                    grid[Size - 1 - i, Size - 1 - j].IsTaken = grid[Size - 1 - j, i].IsTaken;
                    grid[Size - 1 - j, i].IsTaken = isTakenTemp;
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