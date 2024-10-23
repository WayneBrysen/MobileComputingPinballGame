using Photon.Pun;
using UnityEngine;

public class FlipperScript : MonoBehaviourPun
{
    public float restPosition = 0f;
    public float pressedPosition = 45f;
    public float hitStrength = 10000f;
    public float flipperDamper = 150f;
    private HingeJoint hinge;
    public string inputName;

    private JointSpring spring;
    private float lastPosition;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        hinge.useSpring = true;

        spring = new JointSpring();
        spring.spring = hitStrength;
        spring.damper = flipperDamper;

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

    void Update()
    {
        if (photonView.IsMine)
        {
            ControlFlipper();
        }
    }

    void ControlFlipper()
    {
        float currentPosition = Input.GetAxis(inputName) == 1 ? pressedPosition : restPosition;

        if (currentPosition != lastPosition)
        {
            photonView.RPC("SyncFlipper", RpcTarget.All, currentPosition);

            Debug.Log(gameObject.name + " Flipper input detected: " + inputName + " - Position: " + currentPosition);
            lastPosition = currentPosition;
        }
    }

    // 使用 RPC 在所有客户端同步 flipper 的状态
    [PunRPC]
    void SyncFlipper(float targetPosition)
    {
        // 调试日志，检查 RPC 是否被所有客户端接收到
        Debug.Log(gameObject.name + " received SyncFlipper RPC with position: " + targetPosition);

        spring.targetPosition = targetPosition;  // 设置目标位置
        hinge.spring = spring;  // 应用弹簧配置
        hinge.useLimits = true;  // 使用关节的限制
    }
}