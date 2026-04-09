using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    private PlayerControls controls;
    private Vector3 moveInput;

    //Camera obj
    [SerializeField] Transform headTarget;

    //change-able variable setup
    [SerializeField] float MoveSpeed = 5;
    [SerializeField] float JumpForce = 5;
    [SerializeField] float reboundForce = 10f;

    bool isGrounded;

    [SerializeField] Rigidbody rb;


    private bool IsRotating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new PlayerControls();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;


        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
        controls.Player.Move.canceled += ctx => moveInput = Vector3.zero;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void MovePlayer()
    {
       Vector3 Move = new Vector3(0f, 0f, moveInput.z);
       Vector3 worldMove = transform.TransformDirection(Move) * MoveSpeed;
       rb.MovePosition(rb.position + worldMove * Time.deltaTime);
    }

    void RotatePlayer() 
    {
        transform.Rotate(0f, moveInput.x, 0f);
        headTarget.rotation = transform.rotation;
    }
            
    void Jumping()
    {
       if(isGrounded && controls.Player.Jump.triggered)
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


    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("ReboundWall") && !isGrounded)
        {
            Debug.Log("Hit a Wall");

          
            transform.Rotate(0f, 180f, 0f);

           
            if (!IsRotating)
                StartCoroutine(WallRotate(0.35f));

            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            localVel.z = -localVel.z; // reverse forward/backward
            rb.linearVelocity = transform.TransformDirection(localVel);


            rb.AddForce(Vector3.up * reboundForce, ForceMode.Impulse);
            Debug.Log(transform.eulerAngles);

            return;
        }

        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            Vector3 e = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, e.y, 0f);
            return;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            isGrounded = false;
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
        MovePlayer();
        if (!IsRotating && isGrounded)
        {
            RotatePlayer();
        }
        Jumping();
        ResetCam();
    }
}
