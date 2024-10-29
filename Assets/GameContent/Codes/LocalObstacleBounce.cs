using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LocalObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f;

    private Volume volume;
    private Bloom bloomEffect;

    void Start()
    {
        volume = FindObjectOfType<Volume>();

        if (volume != null)
        {
            if (volume.profile.TryGet(out bloomEffect))
            {
                Debug.Log("Bloom effect found and ready to use.");
            }
            else
            {
                Debug.LogWarning("Bloom effect not found in the Volume profile.");
            }
        }
        else
        {
            Debug.LogError("Volume component not found in the scene.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            Vector3 normal = collision.contacts[0].normal;
            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            // 检查碰撞的小球颜色
            if (collision.collider.CompareTag("BlueBall"))
            {
                ChangeBloomTint(HexToColor("4E5EFF"));  // 使用HexToColor函数
            }
            else if (collision.collider.CompareTag("RedBall"))
            {
                ChangeBloomTint(HexToColor("E25AFF"));
            }
        }
    }

    void ChangeBloomTint(Color tintColor)
    {
        if (bloomEffect != null)
        {
            bloomEffect.tint.value = tintColor;
        }
    }

    // 辅助方法：将十六进制字符串转换为Color
    Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString($"#{hex}", out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Invalid hex color format.");
            return Color.white;  // 返回白色作为默认值
        }
    }
}