using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Button _menuButton;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) RestartScene();
        if (Input.touches.Length == 3) RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSolved()
    {
        RestartScene();
    }

    private void HandleMenuClick()
    {
        SceneManager.LoadScene("Menu");
    }

    private void OnEnable()
    {
        GridManager.Solved += OnSolved;
        _menuButton.onClick.AddListener(HandleMenuClick);
    }

    private void OnDisable()
    {
        GridManager.Solved -= OnSolved;
        _menuButton.onClick.RemoveListener(HandleMenuClick);
    }
}
