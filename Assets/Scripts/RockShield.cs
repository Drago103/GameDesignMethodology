using UnityEngine;
using UnityEngine.UI;

public class RockShield : MonoBehaviour
{
    [Header("Shield Object")]
    public GameObject shieldObject;

    [Header("Timing")]
    public float shieldDuration = 3f;
    public float cooldownDuration = 5f;

    [Header("UI")]
    public Slider cooldownBar;

    private bool shieldActive = false;
    private bool coolingDown = false;

    private float shieldTimer = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        // Shield starts turned off
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }

        // Bar starts full because shield is ready
        if (cooldownBar != null)
        {
            cooldownBar.minValue = 0f;
            cooldownBar.maxValue = 1f;
            cooldownBar.value = 1f;
            cooldownBar.interactable = false;
        }
    }

    void Update()
    {
        // Activate shield only if it is not active and not cooling down
        if (Input.GetKeyDown(KeyCode.T) && !shieldActive && !coolingDown)
        {
            ActivateShield();
        }

        // While shield is active, drain the bar
        if (shieldActive)
        {
            shieldTimer -= Time.deltaTime;

            float shieldProgress = shieldTimer / shieldDuration;

            if (cooldownBar != null)
            {
                cooldownBar.value = Mathf.Clamp01(shieldProgress);
            }

            if (shieldTimer <= 0f)
            {
                DeactivateShield();
            }
        }

        // After shield ends, refill the bar during cooldown
        else if (coolingDown)
        {
            cooldownTimer -= Time.deltaTime;

            float cooldownProgress = 1f - (cooldownTimer / cooldownDuration);

            if (cooldownBar != null)
            {
                cooldownBar.value = Mathf.Clamp01(cooldownProgress);
            }

            if (cooldownTimer <= 0f)
            {
                coolingDown = false;
                cooldownTimer = 0f;

                if (cooldownBar != null)
                {
                    cooldownBar.value = 1f;
                }
            }
        }
    }

    void ActivateShield()
    {
        shieldActive = true;
        coolingDown = false;

        shieldTimer = shieldDuration;

        if (shieldObject != null)
        {
            shieldObject.SetActive(true);
        }

        if (cooldownBar != null)
        {
            cooldownBar.value = 1f;
        }
    }

    void DeactivateShield()
    {
        shieldActive = false;
        coolingDown = true;

        cooldownTimer = cooldownDuration;

        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }

        if (cooldownBar != null)
        {
            cooldownBar.value = 0f;
        }
    }

    public bool IsShieldActive()
    {
        return shieldActive;
    }
}