using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    private PlayerControls controls;
    private float ForwardMovement;
    private Vector3 moveInput;

    //Camera obj
    [SerializeField] Transform headTarget;

    //change-able variable setup
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] float JumpForce = 5;

    bool isGrounded;
    bool InRunZone;

    [SerializeField] Rigidbody rb;


    private bool IsRotating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
        controls.Player.Move.canceled += ctx => moveInput = Vector3.zero;

        ForwardMovement = 1f;
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
        transform.Rotate(0f, moveInput.x, 0f);
        headTarget.Rotate(0f, moveInput.x, 0f);
    }
            
    void Jumping()
    {
        if(isGrounded && controls.Player.Jump.triggered)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }

    void ResetCam()
    {
        if (controls.Player.ResetCam.triggered)
        {
            headTarget.rotation = transform.rotation;
        }
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Wall"))
        {
            Debug.Log("Hit a Wall");

            transform.Rotate(0f,180f,0f);

            if (!IsRotating)
            {
                StartCoroutine(WallRotate(0.35f));
            }

            rb.linearVelocity = -rb.linearVelocity *2;
            MoveSpeed = MoveSpeed;
            return;
        }

        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
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
        }
        
        if (other.collider.CompareTag("Anti-RunZone"))
        {
            InRunZone = false;
        }

    }

    IEnumerator WallRotate(float duration)
    {
        IsRotating = true;

        Quaternion startRot = headTarget.rotation;
        Quaternion endRot = headTarget.rotation * Quaternion.Euler(0f,180f,0f);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;
            headTarget.rotation = Quaternion.Slerp(startRot, endRot, time);
            yield return null;
        }

        IsRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetermineForwardMovement();
        MovePlayer();
        if (!IsRotating && isGrounded)
        {
            RotatePlayer();
        }
        Jumping();
        ResetCam();
    }
}
