using System;
using System.Collections;
using System.Collections.Generic;

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

        HandController.Instance.AddCard(lastRecord.Ability, false);
        HandController.Instance.SortCards();
        GridManager.Instance.PlayerGrid = lastRecord.Grid.Clone() as Cell[,];

        AbilityController.Instance.StopPreview();
    }

    internal void RemoveLastRecord()
    {
        _records.RemoveAt(_records.Count - 1);
    }

    private struct RecordStruct
    {
        public Cell[,] Grid;
        public Ability Ability;

        public RecordStruct(Cell[,] grid, Ability ability)
        {
            Grid = GridManager.Instance.PlayerGrid.Clone() as Cell[,];
            Ability = ability;
        }
    }
}
