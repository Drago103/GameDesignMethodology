using UnityEngine;

public class TeleportTunnel : MonoBehaviour
{
    public Transform destination;        // Where the player will appear
    public FadeScreen fadeScreen;        // Reference to fade controller

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportRoutine(other));
        }
    }

    private System.Collections.IEnumerator TeleportRoutine(Collider player)
    {
        // Fade to black
        yield return fadeScreen.FadeOut();

        // Teleport player
        player.transform.position = destination.position;

        // Fade back in
        yield return fadeScreen.FadeIn();
    }
}
