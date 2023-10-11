using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;

    public Cell Cell => IsPlayerCell ? GridManager.Instance.PlayerGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Cell PreviewCell => IsPlayerCell ? GridManager.Instance.PlayerPreviewGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Vector2Int Coordinates;

    public bool IsPlayerCell { get; private set; }

    public void Initialize(bool isPlayerCell, Vector2Int coords)
    {
        IsPlayerCell = isPlayerCell;
        Coordinates = coords;
    }

    public void ShowNumber()
    {
        _numberText.SetText($"{Cell.Number}");
    }

    public void Render()
    {
        StopAllCoroutines();

        if (AbilityController.Instance.IsPreviewing && IsPlayerCell)
        {
            if (PreviewCell.IsTaken) Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.black : Color.blue, 0.05f);
            else Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.red : Color.white, 0.05f);
            _numberText.SetText($"{PreviewCell.Number}");
        }
        else
        {
            Tween.Color(this, _renderer, _renderer.color, Cell.IsTaken ? Color.black : Color.white, 0.05f);
            if (IsPlayerCell) _numberText.SetText($"{Cell.Number}");
        }

        if (IsPlayerCell)
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

    private void OnEnable()
    {
        AbilityController.MadeChanges += Render;
    }

    private void OnDisable()
    {
        AbilityController.MadeChanges -= Render;
    }
}
