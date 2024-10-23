using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBounce : MonoBehaviour
{
    public float bounceForce = 5f; // ÿ���ϰ���ķ�����
    public int scoreValue;  // ÿ����ײ���ӵķ���

    private GameManager gameManager;  // ����ScoreManager
    private LeftFlipperControl leftFlipper;
    private RightFlipperControl rightFlipper;

    void Start()
    {
        // ��ȡ�����е�ScoreManager
        gameManager = FindObjectOfType<GameManager>();

        // ��ȡ����Flipper������
        leftFlipper = FindObjectOfType<LeftFlipperControl>();
        rightFlipper = FindObjectOfType<RightFlipperControl>();
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

            // ��� Flipper ��˫������״̬
            if (leftFlipper != null && leftFlipper.IsDoublePointsActive() ||
                rightFlipper != null && rightFlipper.IsDoublePointsActive())
            {
                scoreValue *= 2;  // ���˫������ģʽ���ã����������
            }
            // ���ӷ���
            gameManager.AddScore(scoreValue);
        }
    }
}