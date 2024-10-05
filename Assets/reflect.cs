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

                // Reflect the ball's velocity based on the trigger's normal direction
                Vector3 collisionNormal = GetCollisionNormal(other);
                Vector3 reflectedVelocity = Vector3.Reflect(currentVelocity, collisionNormal);

                // Apply the reflected velocity and adjust its magnitude using bounceFactor
                ballRigidbody.velocity = reflectedVelocity * bounceFactor;
            }
        }
    }

    // Custom method to get the collision normal direction
    Vector3 GetCollisionNormal(Collider other)
    {
        // You can calculate the normal direction based on the trigger's position and the ball's position
        // For example: return the transform's forward direction (can be adjusted as needed)
        return transform.forward; // Default to return the trigger's forward direction
    }
}
