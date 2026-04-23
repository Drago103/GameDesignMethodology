using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public QuickTimeEvent quickTimeEvent;

    private PlayerControls controls;
    private float ForwardMovement;

    private float originalSpeed;
    private Vector3 moveInput;
    private Vector2 mouseLook;
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 slideScale = new Vector3(1f, 0.3f, 1f);

    //Camera obj
    [SerializeField] Transform headTarget;

    //change-able variable setup
    [SerializeField] float MoveSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] float reboundForce;
    [SerializeField] float slideVel;
    [SerializeField] float slideDuration;
    [SerializeField] float slowDownFactor;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float slideCooldown = 2f;

    [SerializeField] float maxDuration;

    [SerializeField] Rigidbody rb;

    private bool isGrounded;
    private bool InRunZone;
    private bool IsRotating = false;
    private bool isSliding = false;

    private float currentYRotation;
    private float lastSlideTime = -Mathf.Infinity;

    void Awake()
    {
        controls = new PlayerControls();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
        controls.Player.Move.canceled += ctx => moveInput = Vector3.zero;

        controls.Player.Look.performed += ctx => mouseLook = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => mouseLook = Vector2.zero;

        ForwardMovement = 1f;

        currentYRotation = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void DetermineForwardMovement()
    {
        if (!InRunZone)
        {
            ForwardMovement = 1f;
        }
        else
        {
            ForwardMovement = moveInput.z;
        }
    }

    void MovePlayer()
    {
        Vector3 Move = new Vector3(0f, 0f, ForwardMovement);
        Vector3 worldMove = transform.TransformDirection(Move) * MoveSpeed;
        rb.MovePosition(rb.position + worldMove * Time.deltaTime);
    }

    void RotatePlayer()
    {
        currentYRotation += mouseLook.x * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);

        if (!IsRotating)
            headTarget.rotation = transform.rotation;
    }

    void Jumping()
    {
        if (isGrounded && controls.Player.Jump.triggered)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }

    void ResetCam()
    {
        if (controls.Player.ResetCam.triggered)
        {
            headTarget.rotation = transform.rotation;
        }
    }

    void CheckSlideInput()
    {
        if (controls.Player.Interact.triggered && !isSliding && Time.time >= lastSlideTime + slideCooldown)
        {
            lastSlideTime = Time.time;
            StartCoroutine(SlideRoutine());
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("ReboundWall") && !isGrounded)
        {
            Debug.Log("Hit a Rebound Wall");

            if (!IsRotating)
            {
                Debug.Log("player is rotating");
                StartCoroutine(WallRotate(0.35f));
            }

            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            localVel.z = -localVel.z;
            rb.linearVelocity = transform.TransformDirection(localVel);

            rb.AddForce(Vector3.up * reboundForce, ForceMode.Impulse);
            Debug.Log(transform.eulerAngles);

            return;
        }

        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player is on the ground");
            Vector3 e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, e.y, 0f);
            return;
        }

        if (other.collider.CompareTag("Anti-RunZone"))
        {
            InRunZone = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player is off the ground");
        }

        if (other.collider.CompareTag("Anti-RunZone"))
        {
            InRunZone = false;
        }
    }

    IEnumerator WallRotate(float duration)
    {
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
            yield return null;
        }

        IsRotating = false;
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;

        transform.localScale = slideScale;
        rb.linearVelocity += transform.forward * slideVel;

        yield return new WaitForSeconds(slideDuration);

        transform.localScale = normalScale;
        isSliding = false;
    }

    void Update()
    {
        DetermineForwardMovement();
        MovePlayer();
        if (!IsRotating)
        {
            RotatePlayer();
        }
        Jumping();
        ResetCam();
        CheckSlideInput();
    }
}