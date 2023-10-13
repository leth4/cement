using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellRender : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
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

    public void Hide(float delay) => Tween.Delay(this, delay / 70, () => Tween.Scale(this, transform, Vector3.one, Vector3.zero, 0.4f, EaseType.SineOut));

    public void Render()
    {
        if (_isPhantom) return;

        if (AbilityController.Instance.IsPreviewing && IsPlayerCell)
        {
            if (PreviewCell.IsTaken) _renderer.sprite = Cell.IsTaken ? _defaultSpriteBlack : _addSprite;
            else _renderer.sprite = Cell.IsTaken ? _eraseSprite : _defaultSpriteWhite;

            if (PreviewCell.IsErasing) _renderer.sprite = _eraseSprite;
        }
        else
        {
            _renderer.sprite = Cell.IsTaken ? _defaultSpriteBlack : _defaultSpriteWhite;
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
