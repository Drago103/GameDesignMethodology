using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    private PlayerControls controls;
    private Vector3 moveInput;

    [SerializeField] Transform headTarget;

    [SerializeField] float MoveSpeed = 5;

    [SerializeField] Rigidbody rb;

    private bool IsRotating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new PlayerControls();

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
        headTarget.Rotate(0f, moveInput.x, 0f);
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

            rb.linearVelocity = rb.linearVelocity *2;
            MoveSpeed = MoveSpeed;
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
        RotatePlayer();
    }
}
