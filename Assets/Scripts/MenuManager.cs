using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject HUD_pause;

    public void StartGame()
    {
        LevelManager.instance.StartNewGame();
    }

    public void GotoMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void TogglePause()
    {
        HUD_pause.SetActive(!HUD_pause.activeInHierarchy);
        Time.timeScale = HUD_pause.activeInHierarchy ? 0.0f : 1.0f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
