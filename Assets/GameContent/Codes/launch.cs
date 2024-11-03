using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMover : MonoBehaviour
{
    public Vector3 moveDirection; 
    public float moveSpeed = 5f;

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Ball collided with: " + collision.gameObject.name);

        moveDirection = Vector3.Reflect(moveDirection, collision.contacts[0].normal);
    }
}
