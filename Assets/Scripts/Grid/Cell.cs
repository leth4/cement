using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
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

    public void SwapValuesWith(ref Cell cell)
    {
        var numberTemp = Number;
        var isTakenTemp = IsTaken;

        CopyValuesFrom(cell);

        cell.SetNumber(numberTemp);
        cell.SetTaken(isTakenTemp, addInteraction: isTakenTemp);
    }

    public void AddInteraction()
    {
        SetNumber(Number + 1);
    }
}

