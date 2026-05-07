using UnityEngine;
using System.Collections;

public class WallRebound : MonoBehaviour
{
    [SerializeField] Movement movement;

    private float wallReattachDelay = 0.2f;
    private float lastWallJumpTime = -10f;

    public float LTWallJump => lastWallJumpTime;

    public float ReattachDelay => wallReattachDelay;

    [SerializeField] Transform headTarget;

    [SerializeField] Rigidbody rb;

    [SerializeField] float slidingDownWallSpeed;

    private bool IsRotating = false;

    public bool isRotating => IsRotating;

    private bool OnWall = false;
    public bool onwall => OnWall;

    private bool wallJumping = false;
    public bool Walljumping => wallJumping;

    private Coroutine wallSlideRoutine;

    void Awake()
    {
        if (movement == null)
            movement = GetComponent<Movement>();
    }

    void WallJump()
    {
        Debug.Log("[WALL] WallJump executed");

        lastWallJumpTime = Time.time;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(movement.WallNormal.normalized * movement.jumpforce * 1.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * movement.jumpforce* 1.5f, ForceMode.Impulse);
    }

    public void handleWallJump(PlayerControls controls)
    {
        if (!OnWall || IsRotating || wallJumping)
            return;

        if (controls.Player.Jump.triggered)
        {
            Debug.Log("[WALL] Wall jump input detected");

            if (wallSlideRoutine != null)
            {
                Debug.Log("[WALL] Stopping wall slide routine");
                StopCoroutine(wallSlideRoutine);
            }

            wallJumping = true;
            OnWall = false;

            WallJump();
            StartCoroutine(UnlockWallJump());
        }
    }

    public void ReboundWall()
    {
        if (!OnWall)
        {
            Debug.Log("[WALL] ReboundWall triggered");

            OnWall = true;

            if (!IsRotating)
            {
                Debug.Log("[WALL] Starting WallRotate");
                StartCoroutine(WallRotate(0.25f));
            }

            wallSlideRoutine = StartCoroutine(WallSliding());
        }
    }

    IEnumerator WallRotate(float duration)
    {
        Debug.Log("[WALL] WallRotate started");

        IsRotating = true;

        float startY = movement.CurrentYPos;
        float endY = movement.CurrentYPos + 180f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            movement.CurrentYPos = Mathf.Lerp(startY, endY, t);

            transform.rotation = Quaternion.Euler(0f, movement.CurrentYPos, 0f);
            headTarget.rotation = transform.rotation;

            Debug.Log("[WALL] Rotating Y: " + movement.CurrentYPos);

            yield return null;
        }

        Debug.Log("[WALL] WallRotate finished");

        IsRotating = false;
        wallJumping = false;
    }

    IEnumerator WallSliding()
    {
        Debug.Log("[WALL] WallSliding started");

        OnWall = true;

        while (OnWall && !movement.IsGrounded)
        {
            Vector3 vel = rb.linearVelocity;

            if (vel.y < -slidingDownWallSpeed)
                vel.y = -slidingDownWallSpeed;

            vel.x *= 0.1f;
            vel.z *= 0.1f;

            rb.linearVelocity = vel;

            Debug.Log("[WALL] Sliding velocity: " + vel);

            yield return null;
        }

        Debug.Log("[WALL] WallSliding ended");
        OnWall = false;
    }

    IEnumerator UnlockWallJump()
    {
        Debug.Log("[WALL] UnlockWallJump started");
        yield return new WaitForSeconds(0.25f);
        wallJumping = false;
        Debug.Log("[WALL] WallJump unlocked");
    }

}
