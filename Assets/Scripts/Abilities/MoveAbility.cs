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
                    grid[i, j] = grid[i - 1, j];
        }
        if (_direction is Direction.Left)
        {
            for (int i = 0; i < Size - 1; i++)
                for (int j = 0; j < Size; j++)
                    grid[i, j] = grid[i + 1, j];
        }
        if (_direction is Direction.Up)
        {
            for (int i = 0; i < Size; i++)
                for (int j = Size - 1; j >= 1; j--)
                    grid[i, j] = grid[i, j - 1];
        }
        if (_direction is Direction.Down)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size - 1; j++)
                    grid[i, j] = grid[i, j + 1];
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
