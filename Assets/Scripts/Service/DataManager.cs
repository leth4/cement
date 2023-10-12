using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class DataManager
{
    public static GameData GameData;

    [RuntimeInitializeOnLoadMethod]
    private static void Load()
    {
        GameData = Data.Load<GameData>("GameData");
    }

    public static void Save()
    {
        Data.Save(GameData, "GameData");
    }
}

[System.Serializable]
public class GameData
{
    public float MusicVolume = 1;
    public float SoundVolume = 1;
    public bool ShownTutorial = true;
}
