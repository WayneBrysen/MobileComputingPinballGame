using UnityEngine;
using Photon.Pun;

public class PlungerLauncher1 : MonoBehaviourPun
{
    private string microphoneDevice; // Name of the microphone device
    private AudioClip microphoneInput; // Store microphone input data
    private bool isListening = false; // Check if the microphone is actively listening
    public float volumeMultiplier = 30f; // Multiplier to amplify the microphone volume
    private bool ballInContact = false; // Check if the ball is in contact with the plunger
    private float maxVolume = 0f; // Store the maximum volume detected
    private float contactTime = 0f; // Time the ball has been in contact with the plunger
    private float thresholdContactTime = 0f; // Time the sound has been above the threshold (0.1)
    private float requiredContactTime = 1f; // The required time for the sound to trigger the launch
    private GameObject ball; // Reference to the ball (will be found dynamically)

    void Start()
    {
        StartMicrophone(); // Start capturing audio from the microphone
    }

    // Start the microphone and capture audio input
    void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0]; // Use the first microphone device
            // Start the microphone with a buffer length of 1 second and a high sample rate
            microphoneInput = Microphone.Start(microphoneDevice, true, 1, 96000); 
            isListening = true;
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

        // Calculate RMS (Root Mean Square) value for the sound
        foreach (float sample in samples)
        {
            sum += sample * sample;
        }

        return Mathf.Sqrt(sum / samples.Length); // Return the RMS value
    }

    void Update()
    {
        if (ballInContact && isListening)
        {
            // Get the current microphone volume
            float currentVolume = GetMicrophoneVolume();
            float amplifiedVolume = currentVolume * volumeMultiplier;

            // Track the maximum volume during the contact
            if (amplifiedVolume > maxVolume)
            {
                maxVolume = amplifiedVolume;
            }

            // Only count volumes above the threshold (0.1)
            if (amplifiedVolume > 0.1f)
            {
                thresholdContactTime += Time.deltaTime; // Accumulate time when volume is above the threshold
            }

            // Debugging: Print current volume and threshold time
            Debug.Log($"Amplified Microphone Volume: {amplifiedVolume}, Time above 0.1: {thresholdContactTime}");

            // If thresholdContactTime exceeds required time (1 second) and we have a significant volume, launch the ball
            if (thresholdContactTime >= requiredContactTime)
            {
                LaunchBall(); // Launch the ball
                ResetContact(); // Reset the contact and volume tracking
            }
        }
    }

    // Function to launch the ball based on the maximum detected volume
    private void LaunchBall()
    {
        // Find the ball by its tag
        ball = GameObject.FindWithTag("Ball");
        if (ball != null)
        {
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

            // Calculate the direction from the plunger (this object) to the ball
            Vector3 launchDirection = (ball.transform.position - this.transform.position).normalized;

            // Clamp maxVolume between 0 and 1 to ensure it fits within the force range
            float force = Mathf.Clamp(maxVolume, 0f, 1f) * 3f; // 3f is the maximum force

            // Apply the force in the calculated direction
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
        thresholdContactTime = 0f; // Reset the time the volume has been above the threshold
        maxVolume = 0f;
    }

    // Detect if the ball is in contact with the plunger using Collision
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object entering the collision is the ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = true; // Ball is in contact with the plunger
            ResetContact(); // Reset the variables when contact begins
        }
    }

    // Detect when the ball leaves the contact with the plunger using Collision
    private void OnCollisionExit(Collision collision)
    {
        // Check if the object exiting the collision is the ball
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballInContact = false; // Ball is no longer in contact with the plunger
            ResetContact(); // Reset contact state when the ball leaves
        }
    }
}