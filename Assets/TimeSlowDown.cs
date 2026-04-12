using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class NewMonoBehaviourScript : MonoBehaviour
{
    private PlayerControls _controls;

    private bool _isHolding;
    [SerializeField] private float slowFactor = 0.5f;
    [SerializeField] private float changeSpeed = 5f;
    private void Awake()
    {
        _controls = new PlayerControls();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TimeSlowDown();
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.SlowDown.started += OnSlowDownStarted;
        _controls.Player.SlowDown.canceled += OnSlowDownCanceled;
    }

    private void OnDisable()
    {
        _controls.Player.SlowDown.started -= OnSlowDownStarted;
        _controls.Player.SlowDown.canceled -= OnSlowDownCanceled;
        _controls.Player.Disable();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void OnSlowDownStarted(InputAction.CallbackContext ctx)
    { 
        _isHolding = true;
    }
    
    void OnSlowDownCanceled(InputAction.CallbackContext ctx)
    {
        _isHolding = false;
    }
    
    void TimeSlowDown()
    {
        float target = _isHolding ? slowFactor : 1f;

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, target, changeSpeed * Time.unscaledDeltaTime
        );

        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
}
