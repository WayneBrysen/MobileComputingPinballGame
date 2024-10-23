using UnityEngine;
using Photon.Pun;

public class PlungerLauncher : MonoBehaviourPun
{
    private string microphoneDevice; // Name of the microphone device
    private AudioClip microphoneInput; // Store microphone input data
    private bool isListening = false; // Check if the microphone is actively listening
    private float volumeMultiplier = 30f; // Multiplier to amplify the microphone volume
    private bool ballInContact = false; // Check if the ball is in contact with the plunger
    private float maxVolume = 0f; // Store the maximum volume detected
    private float contactTime = 0f; // Time the ball has been in contact with the plunger
    private float thresholdContactTime = 0f; // Time the sound has been above the threshold (0.1)
    private float requiredContactTime = 1f; // The required time for the sound to trigger the launch
    private GameObject ball; // Reference to the ball (will be found dynamically)
    private bool hasStartedMicrophone = false; // Track whether the microphone has been started
    private bool previousIsMineState = false; // Track the previous ownership state

    void Start()
    {
        // Check ownership status at the start
        if (photonView.IsMine)
        {
            StartMicrophone();
            previousIsMineState = true;
            hasStartedMicrophone = true; // Mark that the microphone has been started
        }
    }

    void Update()
    {
        // Check for ownership changes
        if (photonView.IsMine && !previousIsMineState)
        {
            // Ownership transferred to local client
            Debug.Log("Ownership transferred to local client.");

            if (!hasStartedMicrophone) // Ensure the microphone starts only once
            {
                StartMicrophone();
                hasStartedMicrophone = true;
            }

            previousIsMineState = true; // Update ownership state
        }

        // Only execute this logic if the object belongs to the local player and ball is in contact
        if (photonView.IsMine && ballInContact && isListening)
        {
            float currentVolume = GetMicrophoneVolume();
            float amplifiedVolume = currentVolume * volumeMultiplier;

            // Track the maximum volume
            if (amplifiedVolume > maxVolume)
            {
                maxVolume = amplifiedVolume;
            }

            // Accumulate time when volume exceeds the threshold
            if (amplifiedVolume > 0.1f)
            {
                thresholdContactTime += Time.deltaTime;
            }

            // Log information for debugging
            Debug.Log($"Amplified Microphone Volume: {amplifiedVolume}, Time above 0.1: {thresholdContactTime}");

            // Trigger the launch if threshold time is met
            if (thresholdContactTime >= requiredContactTime)
            {
                LaunchBall();
                ResetContact();
            }
        }
    }

    void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0]; // Use the first available microphone
            microphoneInput = Microphone.Start(microphoneDevice, true, 1, 96000);
            isListening = true;
            Debug.Log("Microphone started!");
        }
        else
        {
            Debug.LogError("No microphone devices found!");
        }
    }

    // Stop the microphone input
    void StopMicrophone()
    {
        if (isListening)
        {
            Microphone.End(microphoneDevice);
            isListening = false;
        }
    }

    // Calculate the current volume from the microphone input
    float GetMicrophoneVolume()
    {
        float[] samples = new float[512]; // Create a buffer for the samples
        microphoneInput.GetData(samples, 0); // Get the microphone data
        float sum = 0f;

        foreach (float sample in samples)
        {
            sum += sample * sample; // Calculate RMS (Root Mean Square) value for the sound
        }

        return Mathf.Sqrt(sum / samples.Length); // Return the RMS value
    }

    // Function to launch the ball based on the maximum detected volume
    private void LaunchBall()
    {
        ball = GameObject.FindWithTag("Ball"); // Find the ball by its tag
        if (ball != null)
        {
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            Vector3 launchDirection = (ball.transform.position - this.transform.position).normalized;
            float force = Mathf.Clamp(maxVolume, 0f, 1f) * 3f; // Apply a force scaled by maxVolume
            ballRigidbody.AddForce(launchDirection * force, ForceMode.Impulse);

            Debug.Log("Ball Launched with force: " + force + " in direction: " + launchDirection);
        }
        else
        {
            Debug.LogError("No ball found with the tag 'Ball'");
        }
    }

    // Reset the contact time and maximum volume
    private void ResetContact()
    {
        contactTime = 0f;
        thresholdContactTime = 0f;
        maxVolume = 0f;
    }

    // Detect if the ball is in contact with the plunger using Collision
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = true; // Ball is in contact with the plunger
            ResetContact(); // Reset the variables when contact begins
        }
    }

    // Detect when the ball leaves the contact with the plunger using Collision
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = false; // Ball is no longer in contact with the plunger
            ResetContact(); // Reset contact state when the ball leaves
        }
    }
}
