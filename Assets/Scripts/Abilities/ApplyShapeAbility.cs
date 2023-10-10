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
        ApplyShape(grid, coordinates, Vector2Int.zero);
        return true;
    }

    public override void ApplyRandom(Cell[,] grid)
    {
        var takenCells = GetTakenCells(grid);

        var offset = new Vector2Int(Random.Range(0, _shape.Grid.GetLength(0)), Random.Range(0, _shape.Grid.GetLength(1)));

        ApplyShape(grid, takenCells[0], offset);
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