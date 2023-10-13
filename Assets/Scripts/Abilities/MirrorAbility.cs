using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Abilities/Mirror")]
public class MirrorAbility : Ability
{
    [SerializeField] private MirrorType _mirrorType;

    public override bool Apply(Cell[,] grid, Vector2Int coordinates)
    {
        MirrorGrid(grid);
        return true;
    }

    public override bool ApplyRandom(Cell[,] grid)
    {
        MirrorGrid(grid);
        return true;
    }

    private void MirrorGrid(Cell[,] grid)
    {
        var newlyTaken = new List<Vector2Int>();
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (_mirrorType is MirrorType.Horizontal)
                {
                    if (grid[i, j].IsTaken && j != Size - j - 1)
                    {
                        grid[i, Size - j - 1].SetTaken(true, addInteraction: grid[i, j].IsTaken && !newlyTaken.Contains(new(i, j)));
                        newlyTaken.Add(new(i, Size - j - 1));
                    }
                }
                if (_mirrorType is MirrorType.Vertical)
                {
                    if (grid[i, j].IsTaken && i != Size - i - 1)
                    {
                        grid[Size - i - 1, j].SetTaken(true, addInteraction: grid[i, j].IsTaken && !newlyTaken.Contains(new(i, j)));
                        newlyTaken.Add(new(Size - i - 1, j));
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