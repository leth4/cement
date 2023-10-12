using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private Sprite _defaultSpriteBlack;
    [SerializeField] private Sprite _defaultSpriteWhite;
    [SerializeField] private Sprite _eraseSprite;
    [SerializeField] private Sprite _addSprite;

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
            if (PreviewCell.IsTaken) _renderer.sprite = Cell.IsTaken ? _defaultSpriteBlack : _addSprite;
            else _renderer.sprite = Cell.IsTaken ? _eraseSprite : _defaultSpriteWhite;
            _numberText.SetText($"{PreviewCell.Number}");
        }
        else
        {
            _renderer.sprite = Cell.IsTaken ? _defaultSpriteBlack : _defaultSpriteWhite;
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
