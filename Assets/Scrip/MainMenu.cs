using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del menu")]
    public GameObject mainMenu;
    public GameObject optionsMenu;

    [Header("Escena a cargar al jugar")]
    public string gameSceneName = "Level1";

    // --- Navegacion entre paneles ---
    public void OpenOptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    // --- Botones principales ---
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");

#if UNITY_EDITOR
        // Application.Quit() no hace nada dentro del Editor,
        // asi que detenemos el modo Play para poder probarlo.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}