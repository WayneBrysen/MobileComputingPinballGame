using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MonoBehaviour
{
    public Vector3 moveDirection = new Vector3(-1, 0, 0); // 设置移动方向（例如沿X轴正方向）
    public float moveSpeed = 5f; // 移动速度

    void Update()
    {
        // 手动更新球体的位置，使其在特定方向上移动
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 当球体与其他物体发生碰撞时，打印日志
        Debug.Log("Ball collided with: " + collision.gameObject.name);

        // 碰撞后改变方向，模拟反弹效果
        moveDirection = Vector3.Reflect(moveDirection, collision.contacts[0].normal);
    }
}
