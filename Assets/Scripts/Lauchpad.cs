using UnityEngine;
using System.Collections;

public class LaunchPad : MonoBehaviour
{
    public float upForce = 20f;
    public float forwardForce = 10f;
    public float freezeTime = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        StartCoroutine(Launch(rb));
    }

    private IEnumerator Launch(Rigidbody rb)
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(freezeTime);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.up * upForce + transform.forward * forwardForce;
    }
}
