using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ability
{
    private static int Size => GridManager.Size;

    public static void Activate(AbilityType type, Cell[,] grid)
    {
        if (type is AbilityType.MirrorH) MirrorHorizontally(grid);
        if (type is AbilityType.MirrorV) MirrorVertically(grid);
        if (type is AbilityType.RotateClockwise) RotateClockwise(grid);
        if (type is AbilityType.RotateAnticlockwise) RotateAnticlockwise(grid);
        if (type is AbilityType.AddShapePoint) AddShape(grid, new bool[,] { { true } });
        if (type is AbilityType.AddShapeL) AddShape(grid, new bool[,] { { true, true, true }, { true, false, false } });
        if (type is AbilityType.AddShapeSquare) AddShape(grid, new bool[,] { { true, true }, { true, true } });
        if (type is AbilityType.EraseShapePoint) EraseShape(grid, new bool[,] { { true } });
        if (type is AbilityType.EraseShapeL) EraseShape(grid, new bool[,] { { true, true, true }, { true, false, false } });
        if (type is AbilityType.EraseShapeSquare) EraseShape(grid, new bool[,] { { true, true }, { true, true } });
        if (type is AbilityType.ReverseShapePoint) ReverseShape(grid, new bool[,] { { true } });
        if (type is AbilityType.ReverseShapeL) ReverseShape(grid, new bool[,] { { true, true, true }, { true, false, false } });
        if (type is AbilityType.ReverseShapeSquare) ReverseShape(grid, new bool[,] { { true, true }, { true, true } });
    }

    private static void AddShape(Cell[,] grid, bool[,] shape)
    {
        var takenCells = new List<Vector2Int>();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j].IsTaken) takenCells.Add(new(i, j));

        takenCells.Shuffle();

        var cell = takenCells[0];

        var shapeSizeX = shape.GetLength(0);
        var shapeSizeY = shape.GetLength(1);

        var offsetX = Random.Range(0, shapeSizeX);
        var offsetY = Random.Range(0, shapeSizeX);

        for (int i = 0; i < shapeSizeX; i++)
        {
            for (int j = 0; j < shapeSizeY; j++)
            {
                if (GridHelper.AreValidCoordinates(grid, cell.x + i - offsetX, cell.y + j - offsetY))
                {
                    if (shape[i, j]) grid[cell.x + i - offsetX, cell.y + j - offsetY].IsTaken = true;
                }
            }
        }
    }

    private static void EraseShape(Cell[,] grid, bool[,] shape)
    {
        var takenCells = GetTakenCells(grid);

        var cell = takenCells[0];

        var shapeSizeX = shape.GetLength(0);
        var shapeSizeY = shape.GetLength(1);

        var offsetX = Random.Range(0, shapeSizeX);
        var offsetY = Random.Range(0, shapeSizeX);

        for (int i = 0; i < shapeSizeX; i++)
        {
            for (int j = 0; j < shapeSizeY; j++)
            {
                if (GridHelper.AreValidCoordinates(grid, cell.x + i - offsetX, cell.y + j - offsetY))
                {
                    if (shape[i, j]) grid[cell.x + i - offsetX, cell.y + j - offsetY].IsTaken = false;
                }
            }
        }
    }

    private static void ReverseShape(Cell[,] grid, bool[,] shape)
    {
        var takenCells = GetTakenCells(grid);

        var cell = takenCells[0];

        var shapeSizeX = shape.GetLength(0);
        var shapeSizeY = shape.GetLength(1);

        var offsetX = Random.Range(0, shapeSizeX);
        var offsetY = Random.Range(0, shapeSizeX);

        for (int i = 0; i < shapeSizeX; i++)
        {
            for (int j = 0; j < shapeSizeY; j++)
            {
                if (GridHelper.AreValidCoordinates(grid, cell.x + i - offsetX, cell.y + j - offsetY))
                {
                    if (shape[i, j]) grid[cell.x + i - offsetX, cell.y + j - offsetY].IsTaken = !grid[cell.x + i - offsetX, cell.y + j - offsetY].IsTaken;
                }
            }
        }
    }

    private static void RotateClockwise(Cell[,] grid)
    {
        for (int i = 0; i < Size / 2; i++)
        {
            for (int j = i; j < Size - i - 1; j++)
            {
                bool isTaken = grid[i, j].IsTaken;
                grid[i, j].IsTaken = grid[Size - 1 - j, i].IsTaken;
                grid[Size - 1 - j, i].IsTaken = grid[Size - 1 - i, Size - 1 - j].IsTaken;
                grid[Size - 1 - i, Size - 1 - j].IsTaken = grid[j, Size - 1 - i].IsTaken;
                grid[j, Size - 1 - i].IsTaken = isTaken;
            }
        }
    }

    private static void RotateAnticlockwise(Cell[,] grid)
    {
        for (int i = 0; i < Size / 2; i++)
        {
            for (int j = i; j < Size - i - 1; j++)
            {
                bool isTakenTemp = grid[i, j].IsTaken;
                grid[i, j].IsTaken = grid[j, Size - 1 - i].IsTaken;
                grid[j, Size - 1 - i].IsTaken = grid[Size - 1 - i, Size - 1 - j].IsTaken;
                grid[Size - 1 - i, Size - 1 - j].IsTaken = grid[Size - 1 - j, i].IsTaken;
                grid[Size - 1 - j, i].IsTaken = isTakenTemp;
            }
        }
    }

    private static void MirrorHorizontally(Cell[,] grid)
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j].IsTaken) grid[i, Size - j - 1].IsTaken = true;
    }

    private static void MirrorVertically(Cell[,] grid)
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j].IsTaken) grid[Size - i - 1, j].IsTaken = true;
    }

    private static void AddRandomSquare(Cell[,] grid)
    {
        var takenCells = GetTakenCells(grid);

        foreach (var cell in takenCells)
        {
            var adjacentCells = GridHelper.GetAdjacent(grid, cell.x, cell.y, false);
            adjacentCells.Shuffle();

            bool found = false;

            foreach (var adjacent in adjacentCells)
            {
                if (!adjacent.IsTaken)
                {
                    adjacent.IsTaken = true;
                    found = true;
                    break;
                }
            }

            if (found) break;
        }
    }

    private static List<Vector2Int> GetTakenCells(Cell[,] grid, bool shuffled = true)
    {
        var takenCells = new List<Vector2Int>();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                if (grid[i, j].IsTaken) takenCells.Add(new(i, j));

        if (shuffled) takenCells.Shuffle();

        return takenCells;
    }
}

public enum AbilityType
{
    MirrorH,
    MirrorV,
    RotateClockwise,
    RotateAnticlockwise,
    AddShapePoint,
    AddShapeL,
    AddShapeSquare,
    EraseShapePoint,
    EraseShapeL,
    EraseShapeSquare,
    ReverseShapePoint,
    ReverseShapeL,
    ReverseShapeSquare
}
