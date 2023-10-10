using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }

    public static T RandomWeighted<T>(List<T> list, List<float> weights)
    {
        float weightSum = 0;
        foreach (var weight in weights) weightSum += weight;

        float randomWeight = Random.Range(0, weightSum);

        float currentWeight = 0;

        for (int i = 0; i < list.Count; i++)
        {
            currentWeight += weights[i];
            if (randomWeight <= currentWeight)
                return list[i];
        }

        return default;
    }
}
