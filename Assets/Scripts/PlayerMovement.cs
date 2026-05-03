using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public QuickTimeEvent quickTimeEvent;

    private PlayerControls controls;
    private float ForwardMovement;

    private float originalSpeed;

    float wallReattachDelay = 0.2f;
    float lastWallJumpTime = -10f;

    private Vector3 moveInput;
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 slideScale = new Vector3(1f, 0.3f, 1f);

    private Vector3 wallNormal;
    private Coroutine qteRoutine;

    [SerializeField] Transform headTarget;
    [SerializeField] GameObject QTEObj;

    [SerializeField] float MoveSpeed;
    public float moveSpeed
    {
        get => MoveSpeed;
        set => MoveSpeed = value;
    }
    public bool movementLocked = false;

    [SerializeField] float JumpForce;
    [SerializeField] float reboundForce;
    [SerializeField] float slideVel;
    [SerializeField] float slideDuration;

    [SerializeField] float slowDownFactor;

    [SerializeField] float slidingDownWallSpeed;

    [SerializeField] float dashVelocity = 20f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;
    private Vector3 groundNormal = Vector3.up;

    [SerializeField] Transform model;

    private bool isGrounded;
    public bool InRunZone;

    private bool CanSlide;
    private bool IsRotating = false;
    private bool isSliding = false;
    private bool OnWall = false;

    private bool PendingSlide = false;

    bool wallJumping = false;

    private bool IsQTE = false;
    private bool isDashing = false;

    private float lastDashTime = -Mathf.Infinity;

    private float currentYRotation;

    private Coroutine wallSlideRoutine;

    [SerializeField] float maxDuration;

    [SerializeField] Rigidbody rb;

    float groundCheckRadius = 0.3f;
    float groundCheckDistance = 0.6f;
    [SerializeField] LayerMask groundLayer;
    float groundedCoyoteTime = 0.12f;
    float lastGroundedTime;
    void Awake()
    {
        Debug.Log("[INIT] Awake called");

        controls = new PlayerControls();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ;

        controls.Player.Move.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector3>();
            Debug.Log("[INPUT] Move Input: " + moveInput);
        };

        controls.Player.Move.canceled += ctx =>
        {
            moveInput = Vector3.zero;
            Debug.Log("[INPUT] Move Canceled");
        };

        quickTimeEvent.SetMaxDuration(maxDuration);
        ForwardMovement = 1f;

        currentYRotation = transform.eulerAngles.y;

        Debug.Log("[INIT] Player initialized");
    }

    void OnEnable()
    {
        Debug.Log("[STATE] Controls Enabled");
        controls.Player.Enable();
    }

    void OnDisable()
    {
        Debug.Log("[STATE] Controls Disabled");
        controls.Player.Disable();
    }

    void DetermineForwardMovement()
    {
        float old = ForwardMovement;

        if (!movementLocked)
            if (!InRunZone)
                ForwardMovement = 1f;
            
            else
                ForwardMovement = moveInput.z;
        else
            ForwardMovement = 0f;

        if (old != ForwardMovement)
            Debug.Log("[MOVE] ForwardMovement changed: " + ForwardMovement);
    }

    void MovePlayer()
    {
        // if (OnWall)
        // {
        //     Debug.Log("[MOVE] Blocked: OnWall");
        //     return;
        // }

        // if (wallJumping)
        // {
        //     Debug.Log("[MOVE] Blocked: wallJumping");
        //     return;
        // }
        // {
        //     Debug.Log("[MOVE] Movement blocked (OnWall or wallJumping)");
        //     return;
        // }
        if (isDashing) return;
        if(!isGrounded) return;

        Debug.Log($"[Move] Unblocked OnWall is {OnWall}");
        Debug.Log($"[Move] Unblocked wallJumping is {wallJumping}");

        DetermineForwardMovement();

       
        Vector3 Move = new Vector3(moveInput.x, 0f, ForwardMovement);
        Vector3 worldMove = transform.TransformDirection(Move) * MoveSpeed;
        Vector3 slopeMove = Vector3.ProjectOnPlane(worldMove, groundNormal).normalized * MoveSpeed;

        rb.linearVelocity = new Vector3(
            slopeMove.x,
            rb.linearVelocity.y,
            slopeMove.z
        );

        Debug.Log("[MOVE] Velocity set to: " + rb.linearVelocity);
    }

    void CheckDashInput()
    {
        if (controls.Player.Dash.triggered && !isDashing && Time.time >= lastDashTime + dashCooldown && !OnWall && !wallJumping)
        {
            Debug.Log("Dash is triggered");
            lastDashTime = Time.time;
            StartCoroutine(DashRoutine());
        }
    }


    void RotatePlayer()
    {
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();
        currentYRotation += look.x;

        rb.MoveRotation(Quaternion.Euler(0f, currentYRotation, 0f));

        if (!IsRotating)
            headTarget.rotation = transform.rotation;
    }


    void Jumping()
    {
        if (isGrounded && controls.Player.Jump.triggered)
        {
            Debug.Log("[JUMP] Jump triggered");
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }

    void ResetCam()
    {
        if (controls.Player.ResetCam.triggered)
        {
            Debug.Log("[CAM] Reset camera");
            headTarget.rotation = transform.rotation;
        }
    }

    void WallJump()
    {
        Debug.Log("[WALL] WallJump executed");

        lastWallJumpTime = Time.time;

        rb.linearVelocity = Vector3.zero;

        rb.AddForce(wallNormal.normalized * JumpForce * 1.5f, ForceMode.Impulse);
        rb.AddForce(Vector3.up * JumpForce * 1.5f, ForceMode.Impulse);
    }

    void handleWallJump()
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

    void ReboundWall()
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

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("[COLLISION ENTER] Hit: " + other.collider.tag);

        // if (other.collider.CompareTag("Ground"))
        // {
        //     isGrounded = true;
        //     Debug.Log("[STATE] Grounded = TRUE");
        //
        //     Vector3 e = transform.eulerAngles;
        //     transform.rotation = Quaternion.Euler(0f, e.y, 0f);
        //     return;
        // }

    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log("[COLLISION EXIT] Left: " + other.collider.tag);

        // if (other.collider.CompareTag("Ground"))
        // {
        //     isGrounded = false;
        //     Debug.Log("[STATE] Grounded = FALSE");
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[TRIGGER ENTER] Hit: " + other.tag);

        if (other.CompareTag("LowObj"))
        {
            Debug.Log("[QTE] QTE Triggered");

            QTEObj.SetActive(true);
            qteRoutine = StartCoroutine(runQTE(maxDuration));

            CanSlide = true;
        }

        if (other.CompareTag("Anti-RunZone"))
        {
            InRunZone = true;
            Debug.Log("[STATE] Entered RunZone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Anti-RunZone"))
        {
            InRunZone = false;
            Debug.Log("[STATE] Exited RunZone");
        }
    }

    void OnCollisionStay(Collision other)
    {
        // if (other.collider.CompareTag("Ground"))
        // {
        //     isGrounded = true;
        //     Debug.Log("[STATE] Grounded = TRUE");
            
        //     groundNormal = other.GetContact(0).normal;
            
        //     Vector3 e = transform.eulerAngles;
        //     transform.rotation = Quaternion.Euler(0f, e.y, 0f);
        //     return;
        // }

        if (other.collider.CompareTag("ReboundWall") && !isGrounded)
        {
            Debug.Log("[WALL] Staying on ReboundWall");

            if (Time.time < lastWallJumpTime + wallReattachDelay)
            {
                Debug.Log("[WALL] Ignoring wall (reattach delay)");
                return;
            }

            if (!OnWall)
            {
                wallNormal = other.GetContact(0).normal;
                Debug.Log("[WALL] Wall normal: " + wallNormal);

                ReboundWall();
            }
        }
    }

    IEnumerator UnlockWallJump()
    {
        Debug.Log("[WALL] UnlockWallJump started");
        yield return new WaitForSeconds(0.25f);
        wallJumping = false;
        Debug.Log("[WALL] WallJump unlocked");
    }

    IEnumerator WallSliding()
    {
        Debug.Log("[WALL] WallSliding started");

        OnWall = true;

        while (OnWall && !isGrounded)
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

    IEnumerator WallRotate(float duration)
    {
        Debug.Log("[WALL] WallRotate started");

        IsRotating = true;

        float startY = currentYRotation;
        float endY = currentYRotation + 180f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            currentYRotation = Mathf.Lerp(startY, endY, t);

            transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
            headTarget.rotation = transform.rotation;

            Debug.Log("[WALL] Rotating Y: " + currentYRotation);

            yield return null;
        }

        Debug.Log("[WALL] WallRotate finished");

        IsRotating = false;
        wallJumping = false;
    }

    IEnumerator SlideRoutine()
    {
        Debug.Log("[SLIDE] SlideRoutine started");

        isSliding = true;

        model.localScale = slideScale;

        rb.linearVelocity += transform.forward * slideVel;
        Debug.Log("[SLIDE] Slide burst velocity: " + rb.linearVelocity);

        yield return new WaitForSeconds(slideDuration);

        model.localScale = normalScale;

        isSliding = false;

        Debug.Log("[SLIDE] SlideRoutine ended");
    }

    IEnumerator runQTE(float duration)
    {
        Debug.Log("[QTE] QTE started");

        originalSpeed = MoveSpeed;
        MoveSpeed *= slowDownFactor;

        float timer = 0f;
        quickTimeEvent.SetDuration(0f);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            quickTimeEvent.SetDuration(timer);

            if (controls.Player.Slide.triggered)
            {
                Debug.Log("[QTE] SUCCESS");

                quickTimeEvent.ResetUI();
                QTEObj.SetActive(false);

                IsQTE = false;
                CanSlide = false;

                MoveSpeed = originalSpeed;

                PendingSlide = true;
                yield break;
            }

            yield return null;
        }

        Debug.Log("[QTE] FAILED");

        quickTimeEvent.ResetUI();
        QTEObj.SetActive(false);

        IsQTE = false;
        CanSlide = false;
        MoveSpeed = originalSpeed;
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
            dashDirection.x * MoveSpeed,
            rb.linearVelocity.y,
            dashDirection.z * MoveSpeed
        );

        isDashing = false;
        Debug.Log("Dash is complete");
    }

    void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down,
                out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            lastGroundedTime = Time.time;

            // 仍然记录地面法线，用于斜坡移动
            groundNormal = hit.normal;
        }
        else
        {
            // 给一个很短的容错窗口，斜坡过渡不会断
            isGrounded = (Time.time - lastGroundedTime) <= groundedCoyoteTime;
        }
    }


    void Update()
    {

        GroundCheck();
        CheckDashInput();
        MovePlayer();

        if (!IsRotating && isGrounded && !CanSlide)
            RotatePlayer();

        Jumping();
        ResetCam();
        handleWallJump();
        if (PendingSlide && isGrounded)
        {
            PendingSlide = false;
            StartCoroutine(SlideRoutine());
        }
    }
}