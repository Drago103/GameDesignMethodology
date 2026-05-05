using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerGameOver : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Shield")]
    public RockShield rockShield;

    [Header("Player Scripts To Disable")]
    public MonoBehaviour[] scriptsToDisable;

    private bool gameOver = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameOverText != null)
            gameOverText.text = "GAME OVER !";

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (gameOver) return;

        RockHazard rock = collision.collider.GetComponentInParent<RockHazard>();

        if (rock != null)
        {
            HandleRockHit(collision.collider.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameOver) return;

        RockHazard rock = other.GetComponentInParent<RockHazard>();

        if (rock != null)
        {
            HandleRockHit(other.gameObject);
        }
    }

    void HandleRockHit(GameObject rockObject)
    {
        if (rockShield != null && rockShield.IsShieldActive())
        {
            Debug.Log("[SHIELD] Rock blocked by shield.");

            Rigidbody rockRb = rockObject.GetComponentInParent<Rigidbody>();
            if (rockRb != null)
            {
                rockRb.linearVelocity = Vector3.zero;
                rockRb.AddForce(Vector3.up * 8f, ForceMode.Impulse);
            }

            return;
        }

        TriggerGameOver();
    }

    void TriggerGameOver()
    {
        gameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverText != null)
            gameOverText.text = "GAME OVER !";

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        Debug.Log("[GAME OVER] Player hit a rock.");
    }

    public void RestartGame()
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