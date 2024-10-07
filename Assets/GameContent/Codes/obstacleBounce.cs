using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f; // ÿ���ϰ���ķ�����
    public int scoreValue = 10;  // ÿ����ײ���ӵķ���

    private GameManager gameManager;  // ����ScoreManager

    void Start()
    {
        // ��ȡ�����е�ScoreManager
        gameManager = FindObjectOfType<GameManager>();
    }

    // ��С�������ϰ���ʱ����
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // ��ȡ��ײ�ķ��߷���
            Vector3 normal = collision.contacts[0].normal;

            // ʩ��һ���Է�������������ֱ�Ӹı��ٶ�
            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            // ���ӷ���
            gameManager.AddScore(scoreValue);
        }
    }
}