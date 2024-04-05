using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite Image;
    public bool CanBeFirst = true;
    public bool IsFullCanvas;
    public int Weight = 3;

    protected int GridSize => GridManager.Instance.Size;

    public abstract void Apply(bool[,] grid, Vector2Int coordinates);
    public abstract bool ApplyRandom(bool[,] grid);

    protected List<Vector2Int> GetTakenCells(bool[,] grid, bool shuffled = true)
    {
        var takenCells = new List<Vector2Int>();

        for (int i = 0; i < GridSize; i++)
            for (int j = 0; j < GridSize; j++)
                if (grid[i, j]) takenCells.Add(new(i, j));

        if (shuffled) takenCells.Shuffle();

        return takenCells;
    }

    protected bool AreValidCoordinates(bool[,] grid, int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }
}