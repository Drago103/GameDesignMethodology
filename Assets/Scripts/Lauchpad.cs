using UnityEngine;
using System.Collections;

public class LaunchPad : MonoBehaviour
{
    public float upForce = 20f;
    public float forwardForce = 10f;
    public float stunTime = 1f;

    public Transform head;
    public float dipAmount = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        StartCoroutine(StunAndLaunch(rb, pm));
    }

    private IEnumerator StunAndLaunch(Rigidbody rb, PlayerMovement pm)
    {
        pm.movementLocked = true;
        rb.linearVelocity *= 0.2f;
        StartCoroutine(HeadDip());

        yield return new WaitForSeconds(stunTime);

        rb.linearVelocity = Vector3.up * upForce + transform.forward * forwardForce;

        pm.movementLocked = false;
    }

    private IEnumerator HeadDip()
    {
        float t = 0f;
        float duration = stunTime * 0.8f;

        Quaternion startRot = head.localRotation;
        Quaternion dippedRot = startRot * Quaternion.Euler(dipAmount, 0, 0);

        while (t < duration)
        {
            t += Time.deltaTime;
            head.localRotation = Quaternion.Lerp(startRot, dippedRot, t / duration);
            yield return null;
        }

        head.localRotation = startRot;
    }
}
