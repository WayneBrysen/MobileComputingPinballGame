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
    private bool hasStartedMicrophone = false; // ���ڸ�����˷��Ƿ�������
    private bool previousIsMineState = false; // ���ڸ�������Ȩ��ǰһ��״̬

    // Start the microphone and capture audio input

    void Start()
    {
        // ���μ������Ȩ״̬
        if (photonView.IsMine)
        {
            StartMicrophone();
            previousIsMineState = true;
            hasStartedMicrophone = true; // �����˷�������
        }
    }

    void Update()
    {
        // �������Ȩ״̬�Ƿ����仯
        if (photonView.IsMine && !previousIsMineState)
        {
            // �������Ȩ����˱��ؿͻ���
            Debug.Log("Ownership transferred to local client.");

            if (!hasStartedMicrophone) // ȷ����˷�ֻ����һ��
            {
                StartMicrophone();
                hasStartedMicrophone = true;
            }

            previousIsMineState = true; // ����״̬
        }
        else if (!photonView.IsMine && previousIsMineState)
        {
            // �������Ȩ�ӱ��ؿͻ���ת�Ƴ�ȥ
            Debug.Log("Ownership transferred away from local client.");
            StopMicrophone();
            hasStartedMicrophone = false; // ���ñ�־
            previousIsMineState = false; // ����״̬
        }

        // ֻ�ڱ��ؿͻ�������Ӵ�ʱ�����������
        if (photonView.IsMine && ballInContact && isListening)
        {
            float currentVolume = GetMicrophoneVolume();
            float amplifiedVolume = currentVolume * volumeMultiplier;

            // ��¼�������
            if (amplifiedVolume > maxVolume)
            {
                maxVolume = amplifiedVolume;
            }

            // ֻ������������ֵʱ�ۼ�ʱ��
            if (amplifiedVolume > 0.1f)
            {
                thresholdContactTime += Time.deltaTime;
            }

            // ������Ϣ
            Debug.Log($"Amplified Microphone Volume: {amplifiedVolume}, Time above 0.1: {thresholdContactTime}");

            // ����ۼ�ʱ�䳬����ֵ�������㹻��������
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
            Debug.LogError("microphone devices found!");
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