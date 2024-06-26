using UnityEngine;

public class ShakeUI : MonoBehaviour
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
