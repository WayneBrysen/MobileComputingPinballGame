using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f;
    public int scoreValue;

    private GameManager gameManager; 
    private LeftFlipperControl leftFlipper;
    private RightFlipperControl rightFlipper;

    void Start()
    {
        // 获取场景中的ScoreManager
        gameManager = FindObjectOfType<GameManager>();

        // 获取左右Flipper的引用
        leftFlipper = FindObjectOfType<LeftFlipperControl>();
        rightFlipper = FindObjectOfType<RightFlipperControl>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody ballRb = collision.collider.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            // 获取碰撞的法线方向
            Vector3 normal = collision.contacts[0].normal;

            ballRb.AddForce(-normal * bounceForce, ForceMode.Impulse);

            // Debug the name of the tag
            Debug.Log("Collided with object tagged: " + this.gameObject.tag);

            // 检查 Flipper 的双倍积分状态
            if (leftFlipper != null && leftFlipper.IsDoublePointsActive() ||
                rightFlipper != null && rightFlipper.IsDoublePointsActive())
            {
                scoreValue *= 2;  // 如果双倍积分模式启用，则分数翻倍
            }
            // 增加分数
            gameManager.AddScore(scoreValue);
        }
    }
}