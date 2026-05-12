using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead1 : MonoBehaviour
{
    public GameObject gameover;
    private Rigidbody _rb;
    void OnTriggerEnter(Collider other) {
        Debug.Log("[TRIGGER ENTER] fuck Hit: " + other.tag);

        Debug.Log("[TRIGGER] GAMEOVER TAG FOUND: " + other.tag);
            _rb = GetComponent<Rigidbody>();
            DisablePlayerControl(other.gameObject);

            if (_rb != null) {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.Sleep();
            }

            Time.timeScale = 0;
            gameover.SetActive(true);
            
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
