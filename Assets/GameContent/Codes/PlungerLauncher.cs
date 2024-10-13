using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Import EventSystems for EventTrigger

public class BallLauncher : MonoBehaviour
{
    public Button pushButton; // Reference to the PUSH button
    public Slider powerSlider; // Reference to the power slider
    public Transform plunger; // Reference to the plunger transform
    public Rigidbody ball; // Reference to the ball's rigidbody

    private float pressTime; // Duration of button press
    private float maxPressTime = 5.0f; // Max press duration in seconds
    private bool isPressing = false; // Check if the button is being pressed
    private float launchForce = 1.0f; // Launch force multiplier (1/10 of original value)
    private bool ballInContact = false; // Check if the ball is in contact with the plunger

    void Start()
    {
        // Initialize the slider value
        powerSlider.value = 0;

        // Add EventTrigger event listeners to the button
        EventTrigger eventTrigger = pushButton.gameObject.AddComponent<EventTrigger>();

        // Create PointerDown event listener, triggered when the button is pressed down
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDownEntry.callback.AddListener((data) => { OnButtonPress(); });
        eventTrigger.triggers.Add(pointerDownEntry);

        // Create PointerUp event listener, triggered when the button is released
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUpEntry.callback.AddListener((data) => { OnButtonRelease(); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    // Called when the button is pressed down
    public void OnButtonPress()
    {
        // Check if the ball is in contact with the plunger
        if (ballInContact)
        {
            isPressing = true;
            pressTime = 0f; // Reset the press time
            powerSlider.value = 0; // Reset the slider value
        }
    }

    void Update()
    {
        // Check if the button is being pressed and the ball is in contact with the plunger
        if (isPressing && ballInContact)
        {
            // Accumulate press time
            pressTime += Time.deltaTime;

            // Limit the press time to maxPressTime
            if (pressTime > maxPressTime)
            {
                pressTime = maxPressTime; // Cap the press time
                isPressing = false; // Stop pressing
                LaunchBall(true); // Launch the ball with random force if timeout
            }

            // Update the slider value based on press time
            powerSlider.value = pressTime / maxPressTime;
        }
    }

    // Called when the button is released
    public void OnButtonRelease()
    {
        if (isPressing && ballInContact)
        {
            isPressing = false; // Stop pressing
            LaunchBall(); // Launch the ball
        }
    }

    // Function to launch the ball along the Z-axis
    // If `useRandomForce` is true, use a random force value between 0 and maxPressTime
    private void LaunchBall(bool useRandomForce = false)
    {
        // Determine the force to be applied based on press time or randomly if specified
        float force;

        if (useRandomForce)
        {
            // Generate a random press time between 0 and maxPressTime
            float randomPressTime = Random.Range(0f, maxPressTime);
            force = launchForce * (randomPressTime / maxPressTime);
        }
        else
        {
            force = launchForce * (pressTime / maxPressTime);
        }

        // Apply force along the world z-axis direction
        ball.AddForce(Vector3.forward * force, ForceMode.Impulse); // Vector3.forward is (0, 0, 1)

        // Reset slider and press time
        powerSlider.value = 0;
        pressTime = 0f;
    }

    // Detect if the ball is in contact with the plunger using Collision
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object entering the collision is the ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = true; // Ball is in contact with the plunger
        }
    }

    // Detect when the ball leaves the contact with the plunger using Collision
    private void OnCollisionExit(Collision collision)
    {
        // Check if the object exiting the collision is the ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = false; // Ball is no longer in contact with the plunger
        }
    }
}
