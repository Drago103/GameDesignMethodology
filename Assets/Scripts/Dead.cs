using UnityEngine;
using UnityEngine.SceneManagement;

public class Dead : MonoBehaviour
{
    public GameObject gameoverCanvas;
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0;
            gameoverCanvas.SetActive(true);
        }
        if (other.CompareTag("Gameover"))
        {
            Time.timeScale = 0;
            gameoverCanvas.SetActive(true);
        }
    }
}
