using UnityEngine;
using UnityEngine.SceneManagement;

public class gameover : MonoBehaviour
{
    public string loadscene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(loadscene);
        }
    }
}
