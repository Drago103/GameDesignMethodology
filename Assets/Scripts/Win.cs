using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] GameObject WinUi;
    [SerializeField] GameObject MenuManager;

    private bool isActivated = false;

    void OnTriggerEnter(Collider other)
    {
        if(isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            GameObject Player = other.gameObject;
            Player.GetComponent<Movement>().IsPaused = true;
            MenuManager.SetActive(false);
            WinUi.SetActive(true);
            return;
        }
    }

}
