using UnityEngine;

public class TutorialCheckPoint : MonoBehaviour
{
  [SerializeField] GameObject TutorialManager;

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
            foreach(MonoBehaviour script in Player.GetComponents<MonoBehaviour>())
            {
                script.enabled = false;
            }
            TutorialManager.SetActive(true);
            return;
        }
    }
}
