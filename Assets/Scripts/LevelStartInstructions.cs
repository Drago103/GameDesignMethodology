using UnityEngine;

public class LevelStartInstructions : MonoBehaviour
{
    [Header("Start Instructions UI")]
    public GameObject startInstructionsPanel;

    [Header("Player Control")]
    public MonoBehaviour[] scriptsToDisableAtStart;

    private bool instructionsOpen = true;

    void Start()
    {
        ShowInstructions();
    }

    void Update()
    {
        if (instructionsOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ShowInstructions()
    {
        instructionsOpen = true;

        if (startInstructionsPanel != null)
            startInstructionsPanel.SetActive(true);

        foreach (MonoBehaviour script in scriptsToDisableAtStart)
        {
            if (script != null)
                script.enabled = false;
        }

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartLevel()
    {
        instructionsOpen = false;

        if (startInstructionsPanel != null)
            startInstructionsPanel.SetActive(false);

        foreach (MonoBehaviour script in scriptsToDisableAtStart)
        {
            if (script != null)
                script.enabled = true;
        }

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}