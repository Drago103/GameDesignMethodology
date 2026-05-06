using UnityEngine;

public class TutorialCheckPoint : MonoBehaviour
{
  [SerializeField] GameObject TutorialManager;
  [SerializeField] GameObject MenuManager;

  private bool isActivated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void OnTriggerEnter(Collider other)
    {
        if(isActivated) return;
        
        Debug.Log("[Trigger ENTER] Hit: " + other.GetComponent<Collider>().tag);

        if (other.GetComponent<Collider>().CompareTag("Player"))
        {
            isActivated = true;
            GameObject Player = other.gameObject;
            Player.GetComponent<Movement>().IsPaused = true;
            Pause.PauseLocked = true;
            //Cursor.visible = true;
            //MenuManager.SetActive(false);
            //Pause.PauseLocked = true;
            TutorialManager.SetActive(true);
            return;
        }
    }
}
