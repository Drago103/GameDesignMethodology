using UnityEngine;
using System.Collections;

public class SpeedPad : MonoBehaviour
{
    public float freezeTime = 1f;
    public float speedMultiplier = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        StartCoroutine(SpeedBoost(rb));
    }

    private IEnumerator SpeedBoost(Rigidbody rb)
    {
        Vector3 moveDir = rb.linearVelocity.normalized;
        if (moveDir == Vector3.zero)
            moveDir = rb.transform.forward;

        float originalSpeed = rb.linearVelocity.magnitude;

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(freezeTime);

        rb.isKinematic = false;
        rb.linearVelocity = moveDir * (originalSpeed * speedMultiplier);
    }
}
