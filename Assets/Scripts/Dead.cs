using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public GameObject gameover;
    private Rigidbody _rb;
    

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            _rb = other.GetComponent<Rigidbody>();
            DisablePlayerControl(other.gameObject);
            
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.Sleep();
            }
            
            Time.timeScale = 0;
            gameover.SetActive(true);
        }

    }
    
    private void DisablePlayerControl(GameObject player)
    {
        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
        {
            s.enabled = false;
        }
       
    }
}
