using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/ApplyShape")]
public class ApplyShapeAbility : Ability
{
    [SerializeField] private ApplyShapeType _applyShapeType;
    [SerializeField] public Shape Shape;

    public override void Apply(bool[,] grid, Vector2Int coordinates)
    {
        ApplyShape(grid, coordinates, Shape.Offset);
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        var takenCells = GetTakenCells(grid);

        for (int i = 0; i < 15; i++)
        {
            var cellIndex = Random.Range(0, takenCells.Count);
            var offset = new Vector2Int(Random.Range(-1, Shape.Grid.GetLength(0) + 1), Random.Range(-1, Shape.Grid.GetLength(1) + 1));
            var copy = grid.Clone() as bool[,];
            var hasChanged = ApplyShape(copy, takenCells[cellIndex], offset);
            if (hasChanged && GetTakenCells(copy, false).Count != 0)
            {
                ApplyShape(grid, takenCells[cellIndex], offset);
                return true;
            }
        }

        return false;
    }

    private bool ApplyShape(bool[,] grid, Vector2Int cell, Vector2Int offset)
    {
        var shapeSizeX = Shape.Grid.GetLength(0);
        var shapeSizeY = Shape.Grid.GetLength(1);

        var hasChanged = false;

        for (int i = 0; i < shapeSizeX; i++)
        {
            for (int j = 0; j < shapeSizeY; j++)
            {
                if (AreValidCoordinates(grid, cell.x + i - offset.x, cell.y + j - offset.y))
                {
                    if (Shape.Grid[i, j])
                    {
                        if (_applyShapeType is ApplyShapeType.Add)
                        {
                            if (!grid[cell.x + i - offset.x, cell.y + j - offset.y]) hasChanged = true;
                            grid[cell.x + i - offset.x, cell.y + j - offset.y] = true;
                        }
                        if (_applyShapeType is ApplyShapeType.Erase)
                        {
                            if (grid[cell.x + i - offset.x, cell.y + j - offset.y]) hasChanged = true;
                            grid[cell.x + i - offset.x, cell.y + j - offset.y] = false;
                        }
                        if (_applyShapeType is ApplyShapeType.Reverse)
                        {
                            hasChanged = true;
                            grid[cell.x + i - offset.x, cell.y + j - offset.y] = !grid[cell.x + i - offset.x, cell.y + j - offset.y];
                        }
                    }
                }
            }
        }

        return hasChanged;
    }

    private enum ApplyShapeType
    {
        Add,
        Erase,
        Reverse
    }
}