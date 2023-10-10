using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Recorder : Singleton<Recorder>
{
    private List<RecordStruct> _records = new();

    public void Reset()
    {
        _records = new();
    }

    public void Record(Ability ability)
    {
        _records.Add(new(GridManager.Instance.PlayerGrid, ability));
    }

    public void GoBack()
    {
        if (_records.Count == 0) return;

        var lastRecord = _records[^1];
        _records.RemoveAt(_records.Count - 1);

        AbilityController.Instance.AddAbility(lastRecord.Ability);

        for (int i = 0; i < GridManager.Instance.Size; i++)
        {
            for (int j = 0; j < GridManager.Instance.Size; j++)
            {
                GridManager.Instance.PlayerGrid[i, j].IsTaken = lastRecord.Grid[i, j];
            }
        }
    }

    internal void RemoveLastRecord()
    {
        _records.RemoveAt(_records.Count - 1);
    }

    private struct RecordStruct
    {
        public bool[,] Grid;
        public Ability Ability;

        public RecordStruct(Cell[,] grid, Ability ability)
        {
            Grid = new bool[GridManager.Instance.Size, GridManager.Instance.Size];
            for (int i = 0; i < GridManager.Instance.Size; i++)
            {
                for (int j = 0; j < GridManager.Instance.Size; j++)
                {
                    Grid[i, j] = GridManager.Instance.PlayerGrid[i, j].IsTaken;
                }
            }
            Ability = ability;
        }
    }
}
