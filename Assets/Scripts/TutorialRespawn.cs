using UnityEngine;

public class TutorialRespawn : MonoBehaviour
{
    [SerializeField] Vector3 RespawnPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject Player = other.gameObject;
            foreach(MonoBehaviour script in Player.GetComponents<MonoBehaviour>())
            {
                script.enabled = false;
            }
            Player.transform.position = RespawnPos;

            foreach(MonoBehaviour script in Player.GetComponents<MonoBehaviour>())
            {
                script.enabled = true;
            }

            return;
        }
    } 
}
