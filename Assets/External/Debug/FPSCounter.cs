using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float _fps;
    private float _framesSinceUpdate;
    private float _timeSinceUpdate;

    private const float UPDATE_RATE = 0.1f;

    private void Update()
    {
        _framesSinceUpdate++;
        _timeSinceUpdate += Time.unscaledDeltaTime;

        if (_timeSinceUpdate > UPDATE_RATE)
        {
            _fps = _framesSinceUpdate / _timeSinceUpdate;
            _timeSinceUpdate = 0;
            _framesSinceUpdate = 0;
        }
    }

    private void OnGUI()
    {
        var style = new GUIStyle("Box");
        style.fontSize = 25;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(Screen.width - 65, 10, 55, 35), $"{Mathf.Round(_fps)}", style);
    }
}
