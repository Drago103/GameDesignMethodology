using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level3EndTrigger : MonoBehaviour
{
    [Header("Level Complete UI")]
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText;

    [Header("Scene Settings")]
    public string nextLevelSceneName = "Level 4";
    public string mainMenuSceneName = "MainMenu";

    [Header("Player Control")]
    public MonoBehaviour[] scriptsToDisable;

    private bool levelCompleted = false;

    void Start()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (levelCompleted) return;

        if (other.CompareTag("Player"))
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        levelCompleted = true;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void OpenLevel4()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
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
        SceneManager.LoadScene(mainMenuSceneName);
    }
}