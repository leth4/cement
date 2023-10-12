using UnityEngine;

public class Screenshake : Singleton<Screenshake>
{
    [SerializeField] private Transform _cameraTransform;

    private float _shakeMagnitude;
    private Vector3 _initialPosition;
    private float _shakeDurationLeft;
    private bool _useRealTime;

    private void Start()
    {
        _initialPosition = _cameraTransform.position;
    }

    private void Update()
    {
        if (_shakeDurationLeft > 0)
        {
            transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
            _shakeDurationLeft -= _useRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }
        else
        {
            _shakeDurationLeft = 0f;
            transform.localPosition = _initialPosition;
        }
    }

    public void Shake(float duration, float magnitude = 0.1f, bool realTime = false)
    {
        _useRealTime = realTime;
        _shakeDurationLeft = duration;
        _shakeMagnitude = magnitude;
    }
}