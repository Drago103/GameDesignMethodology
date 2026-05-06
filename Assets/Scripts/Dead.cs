using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public GameObject gameover;
    private GameObject player;
    private Rigidbody _rb;

    private MonoBehaviour[] playerScripts;
    private bool[] scriptsEnabled;
    
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
        
            _rb=player.GetComponent<Rigidbody>();
        
            DisablePlayerControl(player);
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.Sleep();
            }
            Time.timeScale = 0;
        
        
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    private void DisablePlayerControl(GameObject playerObj)
    {
        playerScripts = playerObj.GetComponents<MonoBehaviour>();
        scriptsEnabled = new bool[playerScripts.Length];

        for (int i = 0; i < playerScripts.Length; i++)
        {
            scriptsEnabled[i] = playerScripts[i].enabled;
            playerScripts[i].enabled = false;
        }
    }
}
