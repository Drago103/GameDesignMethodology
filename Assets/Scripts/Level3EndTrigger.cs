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

    [Header("Player Scripts To Disable")]
    public MonoBehaviour[] scriptsToDisable;

    private bool levelCompleted = false;

    void Start()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (levelCompleteText != null)
            levelCompleteText.text = "LEVEL COMPLETED";
    }

    void OnTriggerEnter(Collider other)
    {
        if (levelCompleted) return;

        if (other.CompareTag("Player"))
        {
            CompleteLevel(other.gameObject);
        }
    }

    void CompleteLevel(GameObject player)
    {
        levelCompleted = true;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (levelCompleteText != null)
            levelCompleteText.text = "LEVEL COMPLETED";

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        Debug.Log("[LEVEL COMPLETE] Player reached the level complete prefab.");
    }

    public void GoToNextLevel()
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