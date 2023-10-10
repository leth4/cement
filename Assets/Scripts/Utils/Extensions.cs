using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    #region Vector3

    public static Vector3 SetX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    #endregion

    #region Vector2

    public static Vector2 SetX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector2 SetY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector3 ToVector3(this Vector2 v, float z = 0)
    {
        return new Vector3(v.x, v.y, z);
    }

    #endregion

    #region Transform

    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform) Object.Destroy(child.gameObject);
    }

    #endregion

    #region IEnumerable

    public static string ToString<T>(this IEnumerable<T> list, string separator)
    {
        string result = "";
        foreach (var obj in list)
        {
            if (result != "") result += separator;
            result += obj.ToString();
        }
        return result;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomItem<T>(this IEnumerable<T> list)
    {
        var random = new System.Random();
        int index = random.Next(0, list.Count());
        return list.ElementAt(index);
    }

    public static bool IsEmpty<T>(this IEnumerable<T> list)
    {
        return list.Count() == 0;
    }

    #endregion

    #region Numbers

    public static string Format(this double number)
    {
        if (number < 10000) return $"{(int)number}";
        return number.ToString("0.00E+0");
    }

    public static string Format(this float number)
    {
        if (number < 10000) return $"{(int)number}";
        return number.ToString("0.00E+0");
    }

    public static string Format(this int number)
    {
        if (number < 10000) return $"{number}";
        return number.ToString("0.00E+0");
    }

    public static float RoundTo(float number, float roundTo)
    {
        return Mathf.Round(number / roundTo) * roundTo;
    }

    #endregion

    #region Colors

    public static Color SetR(this Color color, float r)
    {
        color.r = r;
        return color;
    }

    public static Color SetG(this Color color, float g)
    {
        color.g = g;
        return color;
    }

    public static Color SetB(this Color color, float b)
    {
        color.b = b;
        return color;
    }

    public static Color SetA(this Color color, float a)
    {
        color.a = a;
        return color;
    }

    #endregion

}
