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

    private void OnMouseOver()
    {
        if (_isPlayerCell) return;

        if (Input.GetMouseButtonDown(0)) IsTaken = true;
        if (Input.GetMouseButtonDown(1)) IsTaken = false;
    }
}
