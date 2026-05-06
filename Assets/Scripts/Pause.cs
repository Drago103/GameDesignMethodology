using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    private PlayerControls _controls;
    public GameObject pauseUI;
    private GameObject player;
    private Rigidbody rb;
    private bool isPaused = false;
    
    private MonoBehaviour[] playerScripts;
    private bool[] scriptsEnabled;

    private void Awake()
    {
        _controls= new PlayerControls();
    }

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.Pause.performed += OnPause;
    }

    void OnDisable()
    {
        _controls.Player.Disable();
        _controls.Player.Pause.canceled -= OnPause;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        if (isPaused) return;
        pauseUI.SetActive(true);
        isPaused = true;
        
        rb=player.GetComponent<Rigidbody>();
        
        DisableControl(player);
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }
        Time.timeScale = 0;
        
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        
        pauseUI.SetActive(false);

        EnableControl();
        if (rb != null)
        {
            rb.WakeUp();
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Time.timeScale = 1;
    }

    private void DisableControl(GameObject playerObj)
    {
        playerScripts = playerObj.GetComponents<MonoBehaviour>();
        scriptsEnabled = new bool[playerScripts.Length];

        for (int i = 0; i < playerScripts.Length; i++)
        {
            scriptsEnabled[i] = playerScripts[i].enabled;
            playerScripts[i].enabled = false;
        }
    }

    private void EnableControl()
    {
        if (playerScripts==null) return;
        for (int i = 0; i < playerScripts.Length; i++)
        {
            playerScripts[i].enabled = scriptsEnabled[i];
        }
    }

    public void OnResume()
    {
        ResumeGame();
    }
    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenu(string mainMenu)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenu);
    }
}
