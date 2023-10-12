using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private Color _darkNumberColor;
    [SerializeField] private Color _lightNumberColor;
    [SerializeField] private Sprite _defaultSpriteBlack;
    [SerializeField] private Sprite _defaultSpriteWhite;
    [SerializeField] private Sprite _eraseSprite;
    [SerializeField] private Sprite _addSprite;

    public Cell Cell => IsPlayerCell ? GridManager.Instance.PlayerGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Cell PreviewCell => IsPlayerCell ? GridManager.Instance.PlayerPreviewGrid[Coordinates.x, Coordinates.y] : GridManager.Instance.AnswerGrid[Coordinates.x, Coordinates.y];
    public Vector2Int Coordinates;

    public bool IsPlayerCell { get; private set; }

    private bool _isPhantom = false;

    private float _appearDelay = 0;

    public void Initialize(bool isPlayerCell, Vector2Int coords, float appearDelay)
    {
        IsPlayerCell = isPlayerCell;
        Coordinates = coords;
        _appearDelay = appearDelay;
    }

    public void MakePhantom()
    {
        _isPhantom = true;
        _renderer.color = Color.clear;
    }

    public void Start()
    {
        transform.localScale = Vector3.zero;
        Tween.Delay(this, _appearDelay / 20, () => Tween.Scale(this, transform, Vector3.zero, Vector3.one, 0.4f, EaseType.SineOut));
    }

    public void ShowNumber()
    {
        _numberText.SetText($"{Cell.Number}");
    }

    public void Hide(float delay) => Tween.Delay(this, delay / 70, () => Tween.Scale(this, transform, Vector3.one, Vector3.zero, 0.4f, EaseType.SineOut));

    public void Render()
    {
        if (_isPhantom) return;

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
                _numberText.color = PreviewCell.IsTaken ? _lightNumberColor : _darkNumberColor;
                if (PreviewCell.Number == 0) _numberText.color = Color.clear;
            }
            else
            {
                _numberText.color = Cell.IsTaken ? _lightNumberColor : _darkNumberColor;
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
