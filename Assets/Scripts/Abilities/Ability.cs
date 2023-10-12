using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite Image;
    public bool CanBeFirst = true;
    public int Weight = 3;

    protected int Size => GridManager.Instance.Size;

    public abstract bool Apply(Cell[,] grid, Vector2Int coordinates);
    public abstract bool ApplyRandom(Cell[,] grid);

    protected List<Vector2Int> GetTakenCells(Cell[,] grid, bool shuffled = true)
    {
        var takenCells = new List<Vector2Int>();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j].IsTaken) takenCells.Add(new(i, j));

        if (shuffled) takenCells.Shuffle();

        return takenCells;
    }
}