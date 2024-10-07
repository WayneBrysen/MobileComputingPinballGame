using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f; // 每个障碍物的反弹力
    public int scoreValue = 10;  // 每次碰撞增加的分数

    private GameManager gameManager;  // 引用ScoreManager

    void Start()
    {
        // 获取场景中的ScoreManager
        gameManager = FindObjectOfType<GameManager>();
    }

    // 当小球碰到障碍物时触发
    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // 获取碰撞的法线方向
            Vector3 normal = collision.contacts[0].normal;

            // 施加一次性反弹力，而不是直接改变速度
            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            // 增加分数
            gameManager.AddScore(scoreValue);
        }
    }
}