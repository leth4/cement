using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Image _overlay;
    [SerializeField] private float _timeInSeconds;
    [SerializeField] private Color _backgroundColor;

    private bool _isTransitioning;

    public void GoToMainScene() => GoToScene("Main");

    public void GoToMenuScene() => GoToScene("Menu");

    public void GoToTutorialScene() => GoToScene("Tutorial");

    private void GoToScene(string sceneName)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        StartCoroutine(TransitionRoutine(sceneName));
        AudioReceiver.SceneTransition();
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
