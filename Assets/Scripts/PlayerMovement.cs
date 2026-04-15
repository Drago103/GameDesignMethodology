using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    public QuickTimeEvent quickTimeEvent;

    private PlayerControls controls;
    private float ForwardMovement;

    private float originalSpeed;
    private Vector3 moveInput;
    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 slideScale = new Vector3(1f, 0.3f, 1f);
    private Coroutine qteRoutine;

    //Camera obj
    [SerializeField] Transform headTarget;
    [SerializeField] GameObject QTEObj;

    //change-able variable setup
    [SerializeField] float MoveSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] float reboundForce;
    [SerializeField] float slideVel;
    [SerializeField] float slideDuration;

    [SerializeField] float slowDownFactor;

    private bool isGrounded;
    private bool InRunZone;
    private bool CanSlide;
    private bool IsRotating = false;
    private bool isSliding = false;

    private bool IsQTE = false;

    [SerializeField] float maxDuration;

    [SerializeField] Rigidbody rb;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new PlayerControls();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;


        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
        controls.Player.Move.canceled += ctx => moveInput = Vector3.zero;

        quickTimeEvent.SetMaxDuration(maxDuration);
        ForwardMovement = 1f;

        //QTEObj = null;
        
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
       Vector3 Move = new Vector3(0f, 0f,  ForwardMovement);
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
            {
                Debug.Log("player is rotating");
                StartCoroutine(WallRotate(0.35f));
            }
                
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LowObj"))
        {
            //ShowSlidePrompt();
            QTEObj.SetActive(true);
            qteRoutine = StartCoroutine(runQTE(maxDuration));   
            Debug.Log("can slide, hit e to slide.");
            CanSlide = true;
        }
    }

    /* void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LowObj"))
        {
            CanSlide = false;
            IsQTE = false;

            transform.localScale = normalScale;

            if (qteRoutine != null)
            {
                StopCoroutine(qteRoutine);
                qteRoutine = null;
            }

            quickTimeEvent.ResetUI();
            QTEObj.SetActive(false);

            Debug.Log("cannot slide");
        }
    } */


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

    IEnumerator SlideRoutine()
    {
        isSliding = true;

        // Shrink player
        transform.localScale = slideScale;

        // Add forward burst
        rb.linearVelocity += transform.forward * slideVel;

        // Stay in slide for a moment
        yield return new WaitForSeconds(slideDuration);

        // Return to normal size
        transform.localScale = normalScale;

        isSliding = false;
    }

   IEnumerator runQTE(float duration)
    {
        originalSpeed = MoveSpeed;
        MoveSpeed *= slowDownFactor;
        float timer = 0f;
        quickTimeEvent.SetDuration(0f);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            quickTimeEvent.SetDuration(timer);

            if (controls.Player.Interact.triggered)
            {
                Debug.Log("QTE success");

                quickTimeEvent.ResetUI();
                QTEObj.SetActive(false);

                IsQTE = false;
                CanSlide = false;

                MoveSpeed = originalSpeed;

                StartCoroutine(SlideRoutine());

                yield break;
            }

            yield return null;
        }

        Debug.Log("QTE failed");
        quickTimeEvent.ResetUI();
        QTEObj.SetActive(false);
        IsQTE = false;
        CanSlide = false;
        MoveSpeed = originalSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        if (!IsRotating && isGrounded && !CanSlide)
        {
            RotatePlayer();
        }
        Jumping();
        ResetCam();
    }
}
