using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite Image;
    public bool CanBeFirst = true;
    public int Weight = 3;
    public bool IsFullCanvas;

    protected int Size => GridManager.Instance.Size;

    public abstract bool Apply(bool[,] grid, Vector2Int coordinates);
    public abstract bool ApplyRandom(bool[,] grid);

    protected List<Vector2Int> GetTakenCells(bool[,] grid, bool shuffled = true)
    {
        var takenCells = new List<Vector2Int>();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j]) takenCells.Add(new(i, j));

        if (shuffled) takenCells.Shuffle();

        return takenCells;
    }

    protected bool AreValidCoordinates(bool[,] grid, int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }
}