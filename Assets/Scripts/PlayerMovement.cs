using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    private PlayerControls controls;
    private Vector2 moveInput;

    [SerializeField] Transform headTarget;

    [SerializeField] float MoveSpeed = 5;

    [SerializeField] Rigidbody rb;

    private bool IsRotating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
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
       Vector3 Move = new Vector3(moveInput.x, 0f, 1f) * MoveSpeed;
       rb.linearVelocity = new Vector3(Move.x, rb.linearVelocity.y, Move.z);
    }


    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Wall"))
        {
            Debug.Log("Hit a Wall");

            transform.Rotate(0f,180f,0f);

            if (!IsRotating)
            {
                StartCoroutine(SmoothRotate(0.35f));
            }

            rb.linearVelocity = -rb.linearVelocity *2;
            MoveSpeed = -MoveSpeed;
        }
    }

    IEnumerator SmoothRotate(float duration)
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
    }
}
