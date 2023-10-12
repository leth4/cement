using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Move")]
public class MoveAbility : Ability
{
    [SerializeField] private Direction _direction;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        MoveGrid(grid);
        return true;
    }

    public override bool ApplyRandom(Cell[,] grid)
    {
        MoveGrid(grid);
        return true;
    }

    private void MoveGrid(Cell[,] grid)
    {
        if (_direction is Direction.Right)
        {
            for (int i = Size - 1; i >= 1; i--)
                for (int j = 0; j < Size; j++)
                    grid[i, j].CopyValuesFrom(grid[i - 1, j]);
            for (int j = 0; j < Size; j++)
            {
                grid[0, j].SetTaken(false);
                grid[0, j].SetNumber(0);
            }
        }
        if (_direction is Direction.Left)
        {
            for (int i = 0; i < Size - 1; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j].CopyValuesFrom(grid[i + 1, j]);
            for (int j = 0; j < Size; j++)
            {
                grid[Size - 1, j].SetTaken(false);
                grid[Size - 1, j].SetNumber(0);
            }
        }
        if (_direction is Direction.Up)
        {
            for (int i = 0; i < Size; i++)
                for (int j = Size - 1; j >= 1; j--)
                    grid[i, j].CopyValuesFrom(grid[i, j - 1]);
            for (int i = 0; i < Size; i++)
            {
                grid[i, 0].SetTaken(false);
                grid[i, 0].SetNumber(0);
            }
        }
        if (_direction is Direction.Down)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size - 1; j++)
                    grid[i, j].CopyValuesFrom(grid[i, j + 1]);
            for (int i = 0; i < Size; i++)
            {
                grid[i, Size - 1].SetTaken(false);
                grid[i, Size - 1].SetNumber(0);
            }
        }
    }

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
