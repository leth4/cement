using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Flip")]
public class FlipAbility : Ability
{
    [SerializeField] private FlipType _flipType;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        FlipGrid(grid);
        return true;
    }

    public override void ApplyRandom(Cell[,] grid)
    {
        FlipGrid(grid);
    }

    private void FlipGrid(Cell[,] grid)
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                var tempNumber = grid[i, j];
            }
        }
    }


    private enum FlipType
    {
        Horizontal,
        Vertical
    }
}