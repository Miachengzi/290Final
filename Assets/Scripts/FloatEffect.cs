using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    public float floatStrength = 1.0f; // 漂浮的强度
    public float floatSpeed = 1.0f; // 浮动的速度

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition; // 记录原始位置
    }

    void Update()
    {
        // 根据时间和正弦函数计算新的高度
        float newY = originalPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
    }
}