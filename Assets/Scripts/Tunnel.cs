using UnityEngine;
using System.Collections;

public class Tunnel : MonoBehaviour
{
    public Transform destination;
    public Fade fadeScreen;
    public float fadeTime = 1f;
    public float freezeTime = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TeleportRoutine(other));
        }
    }

    private IEnumerator TeleportRoutine(Collider player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();

        // Fade out
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeTime);

        // Freeze player
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }

        // Teleport
        player.transform.position = destination.position;

        // Face negative Z
        player.transform.rotation = Quaternion.LookRotation(Vector3.back);

        // Stay frozen for a moment
        yield return new WaitForSeconds(freezeTime);

        // Fade in
        fadeScreen.FadeIn();

        // Wake rigidbody
        if (rb != null)
            rb.WakeUp();
    }
}
