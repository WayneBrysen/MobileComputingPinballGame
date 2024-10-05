using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleBounce : MonoBehaviour
{
    public float bounceForce = 2f; // 每个障碍物的反弹力

    // 当小球碰到障碍物时触发
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // 获取碰撞的法线方向
            Vector3 normal = collision.contacts[0].normal;

            // 获取小球的当前速度
            Vector3 incomingVelocity = ballRb.velocity;

            // 计算反射方向，但只保留 X 和 Z 平面内的反射，不改变 Y 方向的速度
            Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);
            reflectedVelocity.y = 0; // 忽略 Y 方向的速度，只在水平面上反弹

            // 应用反弹力
            ballRb.velocity = reflectedVelocity * bounceForce;
        }
    }
}
