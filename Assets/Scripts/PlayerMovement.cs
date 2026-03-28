using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private PlayerControls controls;
    private Vector2 moveInput;

    [SerializeField] float MoveSpeed = 5;

    [SerializeField] Rigidbody rb;

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

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }
}
