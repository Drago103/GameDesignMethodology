    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using System.Collections;

    public class Pause : MonoBehaviour
    {
        private PlayerControls _controls;
        public GameObject pauseUI;
        [SerializeField] GameObject player;
        private Rigidbody rb;
        private bool isPaused = false;
        private bool canToggle = true;
        public static bool PauseLocked = true;
        
        private MonoBehaviour[] playerScripts;

        private void Awake()
        {
            _controls= new PlayerControls();
        }

        void OnEnable()
        {
            _controls.Player.Enable();
            _controls.Player.Pause.performed += OnPause;
        }

        void OnDisable()
        {
            _controls.Player.Disable();
            _controls.Player.Pause.performed -= OnPause;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (PauseLocked) return;
            if (!context.performed || !canToggle) return;
            StartCoroutine(PauseCooldown());
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
            player.GetComponent<Movement>().IsPaused = true;
            pauseUI.SetActive(true);
            isPaused = true;
            
            rb=player.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            Time.timeScale = 0;
            
        
            //Cursor.visible = true;
        }

        private void ResumeGame()
        {
            if (!isPaused) return;
            isPaused = false;
            player.GetComponent<Movement>().IsPaused = false;
            
            pauseUI.SetActive(false);

            if (rb != null)
            {
                rb.WakeUp();
            }

            //Cursor.visible = false;
            
            Time.timeScale = 1;
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

        IEnumerator PauseCooldown()
        {
            canToggle = false;
            yield return new WaitForSecondsRealtime(0.15f);
            canToggle = true;
        }
    }
