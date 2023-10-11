using System;
using System.IO;
using UnityEngine;

public static class Data
{
    private static bool _encrypted = true;
    private static string _key = "8d5c37ac-69bf-4593-998e-706e6eda0650";

    private static string DirectoryPath => $"{Application.persistentDataPath}{Path.AltDirectorySeparatorChar}saves";
    private static string GetPath(string name) => $"{DirectoryPath}{Path.AltDirectorySeparatorChar}{name}.save";

    public static void Save<T>(T data, string name)
    {
        var path = GetPath(name);
        if (File.Exists(path)) File.Delete(path);

        Directory.CreateDirectory(DirectoryPath);

        try
        {
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, DataToJson(data));

        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message}{e.StackTrace}");
        }
    }

    public static T Load<T>(string name, T defaultValue = default)
    {
        var path = GetPath(name);
        if (!File.Exists(path)) return defaultValue;

        try
        {
            T data = JsonToData<T>(File.ReadAllText(path));
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to load data: {e.Message}{e.StackTrace}");
            return defaultValue;
        }
    }

    private static string DataToJson<T>(T data)
    {
        var json = JsonUtility.ToJson(data);

        if (!_encrypted) return json;

        var encryptedJson = "";

        for (int i = 0; i < json.Length; i++)
        {
            encryptedJson += (char)(json[i] + _key[i % _key.Length]);
        }

        return encryptedJson;
    }

    private static T JsonToData<T>(string json)
    {
        if (!_encrypted) return JsonUtility.FromJson<T>(json);

        var decryptedJson = "";

        for (int i = 0; i < json.Length; i++)
        {
            decryptedJson += (char)(json[i] - _key[i % _key.Length]);
        }

        return JsonUtility.FromJson<T>(decryptedJson);
    }
}

[System.Serializable]
public struct FloatData
{
    public float Data;
    public FloatData(float data) => Data = data;
}
