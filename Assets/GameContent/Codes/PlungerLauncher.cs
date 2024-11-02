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
    private bool hasStartedMicrophone = false;
    private bool previousIsMineState = false;

    public GameObject playerBall;

    // Start the microphone and capture audio input

    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            StartMicrophone();
            hasStartedMicrophone = true;
        }
    }

    void Update()
    {
        if (photonView.IsMine && ballInContact && isListening)
        {
            float currentVolume = GetMicrophoneVolume();
            float amplifiedVolume = currentVolume * volumeMultiplier;

            if (amplifiedVolume > maxVolume)
            {
                maxVolume = amplifiedVolume;
            }

            if (amplifiedVolume > 0.1f)
            {
                thresholdContactTime += Time.deltaTime;
            }

            Debug.Log($"Amplified Microphone Volume: {amplifiedVolume}, Time above 0.1: {thresholdContactTime}");

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
            microphoneDevice = Microphone.devices[0]; // Use the first microphone device
            microphoneInput = Microphone.Start(microphoneDevice, true, 1, 96000);
            isListening = true;
            Debug.Log("microphone devices found!");
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
        if (playerBall != null)
        {
            Rigidbody ballRigidbody = playerBall.GetComponent<Rigidbody>();
            Vector3 launchDirection = (playerBall.transform.position - this.transform.position).normalized;
            float force = Mathf.Clamp(maxVolume, 0f, 1f) * 30f;

            // 判断当前客户端是否拥有球的所有权
            PhotonView ballPhotonView = playerBall.GetComponent<PhotonView>();
            if (ballPhotonView.Owner == PhotonNetwork.LocalPlayer)
            {
                // 如果当前客户端是所有者，直接弹射
                ballRigidbody.AddForce(launchDirection * force, ForceMode.Impulse);
                Debug.Log("Ball Launched by Owner with force: " + force + " in direction: " + launchDirection);
            }
            else
            {
                // 如果当前客户端不是所有者，通过RPC请求所有者弹射
                ballPhotonView.RPC("LaunchBallRPC", ballPhotonView.Owner, launchDirection, force);
                Debug.Log("LaunchBallRPC called to Owner");
            }
        }
        else
        {
            Debug.LogError("未引用到玩家自己的球！");
        }
    }

    [PunRPC]
    void LaunchBallRPC(Vector3 launchDirection, float force)
    {
        if (playerBall != null)
        {
            Rigidbody ballRigidbody = playerBall.GetComponent<Rigidbody>();
            ballRigidbody.AddForce(launchDirection * force, ForceMode.Impulse);
            Debug.Log("Ball Launched via RPC with force: " + force + " in direction: " + launchDirection);
        }
        else
        {
            Debug.LogError("未引用到玩家自己的球！");
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