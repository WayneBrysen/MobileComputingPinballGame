using UnityEngine;

public class OneWayGate : MonoBehaviour
{
    private Collider gateCollider;   // Reference to the gate's Collider

    void Start()
    {
        // Get the gate's Collider component
        gateCollider = GetComponent<Collider>();
        
        // Ensure the gate starts as a trigger
        gateCollider.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the "Ball" tag
        if (other.CompareTag("Ball"))
        {
            // Get the Rigidbody component of the ball
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();

            // Ensure the ball has a Rigidbody component
            if (ballRigidbody != null)
            {
                // Check the ball's z-axis velocity to determine the movement direction
                if (ballRigidbody.velocity.z > 0)
                {
                    // The ball is moving upwards, allow it to pass through
                    Debug.Log("Ball passed through the gate from below.");
                    
                    // Set the gate to be non-solid (trigger mode)
                    gateCollider.isTrigger = true;
                }
                else if (ballRigidbody.velocity.z < 0)
                {
                    // The ball is moving downwards, block it by setting the gate to solid
                    Debug.Log("Ball tried to pass from above, blocking it.");
                    
                    // Set the gate to be solid (non-trigger) to block the ball
                    gateCollider.isTrigger = false;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // When the ball exits, reset the gate to trigger mode
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball exited, gate reset to non-solid.");
            gateCollider.isTrigger = true;
        }
    }
}
