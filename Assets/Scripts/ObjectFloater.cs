using UnityEngine;

[DisallowMultipleComponent]
public class ObjectFloater : MonoBehaviour
{
    public enum FloatDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Backward,
        Custom
    }

    [Header("Float Settings")]
    [Tooltip("Enable/disable floating")]
    public bool floatEnabled = true;

    [Tooltip("Direction of floating movement")]
    public FloatDirection floatDirection = FloatDirection.Up;

    [Tooltip("Custom direction (only used if FloatDirection is Custom)")]
    public Vector3 customDirection = Vector3.up;

    [Tooltip("Float speed in units per second")]
    public float floatSpeed = 1f;

    [Tooltip("Distance to travel from start position")]
    public float floatDistance = 0.5f;

    [Tooltip("Type of float movement (simplified)")]
    public EaseType easeType = EaseType.SineInOut;

    [Tooltip("Should the movement ping-pong (go back and forth)")]
    public bool pingPong = true;

    [Tooltip("Time offset (0-1) for the animation")]
    [Range(0f, 1f)]
    public float timeOffset = 0f;

    [Tooltip("Randomize float parameters on start")]
    public bool randomizeOnStart = false;

    [Tooltip("Minimum speed when randomizing")]
    public float minRandomSpeed = 0.5f;

    [Tooltip("Maximum speed when randomizing")]
    public float maxRandomSpeed = 2f;

    [Tooltip("Minimum distance when randomizing")]
    public float minRandomDistance = 0.2f;

    [Tooltip("Maximum distance when randomizing")]
    public float maxRandomDistance = 1f;

    public enum EaseType
    {
        Linear,
        SineIn,
        SineOut,
        SineInOut,
        QuadIn,
        QuadOut,
        QuadInOut
    }

    private Vector3 _startPosition;
    private Vector3 _direction;
    private float _animationTime;
    private bool _reversing;

    private void Start()
    {
        _startPosition = transform.position;

        if (randomizeOnStart)
        {
            floatSpeed = Random.Range(minRandomSpeed, maxRandomSpeed);
            floatDistance = Random.Range(minRandomDistance, maxRandomDistance);
            timeOffset = Random.value;
        }

        SetDirection();
        _animationTime = timeOffset * (floatDistance / floatSpeed);
    }

    private void SetDirection()
    {
        switch (floatDirection)
        {
            case FloatDirection.Up:
                _direction = Vector3.up;
                break;
            case FloatDirection.Down:
                _direction = Vector3.down;
                break;
            case FloatDirection.Left:
                _direction = Vector3.left;
                break;
            case FloatDirection.Right:
                _direction = Vector3.right;
                break;
            case FloatDirection.Forward:
                _direction = Vector3.forward;
                break;
            case FloatDirection.Backward:
                _direction = Vector3.back;
                break;
            case FloatDirection.Custom:
                _direction = customDirection.normalized;
                break;
        }
    }

    private void Update()
    {
        if (!floatEnabled) return;

        // 更新动画时间
        _animationTime += Time.deltaTime * (_reversing ? -1f : 1f);

        float progress = _animationTime / (floatDistance / floatSpeed);

        // 处理ping-pong逻辑
        if (pingPong)
        {
            if (progress >= 1f)
            {
                progress = 1f;
                _reversing = true;
            }
            else if (progress <= 0f)
            {
                progress = 0f;
                _reversing = false;
            }
        }
        else
        {
            progress = Mathf.Clamp01(progress);
            if (progress >= 1f)
            {
                progress = 0f;
                _animationTime = 0f;
            }
        }

        // 应用缓动函数
        float easedProgress = ApplyEase(progress);

        // 计算新位置
        Vector3 newPosition = _startPosition + _direction * (floatDistance * easedProgress);
        transform.position = newPosition;
    }

    private float ApplyEase(float t)
    {
        switch (easeType)
        {
            case EaseType.Linear:
                return t;
            case EaseType.SineIn:
                return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            case EaseType.SineOut:
                return Mathf.Sin(t * Mathf.PI * 0.5f);
            case EaseType.SineInOut:
                return -0.5f * (Mathf.Cos(Mathf.PI * t) - 1f);
            case EaseType.QuadIn:
                return t * t;
            case EaseType.QuadOut:
                return t * (2f - t);
            case EaseType.QuadInOut:
                return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
            default:
                return t;
        }
    }

    public void SetFloatEnabled(bool enabled)
    {
        floatEnabled = enabled;
        if (enabled)
        {
            _startPosition = transform.position - _direction * (floatDistance * ApplyEase(_animationTime / (floatDistance / floatSpeed)));
        }
    }

    public void SetFloatDirection(FloatDirection newDirection)
    {
        if (floatDirection == newDirection) return;

        floatDirection = newDirection;
        SetDirection();
        _startPosition = transform.position - _direction * (floatDistance * ApplyEase(_animationTime / (floatDistance / floatSpeed)));
    }

    public void ResetFloat()
    {
        _animationTime = 0f;
        _reversing = false;
        _startPosition = transform.position;
    }
}