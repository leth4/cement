using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/ApplyShape")]
public class ApplyShapeAbility : Ability
{
    [SerializeField] private ApplyShapeType _applyShapeType;
    [SerializeField] private Shape _shape;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        ApplyShape(grid, coordinates, _shape.Offset);
        return true;
    }

    public override bool ApplyRandom(Cell[,] grid)
    {
        var takenCells = GetTakenCells(grid);

        for (int i = 0; i < 10; i++)
        {
            var cellIndex = Random.Range(0, takenCells.Count);
            var offset = new Vector2Int(Random.Range(-1, _shape.Grid.GetLength(0) + 1), Random.Range(-1, _shape.Grid.GetLength(1) + 1));
            var copy = grid.Clone() as Cell[,];
            ApplyShape(copy, takenCells[cellIndex], offset);
            if (GetTakenCells(copy, false).Count != 0)
            {
                ApplyShape(grid, takenCells[cellIndex], offset);
                return true;
            }
        }

        return false;
    }

    private void ApplyShape(Cell[,] grid, Vector2Int cell, Vector2Int offset)
    {
        var shapeSizeX = _shape.Grid.GetLength(0);
        var shapeSizeY = _shape.Grid.GetLength(1);

        for (int i = 0; i < shapeSizeX; i++)
        {
            for (int j = 0; j < shapeSizeY; j++)
            {
                if (GridHelper.AreValidCoordinates(grid, cell.x + i - offset.x, cell.y + j - offset.y))
                {
                    if (_shape.Grid[i, j])
                    {
                        if (_applyShapeType is ApplyShapeType.Add) grid[cell.x + i - offset.x, cell.y + j - offset.y].SetTaken(true);
                        if (_applyShapeType is ApplyShapeType.Erase) grid[cell.x + i - offset.x, cell.y + j - offset.y].SetTaken(false);
                        if (_applyShapeType is ApplyShapeType.Reverse) grid[cell.x + i - offset.x, cell.y + j - offset.y].SetTaken(!grid[cell.x + i - offset.x, cell.y + j - offset.y].IsTaken);
                    }
                }
            }
        }
    }

    private enum ApplyShapeType
    {
        Add,
        Erase,
        Reverse
    }
}