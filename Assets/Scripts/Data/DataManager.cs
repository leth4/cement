using System.Collections.Generic;
using UnityEngine;

public static class DataManager
{
    public static GameData GameData;

    [RuntimeInitializeOnLoadMethod]
    private static void Load()
    {
        GameData = Data.Load<GameData>("GameData", new GameData());
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
    public bool ShownTutorial = false;
    public List<int> UnlockedCards = new() { 1, 2, 3, 4, 5, 6, 7 };
    public int Levels3Solved = 0;
    public int Levels4Solved = 0;
    public int Levels5Solved = 0;

    public int LevelsSolved => Levels3Solved + Levels4Solved + Levels5Solved;
}
