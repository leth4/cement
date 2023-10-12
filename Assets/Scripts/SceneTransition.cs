using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image _overlay;
    [SerializeField] private float _timeInSeconds;
    [SerializeField] private Color _backgroundColor;

    private bool _isTransitioning;

    public void GoToMainScene()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutine("Main"));
    }

    public void GoToMenuScene()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutine("Menu"));
    }

    private IEnumerator TransitionRoutine(string scene)
    {
        _overlay.gameObject.SetActive(true);
        for (float t = 0; t < _timeInSeconds; t += Time.deltaTime)
        {
            _overlay.color = Color.Lerp(_backgroundColor.SetA(0), _backgroundColor, t / _timeInSeconds);
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }
}
