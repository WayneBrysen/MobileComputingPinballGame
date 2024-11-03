using Photon.Pun;
using UnityEngine;

public class FlipperTouchController : MonoBehaviourPun
{
    // References to the left and right flipper controllers
    public LeftFlipperControl leftFlipper;
    public RightFlipperControl rightFlipper;

    public AudioSource audioSource; // Audio source for flipper sound
    //public float maxVolume = 1.0f; // Maximum volume
    //public float smoothSpeed = 5.0f; // Smooth transition speed
    //private float targetVolume = 0f; // Target volume
    //private float currentVolume = 0f; // Current volume

    public float minPitch = 1.0f;
    public float maxPitch = 1.5f;
    private float targetPitch = 1f;
    private float currentPitch = 1f;
    public float smoothSpeed = 5.0f; // Smooth transition speed

    private Color normalColor = Color.black; // Normal flipper color
    public Color highlightColor = Color.red; // Flipper color during double points mode

    private Renderer leftFlipperRenderer;
    private Renderer rightFlipperRenderer;

    private float volumeTimer = 0f; // Timer for volume threshold
    private float pitchThreshold = 1.4f;
    //private float volumeThreshold = 0.9f; // Volume threshold
    private float timeThreshold = 5f; // Time threshold for double points mode
    private float doublePointsTimer = 0f; // Double points mode timer
    private bool isDoublePointsActive = false; // Double points mode status

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("Missing AudioSource component.");
                enabled = false; // Disable script to prevent further errors
                return;
            }
        }

        leftFlipperRenderer = leftFlipper.GetComponent<Renderer>();
        rightFlipperRenderer = rightFlipper.GetComponent<Renderer>();

        // Initialize flipper colors to normal
        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;

        //// Set initial audio volume to 0
        //audioSource.volume = 0f;

        audioSource.pitch = minPitch;
        audioSource.Stop();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleTouchInput();
        }

        SmoothVolumeControl();

        if (photonView.IsMine)
        {
            CheckDoublePointsMode();
        }
    }

    void HandleTouchInput()
    {
        bool isTouching = false;

        // Detect touch input
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                float pressure = Mathf.Clamp(touch.pressure, 0f, 1f);

                targetPitch = Mathf.Lerp(minPitch, maxPitch, pressure);
                currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);
                audioSource.pitch = currentPitch;

                Debug.Log("Pressure: " + pressure + ", Pitch" + audioSource.pitch);

                //targetVolume = pressure >= volumeThreshold ? Mathf.Lerp(0f, maxVolume, pressure) : 0f;
                //currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
                //audioSource.volume = currentVolume;

                //Debug.Log("Pressure: " + pressure + ", Volume: " + audioSource.volume);

                // Left flipper control
                if (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    isTouching = true;
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Left flipper: touch begin with pressure: " + pressure);
                        leftFlipper.OnButtonPress(pressure);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Left flipper: touch end");
                        leftFlipper.OnButtonRelease();
                    }
                }

                // Right flipper control
                if (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height / 2)
                {
                    isTouching = true;
                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                    {
                        Debug.Log("Right flipper: touch begin with pressure: " + pressure);
                        rightFlipper.OnButtonPress(pressure);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        Debug.Log("Right flipper: touch end");
                        rightFlipper.OnButtonRelease();
                    }
                }
            }
        }

        if(isTouching)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            //targetVolume = 0f;
            //currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
            //audioSource.volume = currentVolume;
            targetPitch = minPitch;
            currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);
            audioSource.pitch = currentPitch;
        }
    }

    void SmoothVolumeControl()
    {
        //currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * smoothSpeed);
        //audioSource.volume = currentVolume;
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * smoothSpeed);
        audioSource.pitch = currentPitch;
    }

    void CheckDoublePointsMode()
    {
        // Start timer if pitch reaches threshold and double points mode is not active
        if (audioSource.pitch >= pitchThreshold && !isDoublePointsActive)
        {
            volumeTimer += Time.deltaTime;

            if (volumeTimer >= timeThreshold)
            {
                photonView.RPC("ActivateDoublePointsMode", RpcTarget.All);
            }
        }
        else if (audioSource.pitch < pitchThreshold && !isDoublePointsActive)
        {
            volumeTimer = 0f;
        }

        // Timer for double points mode duration
        if (isDoublePointsActive)
        {
            doublePointsTimer += Time.deltaTime;

            if (doublePointsTimer >= timeThreshold)
            {
                photonView.RPC("DeactivateDoublePointsMode", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void ActivateDoublePointsMode()
    {
        isDoublePointsActive = true;
        doublePointsTimer = 0f;

        leftFlipperRenderer.material.color = highlightColor;
        rightFlipperRenderer.material.color = highlightColor;

        leftFlipper.SetDoublePoints(true);
        rightFlipper.SetDoublePoints(true);

        Debug.Log("Double Points Mode Activated");
    }

    [PunRPC]
    void DeactivateDoublePointsMode()
    {
        isDoublePointsActive = false;
        volumeTimer = 0f;

        leftFlipperRenderer.material.color = normalColor;
        rightFlipperRenderer.material.color = normalColor;

        leftFlipper.SetDoublePoints(false);
        rightFlipper.SetDoublePoints(false);

        Debug.Log("Double Points Mode Deactivated");
    }
}
