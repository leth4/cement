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
    private int _cardsCount;

    public Ability ActiveAbility => _draggedCard?.GetComponent<AbilityCard>().Ability;
    public bool HasCardsLeft => Hand.Count > 0;
    public int CardsLeft => Hand.Count;

    private bool _isSorted = false;
    private List<Ability> _abilityOrder;

    public void SaveSortedOrder(List<Ability> order)
    {
        _abilityOrder = new(order);
    }

    public void SortCards()
    {
        if (!_isSorted) return;

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

    private void SortCardsFirst()
    {
        _isSorted = true;
        SortCards();
    }

    public void AddCard(Ability ability, bool delay = true)
    {
        StartCoroutine(CardAppearRoutine(ability, delay));
        _cardsCount++;
    }

    private IEnumerator CardAppearRoutine(Ability ability, bool delay)
    {
        if (delay) yield return new WaitForSeconds(_appearDelaySeconds * _cardsCount);
        var newCard = Instantiate(_cardPrefab, transform);
        newCard.transform.position -= Vector3.up * 3;
        newCard.Initialize(ability);
        Hand.Add(newCard.transform);
    }

    private void Update()
    {
        _cardAngleDelta = Mathf.Max(_minCardAngleDelta, _maxCardsAngle / Hand.Count);

        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1000, _cardLayer);

        if (!_draggedCard && hit.collider != null)
        {
            var newSelectedCardIndex = Hand.IndexOf(hit.transform);
            _selectedCardIndex = newSelectedCardIndex;
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
            Hand[_selectedCardIndex].localPosition = Vector3.Lerp(Hand[_selectedCardIndex].localPosition, IdealPositionByIndex(_selectedCardIndex).SetZ(-0.6f) + Vector3.up * _selectionHeight, Time.deltaTime * _translationSpeed);
            Hand[_selectedCardIndex].localRotation = Quaternion.Lerp(Hand[_selectedCardIndex].localRotation, Quaternion.identity, Time.deltaTime * _rotationSpeed);
            Hand[_selectedCardIndex].localScale = Vector3.Lerp(Hand[_selectedCardIndex].localScale, Vector3.one * _selectionScale, Time.deltaTime * _scaleSpeed);

            if (Input.GetMouseButtonDown(0))
            {
                HandleDragStart();
            }
        }

        if (!_draggedCard && hit.collider == null && !GameManager.Instance.IsSolved)
        {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) Recorder.Instance.GoBack(true);
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1))
        {
            HandleDragEnd();
        }

    }

    public void RemoveAllCards()
    {
        foreach (var card in Hand) card.GetComponent<AbilityCard>().MoveAway();
        Hand = new();
        _cardsCount = 0;
        _draggedCard = null;
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
        _cardsCount--;
        _draggedCard.GetComponent<AbilityCard>().MoveAway();
        _draggedCard = null;
    }

    private void HandleDragEnd()
    {
        if (Input.GetMouseButtonUp(0)) AbilityController.Instance.TryApplyingAbility();
        if (_draggedCard != null)
        {
            Hand.Insert(_draggedCardInitialIndex, _draggedCard);
            _draggedCard = null;
        }
        AbilityController.Instance.StopPreview();
    }

    private Vector3 IdealPositionByIndex(int index)
    {
        var angle = IdealRotationByIndex(index);
        float x = -_arcRadius * Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = _arcRadius * Mathf.Cos(Mathf.Deg2Rad * angle) - _arcRadius;

        return new(x, y, index * -0.01f);
    }

    private float IdealRotationByIndex(int index)
    {
        return (index - (float)Hand.Count / 2 + 0.5f) * _cardAngleDelta;
    }

    private void OnEnable()
    {
        GameManager.SortCards += SortCardsFirst;
    }

    private void OnDisable()
    {
        GameManager.SortCards -= SortCardsFirst;
    }
}
