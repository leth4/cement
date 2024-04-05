using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Move")]
public class MoveAbility : Ability
{
    [SerializeField] private Direction _direction;

    public override void Apply(bool[,] grid, Vector2Int coordinates)
    {
        MoveGrid(grid);
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        MoveGrid(grid);
        return true;
    }

    private void MoveGrid(bool[,] grid)
    {
        if (_direction is Direction.Right)
        {
            for (int i = GridSize - 1; i >= 1; i--)
                for (int j = 0; j < GridSize; j++)
                    grid[i, j] = grid[i - 1, j];

            for (int j = 0; j < GridSize; j++)
                grid[0, j] = false;
        }
        if (_direction is Direction.Left)
        {
            for (int i = 0; i < GridSize - 1; i++)
                for (int j = 0; j < GridSize; j++)
                    grid[i, j] = grid[i + 1, j];

            for (int j = 0; j < GridSize; j++)
                grid[GridSize - 1, j] = false;
        }
        if (_direction is Direction.Up)
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = GridSize - 1; j >= 1; j--)
                    grid[i, j] = grid[i, j - 1];

            for (int i = 0; i < GridSize; i++)
                grid[i, 0] = false;
        }

        if (_direction is Direction.Down)
        {
            for (int i = 0; i < GridSize; i++)
                for (int j = 0; j < GridSize - 1; j++)
                    grid[i, j] = grid[i, j + 1];

            for (int i = 0; i < GridSize; i++)
                grid[i, GridSize - 1] = false;
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
