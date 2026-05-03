using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    //private WallRebound wall;

    [SerializeField] Movement movement;

    [SerializeField] float dashVelocity = 20f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    [SerializeField] Rigidbody rb;
    [SerializeField] Transform headTarget;

    private bool isDashing = false;

    public bool IsDashing => isDashing;

    private float lastDashTime = -Mathf.Infinity;
    

    void Awake()
    {
        if (movement == null)
            movement = GetComponent<Movement>();
    }

    public void CheckDashInput(PlayerControls controls)
    {
        if (controls.Player.Dash.triggered && !isDashing && Time.time >= lastDashTime + dashCooldown && !movement.Walljumping && !movement.onwall)
        {
            Debug.Log("Dash is triggered");
            lastDashTime = Time.time;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector3 dashDirection = headTarget.forward;
        dashDirection.y = 0f;
        dashDirection.Normalize();

        rb.linearVelocity = new Vector3(
            dashDirection.x * dashVelocity,
            rb.linearVelocity.y,
            dashDirection.z * dashVelocity
        );

        yield return new WaitForSeconds(dashDuration);

        // Restore normal running speed
        rb.linearVelocity = new Vector3(
            dashDirection.x * movement.moveSpeed, // movement script will overwrite this anyway
            rb.linearVelocity.y,
            dashDirection.z * movement.moveSpeed
        );

        isDashing = false;
    }
}
