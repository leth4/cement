using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    private bool _isPlayerCell;

    private bool _isTaken;
    public bool IsTaken
    {
        get => _isTaken;
        set { _isTaken = value; ApplyColor(); }
    }

    public void Initialize(bool isPlayerCell)
    {
        _isPlayerCell = isPlayerCell;
    }

    private void ApplyColor()
    {
        StopAllCoroutines();
        Tween.Color(this, _renderer, _renderer.color, _isTaken ? Color.black : Color.white, 0.15f);
    }

    private void OnMouseEnter()
    {
        GridManager.Instance.SelectedCell = this;
    }

    private void OnMouseExit()
    {
        if (GridManager.Instance.SelectedCell == this) GridManager.Instance.SelectedCell = null;
    }
}

