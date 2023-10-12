using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _magnitude;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition = _initialPosition + Random.insideUnitSphere * _magnitude;
    }
}
