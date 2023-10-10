using System.Collections.Generic;
using UnityEngine;

public static class GridHelper
{
    public static List<T> GetAdjacent<T>(T[,] matrix, T item, bool diagonal)
    {
        var coords = GetCoordinates(matrix, item);
        if (coords == null) return null;
        return GetAdjacent(matrix, coords.Value.x, coords.Value.y, diagonal);
    }

    public static List<T> GetAdjacent<T>(T[,] matrix, int xCoord, int yCoord, bool diagonal)
    {
        var results = new List<T>();

        for (int x = xCoord - 1; x <= xCoord + 1; x++)
        {
            for (int y = yCoord - 1; y <= yCoord + 1; y++)
            {
                if (x == xCoord && y == yCoord) continue;
                if (!AreValidCoordinates(matrix, x, y)) continue;
                if (!diagonal && x != xCoord && y != yCoord) continue;
                results.Add(matrix[x, y]);
            }
        }

        return results;
    }

    public static List<Vector2Int> GetAdjacentCoordinates<T>(T[,] matrix, int xCoord, int yCoord, bool diagonal)
    {
        var results = new List<Vector2Int>();

        for (int x = xCoord - 1; x <= xCoord + 1; x++)
        {
            for (int y = yCoord - 1; y <= yCoord + 1; y++)
            {
                if (x == xCoord && y == yCoord) continue;
                if (!AreValidCoordinates(matrix, x, y)) continue;
                if (!diagonal && x != xCoord && y != yCoord) continue;
                results.Add(new(x, y));
            }
        }

        return results;
    }

    public static Vector2Int? GetCoordinates<T>(T[,] matrix, T item)
    {
        for (int x = 0; x < matrix.GetLength(0); x++)
            for (int y = 0; y < matrix.GetLength(1); y++)
                if (matrix[x, y].Equals(item)) return new Vector2Int(x, y);

        return null;
    }

    public static bool AreValidCoordinates<T>(T[,] matrix, int x, int y)
    {
        return x >= 0 && x < matrix.GetLength(0) && y >= 0 && y < matrix.GetLength(1);
    }

    public static bool IsValidCoordinates<T>(T[,] matrix, Vector2Int coords)
    {
        return AreValidCoordinates(matrix, coords.x, coords.y);
    }

    public static Vector3 GetIsometricWorldPosition(int x, int y, float tileSize = 1, Vector3 offset = default(Vector3))
    {
        var positionX = (x + y) * tileSize / 2f;
        var positionY = (x - y) * tileSize / 4f;
        var positionZ = x - y;

        return new Vector3(positionX, positionY, positionZ) + offset;
    }

    public static Vector2Int GetIsometricCoordinatesByWorldPosition(Vector2 position, float tileSize = 1, Vector2 offset = default(Vector2))
    {
        position += offset;

        var X = (int)((position.x + position.y * 2) / tileSize);
        var Y = (int)((-position.y * 2 + position.x) / tileSize);

        return new Vector2Int(X, Y);
    }
}
