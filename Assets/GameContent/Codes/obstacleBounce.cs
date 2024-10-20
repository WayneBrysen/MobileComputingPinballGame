using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f; // 每个障碍物的反弹力
    public int scoreValue;  // 每次碰撞增加的分数

    private GameManager gameManager;  // 引用ScoreManager
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

            // Debug the name of the tag
            Debug.Log("Collided with object tagged: " + this.gameObject.tag);

            // Check the tag of obstacle which is being bounced
            if (this.gameObject.tag == "ObstacleOne")
            {
                scoreValue = 50;  // ObstacleOne Score
            }
            else if (this.gameObject.tag == "ObstacleTwo")
            {
                scoreValue = 10;  // ObstacleTwo Score
            }
            else if (this.gameObject.tag == "ObstacleThree")
            {
                scoreValue = 5;   // ObstacleThree Score
            }else
            {
                scoreValue = 1; // ObstacleFour Score
            }

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