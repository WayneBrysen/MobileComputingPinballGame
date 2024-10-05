using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    public float bounceForce = 1.5f;  // ����ϵ����ͨ�������� 1 �� 2 ֮��
    public float maxBounceSpeed = 10f;  // ���Ʒ����������ٶ�
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // ��ȡ��ײ��ķ��߷���
        Vector3 normal = collision.contacts[0].normal;

        // ��ȡС��ǰ���ٶ�
        Vector3 incomingVelocity = rb.velocity;

        // ���㷴���ٶ�
        Vector3 reflectedVelocity = Vector3.Reflect(incomingVelocity, normal);

        // ����һ���ķ���ϵ������������ȫ����
        Vector3 newVelocity = reflectedVelocity * bounceForce;

        // ���Ʒ����ٶȣ����ⵯ�ù���
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxBounceSpeed);

        // ����С����ٶ�
        rb.velocity = newVelocity;
    }
}
