using Photon.Pun;
using UnityEngine;

public class RightFlipperControl : MonoBehaviourPun
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float minHitStrength = 3000f;
    public float maxHitStrength = 20000f; // Maximum hit strength
    public float flipperDamper = 150f;
    public float smoothPressureSpeed = 5f; // Pressure smoothing speed
    private HingeJoint hinge;
    private bool isPressed = false;
    private float currentPressure = 0f; // Current pressure value
    private float targetPressure = 0f;  // Target pressure value
    private bool isDoublePoints = false;

    private JointSpring spring;
    private float lastPosition;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;

        spring = new JointSpring
        {
            damper = flipperDamper
        };

        lastPosition = restPosition;

        if (photonView.IsMine)
        {
            Debug.Log(gameObject.name + " is controlled by this client.");
        }
        else
        {
            Debug.Log(gameObject.name + " is controlled by another client.");
        }
    }

    public void OnButtonPress(float touchPressure)
    {
        isPressed = true;
        targetPressure = touchPressure; // Set target pressure
    }

    public void OnButtonRelease()
    {
        isPressed = false;
        targetPressure = 0f; // Reset pressure
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ControlFlipper();
        }
    }

    void ControlFlipper()
    {
        currentPressure = Mathf.Lerp(currentPressure, targetPressure, Time.deltaTime * smoothPressureSpeed);
        float springForce = Mathf.Clamp(minHitStrength + 15000 * currentPressure, 5000, 20000);

        spring.spring = springForce;
        spring.targetPosition = isPressed ? pressedPosition : restPosition;

        if (spring.targetPosition != lastPosition)
        {
            photonView.RPC("SyncFlipper", RpcTarget.All, spring.targetPosition);

            Debug.Log(gameObject.name + " Right Flipper: Position: " + spring.targetPosition);
            lastPosition = spring.targetPosition;
        }

        hinge.spring = spring;
        hinge.useLimits = true;
    }

    [PunRPC]
    void SyncRightFlipper(float targetPosition)
    {
        Debug.Log(gameObject.name + " received SyncFlipper RPC with position: " + targetPosition);

        spring.targetPosition = targetPosition;
        hinge.spring = spring;
        hinge.useLimits = true;
    }

    public void SetDoublePoints(bool isActive)
    {
        isDoublePoints = isActive;
        Debug.Log("Right Flipper Double Points Mode: " + isActive);
    }

    public bool IsDoublePointsActive()
    {
        return isDoublePoints;
    }
}