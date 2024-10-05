using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleBounce : MonoBehaviour
{
    public float bounceForce = 2f; // ÿ���ϰ���ķ�����

    // ��С�������ϰ���ʱ����
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // ��ȡ��ײ�ķ��߷���
            Vector3 normal = collision.contacts[0].normal;

            // ��ȡС��ĵ�ǰ�ٶ�
            Vector3 incomingVelocity = ballRb.velocity;

            // ���㷴�䷽�򣬵�ֻ���� X �� Z ƽ���ڵķ��䣬���ı� Y ������ٶ�
            Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);
            reflectedVelocity.y = 0; // ���� Y ������ٶȣ�ֻ��ˮƽ���Ϸ���

            // Ӧ�÷�����
            ballRb.velocity = reflectedVelocity * bounceForce;
        }
    }
}
