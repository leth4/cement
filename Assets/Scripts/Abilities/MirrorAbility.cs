using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Mirror")]
public class MirrorAbility : Ability
{
    [SerializeField] private MirrorType _mirrorType;

    public override void Apply(bool[,] grid, Vector2Int coordinates)
    {
        MirrorGrid(grid);
    }

    public override bool ApplyRandom(bool[,] grid)
    {
        MirrorGrid(grid);
        return true;
    }

    private void MirrorGrid(bool[,] grid)
    {
        var newlyTaken = new List<Vector2Int>();
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (_mirrorType is MirrorType.Horizontal)
                {
                    if (grid[i, j] && j != GridSize - j - 1)
                    {
                        grid[i, GridSize - j - 1] = true;
                        newlyTaken.Add(new(i, GridSize - j - 1));
                    }
                }
                if (_mirrorType is MirrorType.Vertical)
                {
                    if (grid[i, j] && i != GridSize - i - 1)
                    {
                        grid[GridSize - i - 1, j] = true;
                        newlyTaken.Add(new(GridSize - i - 1, j));
                    }
                }
            }
        }
    }


    private enum MirrorType
    {
        Horizontal,
        Vertical
    }
}