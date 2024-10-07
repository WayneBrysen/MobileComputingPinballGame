using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    public float bounceForce = 1.5f;  // 反弹系数，通常设置在 1 到 2 之间
    public float maxBounceSpeed = 10f;  // 限制反弹后的最大速度
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // 获取碰撞点的法线方向
        Vector3 normal = collision.contacts[0].normal;

        // 获取小球当前的速度
        Vector3 incomingVelocity = rb.velocity;

        // 计算反射速度
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);

        // 保持一定的反弹系数，而不是完全反射
        Vector3 newVelocity = reflectedVelocity * bounceForce;

        // 限制反弹速度，避免弹得过高
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxBounceSpeed);

        // 更新小球的速度
        rb.velocity = newVelocity;
    }
}
