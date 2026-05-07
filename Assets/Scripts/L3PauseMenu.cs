using UnityEngine;
using UnityEngine.SceneManagement;

public class L3PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseMenu script started");

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Pause Menu Panel is not assigned!");
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");

        isPaused = true;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Debug.Log("Game Resumed");

        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}