using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Move")]
public class MoveAbiity : Ability
{
    // [SerializeField] private RotateType _rotateType;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        // RotateGrid(grid);
        return true;
    }

    public override bool ApplyRandom(Cell[,] grid)
    {
        // RotateGrid(grid);
        return true;
    }

    private void MoveGrid(Cell[,] grid, Vector2Int direction)
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {

            }
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
