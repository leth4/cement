using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _doubleTapThreshold = 0.3f;

    private float _timeSinceLastTap = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) GameManager.Instance.HandleMenuClick();
        if (Input.GetKeyDown(KeyCode.R)) GameManager.Instance.ResetLevel(true);

        if (Input.GetMouseButtonDown(1) && HandController.Instance.ActiveAbility == null && !GameManager.Instance.IsLevelSolved)
        {
            Recorder.Instance.GoBack(true);
        }

        _timeSinceLastTap += Time.unscaledDeltaTime;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (_timeSinceLastTap < _doubleTapThreshold) GameManager.Instance.ResetLevel(true);
            _timeSinceLastTap = 0;
        }
    }
}
