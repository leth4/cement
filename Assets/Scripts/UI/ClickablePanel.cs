using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickablePanel : MonoBehaviour, IPointerClickHandler
{
    public event Action OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
