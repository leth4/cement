using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityCard : MonoBehaviour, IPointerClickHandler
{
    public static event Action<AbilityCard> Clicked;

    [SerializeField] private TMP_Text _nameText;

    public Ability Ability { get; private set; }

    public void Initialize(Ability ability)
    {
        Ability = ability;
        _nameText.SetText(ability.name);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }
}
