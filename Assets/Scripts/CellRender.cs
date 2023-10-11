using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;

    public Cell Cell => _isPlayerCell ? GridManager.Instance.PlayerGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Cell PreviewCell => _isPlayerCell ? GridManager.Instance.PlayerPreviewGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Vector2Int Coordinates;

    private bool _isPlayerCell;

    public void Initialize(bool isPlayerCell, Vector2Int coords)
    {
        _isPlayerCell = isPlayerCell;
        Coordinates = coords;
    }

    public void ShowNumber()
    {
        _numberText.SetText($"{Cell.Number}");
    }

    public void Render()
    {
        StopAllCoroutines();

        if (AbilityController.Instance.IsPreviewing && _isPlayerCell)
        {
            if (PreviewCell.IsTaken) Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.black : Color.blue, 0.15f);
            else Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.red : Color.white, 0.15f);
            _numberText.SetText($"{PreviewCell.Number}");
        }
        else
        {
            Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.black : Color.white, 0.15f);
            if (_isPlayerCell) _numberText.SetText($"{Cell.Number}");
        }

        if (_isPlayerCell)
        {
            if (AbilityController.Instance.IsPreviewing)
            {
                _numberText.color = PreviewCell.IsTaken ? Color.white : Color.gray;
                if (PreviewCell.Number == 0) _numberText.color = Color.clear;
            }
            else
            {
                _numberText.color = Cell.IsTaken ? Color.white : Color.gray;
                if (Cell.Number == 0) _numberText.color = Color.clear;
            }
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

    private void OnEnable()
    {
        AbilityController.MadeChanges += Render;
    }

    private void OnDisable()
    {
        AbilityController.MadeChanges -= Render;
    }
}
