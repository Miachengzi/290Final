using UnityEngine;

[DisallowMultipleComponent]
public class ObjectRotator : MonoBehaviour
{
    public enum RotationSpace
    {
        Local,
        World
    }

    [Header("Rotation Settings")]
    [Tooltip("Enable/disable rotation")]
    public bool rotateEnabled = true;

    [Tooltip("Space to rotate in")]
    public RotationSpace rotationSpace = RotationSpace.Local;

    [Tooltip("Rotation axis (normalized automatically)")]
    public Vector3 rotationAxis = Vector3.up;

    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 30f;

    [Tooltip("Randomize rotation speed on start")]
    public bool randomizeSpeed = false;

    [Tooltip("Minimum speed when randomizing")]
    public float minRandomSpeed = 15f;

    [Tooltip("Maximum speed when randomizing")]
    public float maxRandomSpeed = 45f;

    [Tooltip("Smooth acceleration to target speed (0 for instant)")]
    public float accelerationTime = 0f;

    [Header("Runtime Control")]
    [Tooltip("Current effective rotation speed")]
    [SerializeField] private float _currentSpeed;

    private float _targetSpeed;
    private float _velocity;
    private Vector3 _normalizedAxis;

    private void Start()
    {
        InitializeRotation();
    }

    private void InitializeRotation()
    {
        _normalizedAxis = rotationAxis.normalized;
        _targetSpeed = rotationSpeed;

        if (randomizeSpeed)
        {
            _targetSpeed = Random.Range(minRandomSpeed, maxRandomSpeed);
        }

        if (accelerationTime <= 0)
        {
            _currentSpeed = _targetSpeed;
        }
        else
        {
            _currentSpeed = 0f;
        }
    }

    private void Update()
    {
        UpdateRotationParameters();
        ApplyRotation();
    }

    private void UpdateRotationParameters()
    {
        // 如果轴发生变化，重新归一化
        if (rotationAxis != _normalizedAxis && rotationAxis.sqrMagnitude > 0)
        {
            _normalizedAxis = rotationAxis.normalized;
        }

        // 更新目标速度
        if (!randomizeSpeed && !Mathf.Approximately(_targetSpeed, rotationSpeed))
        {
            _targetSpeed = rotationSpeed;
        }

        // 平滑过渡速度
        if (accelerationTime > 0)
        {
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _velocity, accelerationTime);
        }
        else
        {
            _currentSpeed = _targetSpeed;
        }
    }

    private void ApplyRotation()
    {
        if (!rotateEnabled || Mathf.Approximately(_currentSpeed, 0f))
            return;

        float rotationAmount = _currentSpeed * Time.deltaTime;
        Quaternion rotation = Quaternion.AngleAxis(rotationAmount, _normalizedAxis);

        if (rotationSpace == RotationSpace.Local)
        {
            transform.localRotation *= rotation;
        }
        else
        {
            transform.rotation *= rotation;
        }
    }

    // 在Inspector值变化时调用
    private void OnValidate()
    {
        // 确保轴不为零向量
        if (rotationAxis.sqrMagnitude == 0)
        {
            rotationAxis = Vector3.up;
        }

        // 限制随机速度范围
        if (minRandomSpeed > maxRandomSpeed)
        {
            minRandomSpeed = maxRandomSpeed;
        }

        // 如果不在播放模式，直接初始化
        if (!Application.isPlaying) return;

        // 如果启用了随机速度，不覆盖目标速度
        if (!randomizeSpeed)
        {
            _targetSpeed = rotationSpeed;
        }
    }

    // Public methods for runtime control
    public void SetRotationSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
        _targetSpeed = newSpeed;
    }

    public void SetRotationEnabled(bool enabled)
    {
        rotateEnabled = enabled;
    }

    public void ReverseRotationDirection()
    {
        rotationSpeed *= -1f;
        _targetSpeed *= -1f;
    }

    public void RandomizeSpeed()
    {
        _targetSpeed = Random.Range(minRandomSpeed, maxRandomSpeed);
    }
}