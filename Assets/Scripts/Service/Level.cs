using System.Collections.Generic;

[System.Serializable]
public struct Level
{
    public bool[,] Grid;
    public List<Ability> Abilities;
}
