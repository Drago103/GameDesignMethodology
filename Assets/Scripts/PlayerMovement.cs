    using UnityEngine;
    using System.Collections;

    public class PlayerMovement : MonoBehaviour
    {

        public QuickTimeEvent quickTimeEvent;

        private PlayerControls controls;
        private float ForwardMovement;

        private float originalSpeed;

        float wallReattachDelay = 0.35f;
        float lastWallJumpTime = -10f;

        private Vector3 moveInput;
        private Vector3 normalScale = new Vector3(1f, 1f, 1f);
        private Vector3 slideScale = new Vector3(1f, 0.3f, 1f);

        private Vector3 wallNormal;
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

        [SerializeField] float slidingDownWallSpeed;

        [SerializeField] Transform model;

        private bool isGrounded;
        private bool InRunZone;

        private float currentYRotation;

        private Coroutine wallSlideRoutine;

        [SerializeField] float maxDuration;

        [SerializeField] Rigidbody rb;

        enum PlayerState { Normal, WallSliding, WallJumping, Rotating, Sliding, QTE }
        PlayerState state;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            state = PlayerState.Normal;

            controls = new PlayerControls();

            rb.constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;


            controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
            controls.Player.Move.canceled += ctx => moveInput = Vector3.zero;

            quickTimeEvent.SetMaxDuration(maxDuration);
            ForwardMovement = 1f;

            currentYRotation = transform.eulerAngles.y;

            //  QTEObj = null;
            
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
        if (state == PlayerState.WallSliding ||
            state == PlayerState.WallJumping ||
            state == PlayerState.Rotating ||
            state == PlayerState.Sliding)
            return;

        Debug.Log($"[MovePlayer] state={state} vel={rb.linearVelocity}");
        Vector3 Move = new Vector3(0f, 0f,  ForwardMovement);
        Vector3 worldMove = transform.TransformDirection(Move) * MoveSpeed;
        rb.linearVelocity = new Vector3(worldMove.x, rb.linearVelocity.y, worldMove.z);
        }

        void RotatePlayer()
        {
            currentYRotation += moveInput.x; // or multiply by turn speed
            rb.MoveRotation(Quaternion.Euler(0f, currentYRotation, 0f));
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

        void WallJump()
        {
            lastWallJumpTime = Time.time;

            rb.linearVelocity = Vector3.zero;

            rb.AddForce(wallNormal.normalized * JumpForce * 1.5f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * JumpForce * 2f, ForceMode.Impulse);

            Debug.Log("Player is wall jumping!!!");
        }

        void handleWallJump()
        {
            if (state != PlayerState.WallSliding) return;

            if(controls.Player.Jump.triggered)
            {
                Debug.Log("Perform Wall jump");

                if (wallSlideRoutine != null)
                    StopCoroutine(wallSlideRoutine);

                state = PlayerState.WallJumping;
                WallJump();
                StartCoroutine(UnlockWallJump());
            }
        }
        void ReboundWall()
        {
           if (state == PlayerState.WallSliding ||
            state == PlayerState.WallJumping ||
            state == PlayerState.Rotating)
            return;

            StartCoroutine(WallSequence());
            
        }

        void ExitReboundWall()
        {
            if (state != PlayerState.WallSliding) return;

            state = PlayerState.Normal;

            if (wallSlideRoutine != null)
                StopCoroutine(wallSlideRoutine);
        }

        void OnCollisionEnter(Collision other)
        {

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

            if (other.collider.CompareTag("ReboundWall"))
            {
                ExitReboundWall();
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
                //state = PlayerState.QTE;
            }
        }

        void OnCollisionStay(Collision other)
        {
            if (other.collider.CompareTag("ReboundWall") && !isGrounded)
            {
               if (!isGrounded &&
                    rb.linearVelocity.y <= 0f &&
                    Time.time >= lastWallJumpTime + wallReattachDelay &&
                    state != PlayerState.WallJumping &&
                    state != PlayerState.Rotating &&
                    state != PlayerState.Sliding)
                {
                    wallNormal = other.GetContact(0).normal;
                    ReboundWall();
                }
                
                //wallJumping = false;
            
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

        IEnumerator UnlockWallJump()
        {
            yield return new WaitForSeconds(0.25f);
            //state = PlayerState.Normal;
        }

        IEnumerator WallSequence()
        {
            state = PlayerState.Rotating;

            yield return StartCoroutine(WallRotate(0.25f));

            state = PlayerState.WallSliding;
            wallSlideRoutine = StartCoroutine(WallSliding());
        }

        IEnumerator WallSliding()
        {
            // Reduce downward speed to create a sliding effect
            while (state == PlayerState.WallSliding && !isGrounded)
            {
                Vector3 vel = rb.linearVelocity;

                // Clamp downward velocity
                if (vel.y < -slidingDownWallSpeed)
                    vel.y = -slidingDownWallSpeed;

                vel.x *=0.1f;
                vel.z *=0.1f;

                rb.linearVelocity = vel;

                yield return null;
            }

            state = PlayerState.Normal;
        }

        IEnumerator WallRotate(float duration)
        {

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
       
        }


        IEnumerator SlideRoutine()
        {
            state = PlayerState.Sliding;

            // Shrink player
            model.localScale = slideScale;

            // Add forward burst
            rb.linearVelocity += transform.forward * slideVel;

            // Stay in slide for a moment
            yield return new WaitForSeconds(slideDuration);

            // Return to normal size
            model.localScale = normalScale;

            state = PlayerState.Normal;
        }

    IEnumerator runQTE(float duration)
        {
            state = PlayerState.QTE;
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

                    state = PlayerState.Normal;

                    MoveSpeed = originalSpeed;

                    StartCoroutine(SlideRoutine());

                    yield break;
                }

                yield return null;
            }

            Debug.Log("QTE failed");
            quickTimeEvent.ResetUI();
            QTEObj.SetActive(false);
            state = PlayerState.Normal;
            MoveSpeed = originalSpeed;

        }

        // Update is called once per frame
        void Update()
        {
            MovePlayer();
            if (state == PlayerState.Normal && isGrounded)
            {
                RotatePlayer();
            }
            Jumping();
            ResetCam();
            handleWallJump();
        }
    }
