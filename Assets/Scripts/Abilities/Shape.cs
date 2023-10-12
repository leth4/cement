using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Shape")]
public class Shape : ScriptableObject
{
    public int Columns;
    public int Rows;
    public List<bool> GridList;
    public Vector2Int Offset = Vector2Int.zero;

    public bool[,] Grid
    {
        get
        {
            var grid = new bool[Columns, Rows];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    grid[j, Rows - i - 1] = GridList[i * Columns + j];
                }
            }
            return grid;
        }
    }
}

