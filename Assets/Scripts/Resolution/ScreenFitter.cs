using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFitter : MonoBehaviour
{
    private const float LANDSCAPE_MODE_RATIO = 0.6f;

    [SerializeField] private ScreenMode _orientation;

    [Header("Scale")]
    [SerializeField] private float _AspectToScaleRatio = 1;
    [SerializeField] private float _maxScale = 1;

    [Header("Anchor")]
    [SerializeField] private AnchorType _anchor;
    [SerializeField] private Vector2 _anchorOffset;
    [SerializeField] private bool _scaleOffset = true;

    private Camera _camera;

    private bool IsPortraitMode => Screen.width / (float)Screen.height < LANDSCAPE_MODE_RATIO;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_orientation != ScreenMode.Any && !(_orientation == ScreenMode.Portrait && IsPortraitMode) && !(_orientation == ScreenMode.Landscape && !IsPortraitMode)) return;

        var newScale = Vector3.one * Screen.width / Screen.height * _AspectToScaleRatio;
        transform.localScale = Vector3.Min(newScale, Vector3.one * _maxScale);

        if (_anchor != AnchorType.None) transform.position = GetAnchoredPosition();
    }

    private Vector3 GetAnchoredPosition()
    {
        var height = 2f * _camera.orthographicSize;
        var width = height * _camera.aspect;

        var cameraPos = _camera.transform.position;

        var leftX = cameraPos.x - width / 2;
        var rightX = cameraPos.x + width / 2;
        var topY = cameraPos.y + height / 2;
        var bottomY = cameraPos.y - height / 2;

        var anchoredPosition = _anchor switch
        {
            AnchorType.None => Vector3.zero,
            AnchorType.BottomLeft => new Vector3(leftX, bottomY, 0),
            AnchorType.BottomCenter => new Vector3(cameraPos.x, bottomY, 0),
            AnchorType.BottomRight => new Vector3(rightX, bottomY, 0),
            AnchorType.MiddleLeft => new Vector3(leftX, cameraPos.y, 0),
            AnchorType.MiddleCenter => new Vector3(cameraPos.x, cameraPos.y, 0),
            AnchorType.MiddleRight => new Vector3(rightX, cameraPos.y, 0),
            AnchorType.TopLeft => new Vector3(leftX, topY, 0),
            AnchorType.TopCenter => new Vector3(cameraPos.x, topY, 0),
            AnchorType.TopRight => new Vector3(rightX, topY, 0),
            _ => Vector3.zero
        };

        var scaledOffset = _scaleOffset ? _anchorOffset * transform.localScale.x : _anchorOffset;

        return anchoredPosition + scaledOffset.ToVector3();
    }

    private enum AnchorType
    {
        None,
        BottomLeft,
        BottomCenter,
        BottomRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        TopLeft,
        TopCenter,
        TopRight
    }

    private enum ScreenMode
    {
        Any,
        Landscape,
        Portrait
    }
}
