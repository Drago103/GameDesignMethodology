using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public GameObject gameoverCanvas;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            DisablePlayerControl(other.gameObject);
            Time.timeScale = 0;
            gameoverCanvas.SetActive(true);
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
