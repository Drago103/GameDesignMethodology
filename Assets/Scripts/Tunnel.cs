using UnityEngine;

public class Tunnel : MonoBehaviour
{
    public Transform destination;
    public Fade fadeScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportRoutine(other));
        }
    }

    private System.Collections.IEnumerator TeleportRoutine(Collider player)
    {
        yield return fadeScreen.FadeOut();

        player.transform.position = destination.position;

        yield return fadeScreen.FadeIn();
    }
}
