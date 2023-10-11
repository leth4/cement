using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Start()
    {

    }

    private void StartPlaying()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(StartPlaying);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(StartPlaying);
    }
}
