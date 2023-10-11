using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;

    private bool _isPlayerCell;

    private bool _previewIsTaken;
    private int _previewNumber;

    public int Number { get; private set; }
    public bool IsTaken { get; private set; }

    public void SetNumber(int number)
    {
        if (AbilityController.Instance.IsPreviewing)
        {
            _previewNumber = number;
            _numberText.SetText($"{_previewNumber}");
        }
        else
        {
            Number = number;
            _numberText.SetText($"{Number}");
        }
    }

    public void SetTaken(bool taken, bool addInteraction = true)
    {
        if (AbilityController.Instance.IsPreviewing)
        {
            _previewIsTaken = taken;
        }
        else
        {
            IsTaken = taken;
        }
        if (addInteraction) AddInteraction();
        ApplyColor();
    }

    public void Initialize(bool isPlayerCell)
    {
        _isPlayerCell = isPlayerCell;
        Number = 0;
        _numberText.SetText($"{Number}");
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

    private void ApplyColor()
    {
        StopAllCoroutines();

        if (AbilityController.Instance.IsPreviewing)
        {
            if (_previewIsTaken) Tween.Color(this, _renderer, _renderer.color, IsTaken ? Color.black : Color.blue, 0.15f);
            if (!_previewIsTaken) Tween.Color(this, _renderer, _renderer.color, IsTaken ? Color.red : Color.white, 0.15f);
        }
        else
        {
            Tween.Color(this, _renderer, _renderer.color, IsTaken ? Color.black : Color.white, 0.15f);
        }

        if (_isPlayerCell)
        {
            _numberText.color = IsTaken ? Color.white : Color.gray;
            // if (Number == 0) _numberText.color = Color.clear;
        }
    }

    private void OnMouseEnter()
    {
        if (!_isPlayerCell) return;
        GridManager.Instance.SelectedCell = this;
        AbilityController.Instance.UpdatePreview();
    }

    private void OnMouseExit()
    {
        if (!_isPlayerCell) return;
        if (GridManager.Instance.SelectedCell == this)
        {
            GridManager.Instance.SelectedCell = null;
            AbilityController.Instance.StopPreview();
        }
    }

    private void OnPreviewStopped()
    {
        ApplyColor();
        _numberText.SetText($"{Number}");
    }

    private void OnEnable()
    {
        AbilityController.StoppedPreview += OnPreviewStopped;
    }

    private void OnDisable()
    {
        AbilityController.StoppedPreview -= OnPreviewStopped;
    }
}

