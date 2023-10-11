using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tilt
// Handle when cursor at the edge so it jumps back and forth

public class HandController : Singleton<HandController>
{
    [SerializeField] private AbilityCard _cardPrefab;

    [Header("Positioning")]
    [SerializeField] private float _maxCardsAngle = 30;
    [SerializeField] private float _minCardAngleDelta = 3.5f;
    [SerializeField] private float _arcRadius = 140;

    [Header("Interactivity")]
    [SerializeField] private float _selectionScale = 1.3f;
    [SerializeField] private float _selectionHeight = 5f;

    [Header("Movement")]
    [SerializeField] private float _translationSpeed = 8;
    [SerializeField] private float _rotationSpeed = 10;
    [SerializeField] private float _scaleSpeed = 10;

    [Header("Settings")]
    [SerializeField] private LayerMask _cardLayer;

    private int _selectedCardIndex = -1;
    private List<Transform> Hand = new();
    private Transform _draggedCard;
    private int _draggedCardInitialIndex;
    private Vector3 _previousDraggedPosition;

    private float _cardAngleDelta;

    public Ability ActiveAbility => _draggedCard?.GetComponent<AbilityCard>().Ability;

    public void AddCard(Ability ability)
    {
        var newCard = Instantiate(_cardPrefab, transform);
        newCard.Initialize(ability);
        Hand.Add(newCard.transform);
    }

    private void Update()
    {
        _cardAngleDelta = Mathf.Min(_minCardAngleDelta, _maxCardsAngle / Hand.Count);

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, _cardLayer);

        if (!_draggedCard && hit.collider != null)
        {
            _selectedCardIndex = Hand.IndexOf(hit.transform.parent.parent);
        }
        else
        {
            _selectedCardIndex = -1;
        }

        for (int i = 0; i < Hand.Count; i++)
        {
            if (i == _selectedCardIndex) continue;
            Hand[i].localScale = Vector3.Lerp(Hand[i].localScale, Vector3.one, Time.deltaTime * _scaleSpeed);
            Hand[i].localPosition = Vector3.Lerp(Hand[i].localPosition, IdealPositionByIndex(i), Time.deltaTime * _translationSpeed);
            Hand[i].localRotation = Quaternion.Lerp(Hand[i].localRotation, Quaternion.AngleAxis(IdealRotationByIndex(i), Vector3.forward), Time.deltaTime * _rotationSpeed);
        }

        if (_selectedCardIndex != -1)
        {
            Hand[_selectedCardIndex].localPosition = Vector3.Lerp(Hand[_selectedCardIndex].localPosition, IdealPositionByIndex(_selectedCardIndex) + Vector3.up * _selectionHeight, Time.deltaTime * _translationSpeed);
            Hand[_selectedCardIndex].localRotation = Quaternion.Lerp(Hand[_selectedCardIndex].localRotation, Quaternion.identity, Time.deltaTime * _rotationSpeed);
            Hand[_selectedCardIndex].localScale = Vector3.Lerp(Hand[_selectedCardIndex].localScale, new Vector3(_selectionScale, _selectionScale, _selectionScale), Time.deltaTime * _scaleSpeed);

            if (Input.GetMouseButtonDown(0))
            {
                HandleDragStart();
            }
        }

        if (_draggedCard != null) HandleDrag();

        if (Input.GetMouseButtonUp(0))
        {
            HandleDragEnd();
        }

    }

    private void HandleDrag()
    {
        //
    }

    private void HandleDragStart()
    {
        _draggedCard = Hand[_selectedCardIndex];
        Hand.RemoveAt(_selectedCardIndex);
        _draggedCardInitialIndex = _selectedCardIndex;
        _previousDraggedPosition = _draggedCard.position;
    }

    public void DestroyDraggedCard()
    {
        Hand.Remove(_draggedCard);
        _draggedCard.GetComponent<AbilityCard>().MoveAway();
        _draggedCard = null;
    }

    private void HandleDragEnd()
    {
        AbilityController.Instance.TryApplyingAbility();
        if (_draggedCard != null)
        {
            Hand.Insert(_draggedCardInitialIndex, _draggedCard);
            _draggedCard = null;
        }
    }

    private Vector3 IdealPositionByIndex(int index)
    {
        var angle = IdealRotationByIndex(index);
        float x = -_arcRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = _arcRadius * Mathf.Cos(Mathf.Deg2Rad * angle) - _arcRadius;

        return new(x, y, index * 0.01f);
    }

    private float IdealRotationByIndex(int index)
    {
        return (index - (float)Hand.Count / 2 + 0.5f) * _cardAngleDelta;
    }
}