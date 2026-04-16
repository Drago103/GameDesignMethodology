using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private PlayerControls _controls;

    private bool _isHolding;
    [Header("Ability")] [SerializeField] private float slowFactor = 0.5f; // 减速倍率
    [SerializeField] private float changeSpeed = 5f; // 减速过渡速度
    [SerializeField] private float duration = 2f; // 持续时间
    [SerializeField] private float cooldown = 5f; // CD 时间

    [Header("UI")] [SerializeField] private Image imgCooldownMask; // CD 遮罩
    [SerializeField] private TextMeshProUGUI txtCD; // CD 倒计时文字

    private bool _isReady = true;
    private bool _isActive = false;
    private float _cooldownTimer = 0f;
    private float _durationTimer = 0f;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        txtCD.text = "Ready!";
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            // Activating slowdown the time
            _durationTimer -= Time.unscaledDeltaTime;

            if (_durationTimer <= 0f)
            {
                // CD started
                _isActive = false;
                _cooldownTimer = cooldown;
            }
        }

        if (!_isReady && !_isActive)
        {
            // in CD
            _cooldownTimer -= Time.unscaledDeltaTime;

            if (_cooldownTimer <= 0f)
            {
                // CD finished
                _isReady = true;
                _cooldownTimer = 0f;
            }
        }

        UpdateTimeScale();
        UpdateUI();
    }

    private void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.SlowDown.performed += OnSlowDown;
    }

    private void OnDisable()
    {
        _controls.Player.SlowDown.performed -= OnSlowDown;
        _controls.Player.Disable();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void OnSlowDown(InputAction.CallbackContext ctx)
    {
        if (!_isReady) return;

        _isReady = false;
        _isActive = true;
        _durationTimer = duration;

        Debug.Log("Time Slow DOWN!");
    }

    void TimeSlowDown()
    {
        print(_isHolding);
        float target = _isHolding ? slowFactor : 1f;

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, target, changeSpeed * Time.unscaledDeltaTime);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void UpdateTimeScale()
    {
        float target = _isActive ? slowFactor : 1f;
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, target, changeSpeed * Time.unscaledDeltaTime);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void UpdateUI()
    {
        if (imgCooldownMask != null)
        {
            imgCooldownMask.fillAmount = _cooldownTimer / cooldown;
        }

        if (txtCD != null)
        {
            if (!_isReady)
            {
                if (_cooldownTimer > 0f)
                {
                    txtCD.text = "TIme Slow DOWN!";
                }

                txtCD.text = "CD" + Mathf.CeilToInt(_cooldownTimer).ToString();
                txtCD.gameObject.SetActive(true);
            }
            else
            {
                txtCD.text = "Ready!";
            }
        }
    }
}