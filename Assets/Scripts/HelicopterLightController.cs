using UnityEngine;

public class HelicopterLightController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Height Settings")]
    public float height = 8f;

    [Header("Light")]
    public Light spotlight;
    public float lookOffset = 1.5f;
    public float baseIntensity = 60f;

    [Header("Flicker Settings")]
    public float flickerAmount = 0.5f;   // how strong the flicker is
    public float flickerSpeed = 2f;      // how fast it changes

    void LateUpdate()
    {
        if (player == null || spotlight == null) return;

        // Position light above player
        Vector3 lightPosition = player.position + Vector3.up * height;
        transform.position = lightPosition;

        // Aim at player
        Vector3 lookTarget = player.position + Vector3.up * lookOffset;

        spotlight.transform.position = transform.position;
        spotlight.transform.LookAt(lookTarget);

        // ✅ Smooth flicker using Perlin Noise (no jitter)
        float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerAmount;

        spotlight.intensity = baseIntensity + flicker;
    }
}