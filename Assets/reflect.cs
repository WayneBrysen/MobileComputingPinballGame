using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBounce : MonoBehaviour
{
    public float bounceFactor = 0.9f; // Bounce coefficient to adjust the rebound force

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the ball
        if (other.CompareTag("Ball")) // Assuming the ball's tag is "Ball"
        {
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

            if (ballRigidbody != null)
            {
                // Get the current velocity direction of the ball
                Vector3 currentVelocity = ballRigidbody.velocity;

                // Calculate the true collision normal based on the closest point between the ball and the trigger
                Vector3 collisionNormal = GetCollisionNormal(other);

                // Reflect the ball's velocity based on the calculated normal direction
                Vector3 reflectedVelocity = Vector3.Reflect(currentVelocity, collisionNormal);

                // Apply the reflected velocity and adjust its magnitude using bounceFactor
                ballRigidbody.velocity = reflectedVelocity * bounceFactor;
            }
        }
    }

    // Calculate the collision normal direction based on the closest point
    Vector3 GetCollisionNormal(Collider other)
    {
        // Get the closest point on the trigger to the ball
        Vector3 closestPoint = other.ClosestPoint(transform.position);

        // Calculate the normal direction from the closest point to the trigger's position
        Vector3 normalDirection = (transform.position - closestPoint).normalized;

        // Return the calculated normal direction
        return normalDirection;
    }
}