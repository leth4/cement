using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    private bool _previewIsTaken;
    private int _previewNumber;

    public int Number { get; private set; }
    public bool IsTaken { get; private set; }

    public void SetNumber(int number)
    {
        Number = number;
    }

    public void SetTaken(bool taken, bool addInteraction = true)
    {
        IsTaken = taken;
        if (addInteraction) AddInteraction();
    }

    public void CopyValuesFrom(Cell cell)
    {
        SetTaken(cell.IsTaken, false);
        SetNumber(cell.Number);

        if (IsTaken) AddInteraction();
    }

    public void SwapValuesWith(Cell cell)
    {
        var numberTemp = Number;
        var isTakenTemp = IsTaken;

        CopyValuesFrom(cell);

        cell.Number = numberTemp;
        cell.IsTaken = isTakenTemp;

        if (cell.IsTaken) cell.AddInteraction();
    }

    public void AddInteraction()
    {
        SetNumber(Number + 1);
    }
}

