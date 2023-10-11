using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : Singleton<AbilityController>
{
    public static event Action MadeChanges;

    [SerializeField] private Transform _abilityCardContainer;
    [SerializeField] private AbilityCard _abilityCardPrefab;

    public bool IsPreviewing = false;

    private AbilityCard _selectedAbilityCard;

    public void AddAbility(Ability ability)
    {
        var abilityCard = Instantiate(_abilityCardPrefab, _abilityCardContainer);
        abilityCard.Initialize(ability);
    }

    public void UpdatePreview()
    {
        GridManager.Instance.PlayerPreviewGrid = GridManager.Instance.PlayerGrid.Clone() as Cell[,];
        IsPreviewing = true;
        if (GridManager.Instance.SelectedCell != null && _selectedAbilityCard != null)
        {
            _selectedAbilityCard.Ability.Apply(GridManager.Instance.PlayerPreviewGrid, GridManager.Instance.SelectedCell.Coordinates);
        }
        MadeChanges?.Invoke();
    }

    public void CallChange() => MadeChanges?.Invoke();

    public void StopPreview()
    {
        IsPreviewing = false;
        MadeChanges?.Invoke();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GridManager.Instance.SelectedCell != null && _selectedAbilityCard != null)
            {
                StopPreview();

                Recorder.Instance.Record(_selectedAbilityCard.Ability);
                var applied = _selectedAbilityCard.Ability.Apply(GridManager.Instance.PlayerGrid, GridManager.Instance.SelectedCell.Coordinates);
                if (applied)
                {
                    Destroy(_selectedAbilityCard.gameObject);
                    _selectedAbilityCard = null;
                    MadeChanges?.Invoke();
                }
                else
                {
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
