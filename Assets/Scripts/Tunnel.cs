using UnityEngine;

public class Tunnel : MonoBehaviour
{
    public Transform destination;
    public Fade fadeScreen;
    public float fadeTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportRoutine(other));
        }
    }

    private System.Collections.IEnumerator TeleportRoutine(Collider player)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(fadeTime);

        player.transform.position = destination.position;

        fadeScreen.FadeIn();
    }
}
