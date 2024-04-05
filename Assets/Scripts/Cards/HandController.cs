using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : Singleton<HandController>
{
    [SerializeField] private AbilityCard _cardPrefab;
    [SerializeField] private float _appearDelaySeconds;

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
    public bool HasCardsLeft => Hand.Count > 0;
    public int CardsLeft => Hand.Count;

    private List<Ability> _abilityOrder;

    public void SaveSortedOrder(List<Ability> order)
    {
        _abilityOrder = new(order);
    }

    public void SortCards()
    {
        var newHand = new List<Transform>();
        foreach (var ability in _abilityOrder)
        {
            Transform foundCard = null;
            foreach (var card in Hand)
            {
                if (newHand.Contains(card)) continue;
                if (card.GetComponent<AbilityCard>().Ability == ability)
                {
                    foundCard = card;
                    break;
                }
            }
            if (foundCard != null) newHand.Add(foundCard);
        }

        Hand = newHand;
    }

    public void AddCard(Ability ability, int delayIndex = 0)
    {
        StartCoroutine(CardAppearRoutine(ability, _appearDelaySeconds * delayIndex));
    }

    private IEnumerator CardAppearRoutine(Ability ability, float delay)
    {
        yield return new WaitForSeconds(_appearDelaySeconds);
        var newCard = Instantiate(_cardPrefab, transform);
        newCard.transform.position -= Vector3.up * 3;
        newCard.Initialize(ability);
        Hand.Add(newCard.transform);
    }

    private void Update()
    {
        _cardAngleDelta = Mathf.Max(_minCardAngleDelta, _maxCardsAngle / Hand.Count);

        var raycastHit = Physics2D.Raycast(Helper.MainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, _cardLayer);

        if (_draggedCard == null && raycastHit.collider != null)
        {
            _selectedCardIndex = Hand.IndexOf(raycastHit.transform);
        }
        else
        {
            _selectedCardIndex = -1;
        }

        for (int i = 0; i < Hand.Count; i++)
        {
            if (i == _selectedCardIndex)
            {
                Hand[_selectedCardIndex].localScale = Vector3.Lerp(Hand[_selectedCardIndex].localScale, Vector3.one * _selectionScale, Time.deltaTime * _scaleSpeed);
                Hand[_selectedCardIndex].localPosition = Vector3.Lerp(Hand[_selectedCardIndex].localPosition, GetTargetPositionByIndex(_selectedCardIndex).SetZ(-0.6f) + Vector3.up * _selectionHeight, Time.deltaTime * _translationSpeed);
                Hand[_selectedCardIndex].localRotation = Quaternion.Lerp(Hand[_selectedCardIndex].localRotation, Quaternion.identity, Time.deltaTime * _rotationSpeed);
            }
            else
            {
                Hand[i].localScale = Vector3.Lerp(Hand[i].localScale, Vector3.one, Time.deltaTime * _scaleSpeed);
                Hand[i].localPosition = Vector3.Lerp(Hand[i].localPosition, GetTargetPositionByIndex(i), Time.deltaTime * _translationSpeed);
                Hand[i].localRotation = Quaternion.Lerp(Hand[i].localRotation, Quaternion.AngleAxis(GetTargetRotationByIndex(i), Vector3.forward), Time.deltaTime * _rotationSpeed);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleDragStart();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1))
        {
            HandleDragEnd();
        }

        if (_draggedCard == null && raycastHit.collider == null && !GameManager.Instance.IsLevelSolved)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) Recorder.Instance.GoBack(true);
        }
    }

    public void RemoveAllCards()
    {
        Hand.ForEach(card => card.GetComponent<AbilityCard>().MoveAway());
        Hand = new();
        _draggedCard = null;
    }

    private void HandleDragStart()
    {
        if (_selectedCardIndex == -1) return;
        _draggedCard = Hand[_selectedCardIndex];
        Hand.RemoveAt(_selectedCardIndex);
        _draggedCardInitialIndex = _selectedCardIndex;
        _previousDraggedPosition = _draggedCard.position;
    }

    public void DestroyDraggedCard()
    {
        _draggedCard.GetComponent<AbilityCard>().MoveAway();
        _draggedCard = null;
    }

    private void HandleDragEnd()
    {
        if (Input.GetMouseButtonUp(0)) AbilityController.Instance.ApplyAbility();

        if (_draggedCard != null)
        {
            Hand.Insert(_draggedCardInitialIndex, _draggedCard);
            _draggedCard = null;
        }

        AbilityController.Instance.StopPreview();
    }

    private Vector3 GetTargetPositionByIndex(int index)
    {
        var angle = GetTargetRotationByIndex(index);

        float x = -_arcRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = _arcRadius * Mathf.Cos(Mathf.Deg2Rad * angle) - _arcRadius;

        return new(x, y, index * -0.01f);
    }

    private float GetTargetRotationByIndex(int index)
    {
        return (index - (float)Hand.Count / 2 + 0.5f) * _cardAngleDelta;
    }
}
