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
    private bool hasStartedMicrophone = false; // 用于跟踪麦克风是否已启动
    private bool previousIsMineState = false; // 用于跟踪所有权的前一个状态

    // Start the microphone and capture audio input

    void Start()
    {
        // 初次检查所有权状态
        if (photonView.IsMine)
        {
            StartMicrophone();
            previousIsMineState = true;
            hasStartedMicrophone = true; // 标记麦克风已启动
        }
    }

    void Update()
    {
        // 检查所有权状态是否发生变化
        if (photonView.IsMine && !previousIsMineState)
        {
            // 如果所有权变成了本地客户端
            Debug.Log("Ownership transferred to local client.");

            if (!hasStartedMicrophone) // 确保麦克风只启动一次
            {
                StartMicrophone();
                hasStartedMicrophone = true;
            }

            previousIsMineState = true; // 更新状态
        }

        // 只在本地客户端且球接触时进行音量检测
        if (photonView.IsMine && ballInContact && isListening)
        {
            float currentVolume = GetMicrophoneVolume();
            float amplifiedVolume = currentVolume * volumeMultiplier;

            // 记录最大音量
            if (amplifiedVolume > maxVolume)
            {
                maxVolume = amplifiedVolume;
            }

            // 只在音量超过阈值时累加时间
            if (amplifiedVolume > 0.1f)
            {
                thresholdContactTime += Time.deltaTime;
            }

            // 调试信息
            Debug.Log($"Amplified Microphone Volume: {amplifiedVolume}, Time above 0.1: {thresholdContactTime}");

            // 如果累加时间超过阈值且音量足够大，则发射球
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
        ball = GameObject.FindWithTag("Ball");
        if (ball != null)
        {
            Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
            Vector3 launchDirection = (ball.transform.position - this.transform.position).normalized;
            float force = Mathf.Clamp(maxVolume, 0f, 1f) * 3f; // 3f is the maximum force
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