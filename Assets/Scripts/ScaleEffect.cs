using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    public float scaleStrength = 0.1f; // 缩放强度（相对于原始尺寸）
    public float scaleSpeed = 1.0f;    // 缩放速度

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale; // 记录原始尺寸
    }

    void Update()
    {
        // 用正弦函数计算缩放比例（在 1.0 附近波动）
        float scaleFactor = 1.0f + Mathf.Sin(Time.time * scaleSpeed) * scaleStrength;
        transform.localScale = originalScale * scaleFactor;
    }
}
