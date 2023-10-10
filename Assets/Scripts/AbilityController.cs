using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : Singleton<AbilityController>
{
    [SerializeField] private Transform _abilityCardContainer;
    [SerializeField] private AbilityCard _abilityCardPrefab;

    private AbilityCard _selectedAbilityCard;

    public void AddAbility(Ability ability)
    {
        var abilityCard = Instantiate(_abilityCardPrefab, _abilityCardContainer);
        abilityCard.Initialize(ability);
    }

    private void Update()
    {
        if (GridManager.Instance.SelectedCell != null && _selectedAbilityCard != null)
        {
            // Preview
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (GridManager.Instance.SelectedCell != null && _selectedAbilityCard != null)
            {
                var coords = (Vector2Int)GridHelper.GetCoordinates(GridManager.Instance.PlayerGrid, GridManager.Instance.SelectedCell);
                Recorder.Instance.Record(_selectedAbilityCard.Ability);
                var applied = _selectedAbilityCard.Ability.Apply(GridManager.Instance.PlayerGrid, coords);
                if (applied)
                {
                    Destroy(_selectedAbilityCard.gameObject);
                    _selectedAbilityCard = null;
                }
                else {
                    Recorder.Instance.RemoveLastRecord();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Recorder.Instance.GoBack();
        }
    }

    private void OnAbilityCardClicked(AbilityCard card)
    {
        _selectedAbilityCard = card;
    }

    private void OnEnable()
    {
        AbilityCard.Clicked += OnAbilityCardClicked;
    }

    private void OnDisable()
    {
        AbilityCard.Clicked -= OnAbilityCardClicked;
    }
}
