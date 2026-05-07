using UnityEngine;
using System.Collections;

public class SpeedPad : MonoBehaviour
{
    public float freezeTime = 1f;
    public float launchForce = 25f;

    public float speedMultiplier = 3f;
    public float boostDuration = 2f;

    [Header("Camera FOV Boost")]
    public Camera boostCamera;
    public float fovBoost = 20f;
    public float fovReturnSpeed = 8f;

    private Coroutine fovRoutine;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm == null) return;

        StartCoroutine(SpeedBoost(rb, pm));
    }

    private IEnumerator SpeedBoost(Rigidbody rb, PlayerMovement pm)
    {
        pm.movementLocked = true;

        rb.linearVelocity *= 0.2f;
        Vector3 moveDir = rb.transform.forward;

        float originalSpeed = pm.moveSpeed;
        float originalFOV = boostCamera != null ? boostCamera.fieldOfView : 60f;

        if (boostCamera != null)
        {
            if (fovRoutine != null) StopCoroutine(fovRoutine);
            fovRoutine = StartCoroutine(FOVBoost(originalFOV));
        }

        yield return new WaitForSeconds(freezeTime);

        rb.linearVelocity = moveDir * launchForce;
        pm.moveSpeed = originalSpeed * speedMultiplier;

        pm.movementLocked = false;

        yield return new WaitForSeconds(boostDuration);

        StartCoroutine(RestoreSpeed(pm, originalSpeed));
        if (boostCamera != null)
        {
            if (fovRoutine != null) StopCoroutine(fovRoutine);
            fovRoutine = StartCoroutine(RestoreFOV(originalFOV));
        }
    }

    private IEnumerator FOVBoost(float originalFOV)
    {
        float targetFOV = originalFOV + fovBoost;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / freezeTime;
            boostCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, t);
            yield return null;
        }
    }

    private IEnumerator RestoreFOV(float originalFOV)
    {
        while (Mathf.Abs(boostCamera.fieldOfView - originalFOV) > 0.1f)
        {
            boostCamera.fieldOfView = Mathf.Lerp(
                boostCamera.fieldOfView,
                originalFOV,
                Time.deltaTime * fovReturnSpeed
            );
            yield return null;
        }

        boostCamera.fieldOfView = originalFOV;
    }

    private IEnumerator RestoreSpeed(PlayerMovement pm, float originalSpeed)
    {
        float t = 0f;
        float boostedSpeed = pm.moveSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            pm.moveSpeed = Mathf.Lerp(boostedSpeed, originalSpeed, t);
            yield return null;
        }

        pm.moveSpeed = originalSpeed;
    }
}
