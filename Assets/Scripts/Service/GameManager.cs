using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
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

    private void OnEnable()
    {
        GridManager.Solved += OnSolved;
    }

    private void OnDisable()
    {
        GridManager.Solved -= OnSolved;
    }
}
