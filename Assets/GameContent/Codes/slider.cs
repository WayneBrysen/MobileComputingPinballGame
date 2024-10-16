using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneVolumeDisplay : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the slider that will display microphone volume
    private string microphoneDevice; // Name of the microphone device
    private AudioClip microphoneInput; // Store microphone input data
    private bool isListening = false; // Check if the microphone is actively listening
    public float volumeMultiplier = 30f; // Multiplier to amplify the microphone volume

    void Start()
    {
        // Initialize the slider if it exists
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f; // Minimum volume is 0
            volumeSlider.maxValue = 1f; // Maximum volume is 1
            volumeSlider.value = 0f; // Start with slider value at 0
        }

        StartMicrophone(); // Start capturing audio from the microphone
    }

    // Start the microphone and capture audio input
    void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            // Print all detected microphone devices for debugging
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Detected Microphone: " + device);
            }

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
        if (isListening)
        {
            // Get the current microphone volume
            float currentVolume = GetMicrophoneVolume();

            // Apply the volume multiplier and clamp it between 0 and 1
            float amplifiedVolume = Mathf.Clamp(currentVolume * volumeMultiplier, 0f, 1f);

            // Update the slider to reflect the amplified volume
            if (volumeSlider != null)
            {
                volumeSlider.value = amplifiedVolume;
            }

            // Optional: Display the amplified volume value in the console (for debugging)
            Debug.Log("Amplified Microphone Volume: " + amplifiedVolume);
        }
    }
}
