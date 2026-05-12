using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
  
    public Dash dash;

    public Sliding slide;

    public WallRebound wallRebound;

    private PlayerControls controls;
    private float ForwardMovement;

    private Vector3 moveInput;

    private Vector3 wallNormal;

    public Vector3 WallNormal => wallNormal;

    [SerializeField] Transform headTarget;

    [SerializeField] float MoveSpeed;

    [SerializeField] bool CanDash;

    [SerializeField] bool CanSlide;

    [SerializeField] bool CanWallJump;
    public float moveSpeed
    {
        get => MoveSpeed;
        set => MoveSpeed = value;
    }
    public bool movementLocked = false;

    private bool CanJump = true;

    [SerializeField] float mouseSensitivity = 150f;

    [SerializeField] float JumpForce;

    public float jumpforce => JumpForce;

    private Vector3 groundNormal = Vector3.up;

    [SerializeField] Transform model;

    private bool isGrounded;

    public bool IsGrounded => isGrounded;
    public bool InRunZone;

    private bool cameraLocked = true;

    public bool IsPaused { get; set; }

    private float currentYRotation;

    public float CurrentYPos{
        get => currentYRotation;
        set => currentYRotation = value;
    }

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
                         RigidbodyConstraints.FreezeRotationY |
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
        if(dash!=null && dash.IsDashing) return;
        if (!isGrounded) return;

        Debug.Log($"[Move] Unblocked OnWall is {wallRebound.onwall}");
        Debug.Log($"[Move] Unblocked wallJumping is {wallRebound.Walljumping}");    

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

    void RotatePlayer()
    {
        if (cameraLocked) return;
        
        Vector2 look = controls.Player.Look.ReadValue<Vector2>();
        
        float yaw = look.x * mouseSensitivity * Time.deltaTime;

        currentYRotation += yaw;

        rb.MoveRotation(Quaternion.Euler(0f, currentYRotation, 0f));

        if (!wallRebound.isRotating)
            headTarget.rotation = transform.rotation;
    }

    void Jumping()
    {
        if (wallRebound.onwall || wallRebound.Walljumping)
        return;

        if (isGrounded && CanJump && controls.Player.Jump.triggered)
        {
            Debug.Log("[JUMP] Jump triggered");
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            StartCoroutine(unlockJump());
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

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("[COLLISION ENTER] Hit: " + other.collider.tag);
        if (other.collider.CompareTag("Wall"))
        {
            wallNormal = other.GetContact(0).normal;
            rb.AddForce( wallNormal* 2f, ForceMode.Impulse);
        }

        if (other.collider.CompareTag("Ground"))
        {
            
            CanJump = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        Debug.Log("[COLLISION EXIT] Left: " + other.collider.tag);


        // if (other.collider.CompareTag("Ground"))
        // {
            
        //     CanJump = false;
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[TRIGGER ENTER] Hit: " + other.tag);

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
        if (other.collider.CompareTag("ReboundWall") && !isGrounded)
        {
            Debug.Log("[WALL] Staying on ReboundWall");

            if (Time.time < wallRebound.LTWallJump + wallRebound.ReattachDelay)
            {
                Debug.Log("[WALL] Ignoring wall (reattach delay)");
                return;
            }

            if (!wallRebound.onwall)
            {
                wallNormal = other.GetContact(0).normal;
                Debug.Log("[WALL] Wall normal: " + wallNormal);

                wallRebound.ReboundWall();
            }
        }
    }

   void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down,
                out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            isGrounded = true;
            lastGroundedTime = Time.time;
            groundNormal = hit.normal;
        }
        else
        {
            isGrounded = (Time.time - lastGroundedTime) <= groundedCoyoteTime;
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.15f); // adjust as needed
        cameraLocked = false;
    }

    IEnumerator unlockJump()
    {
        CanJump = false;                    
        yield return new WaitForSeconds(0.2f);                    
    }

    void Update()
    {
        if (IsPaused) return;
        
        GroundCheck();
        if (CanDash)
        {
            dash.CheckDashInput(controls);
        }
        MovePlayer();

        if (!wallRebound.isRotating && !slide.IsSliding && !wallRebound.onwall)
            RotatePlayer();

        Jumping();
        ResetCam();

        if (CanWallJump)
        {
            wallRebound.handleWallJump(controls);
        }

        if (CanSlide)
        {
            slide.Slide(controls);
        }
    
    }
}
