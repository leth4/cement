using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;

    private bool _isPlayerCell;

    private int _number;
    public int Number
    {
        get => _number;
        set
        {
            _number = value;
            _numberText.SetText($"{Number}");
        }
    }

    private bool _previewIsTaken;

    private bool _isTaken;
    public bool IsTaken
    {
        get => _isTaken;
        set
        {
            if (AbilityController.Instance.IsPreviewing)
            {
                _previewIsTaken = value;
            }
            else
            {
                _isTaken = value;
                AddInteraction();
            }
            ApplyColor();
        }
    }

    public void Initialize(bool isPlayerCell)
    {
        _isPlayerCell = isPlayerCell;
        Number = 0;
        _numberText.SetText($"{Number}");
    }

    public void CopyValuesFrom(Cell cell)
    {
        Number = cell.Number;
        IsTaken = cell.IsTaken;
        AddInteraction();
    }

    public void AddInteraction()
    {
        Number++;
    }

    private void ApplyColor()
    {
        StopAllCoroutines();

        if (AbilityController.Instance.IsPreviewing)
        {
            if (_previewIsTaken) Tween.Color(this, _renderer, _renderer.color, _isTaken ? Color.black : Color.blue, 0.15f);
            if (!_previewIsTaken) Tween.Color(this, _renderer, _renderer.color, _isTaken ? Color.red : Color.white, 0.15f);
        }
        else
        {
            Tween.Color(this, _renderer, _renderer.color, _isTaken ? Color.black : Color.white, 0.15f);
        }

        if (_isPlayerCell) _numberText.color = _isTaken ? Color.white : Color.gray;
    }

    private void OnMouseEnter()
    {
        GridManager.Instance.SelectedCell = this;
        AbilityController.Instance.UpdatePreview();
    }

    private void OnMouseExit()
    {
        if (GridManager.Instance.SelectedCell == this)
        {
            GridManager.Instance.SelectedCell = null;
            AbilityController.Instance.StopPreview();
        }
    }

    private void OnEnable()
    {
        AbilityController.StoppedPreview += ApplyColor;
    }

    private void OnDisable()
    {
        AbilityController.StoppedPreview -= ApplyColor;
    }
}

