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

        lastPosition = restPosition;  // ��ʼ��Ϊ��ֹ״̬

        // ������־����� PhotonView ������Ȩ
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
        // ֻ�ڱ��ؿͻ���ִ�а�������
        if (photonView.IsMine)
        {
            ControlFlipper();
        }
    }

    void ControlFlipper()
    {
        // ��ȡ��ǰ����״̬
        float currentPosition = Input.GetAxis(inputName) == 1 ? pressedPosition : restPosition;

        // ֻ�е�����״̬�����ı�ʱ�Ž��� RPC ͬ��
        if (currentPosition != lastPosition)
        {
            // ͬ�� flipper �Ķ��������пͻ���
            photonView.RPC("SyncFlipper", RpcTarget.All, currentPosition);

            // ������־����鰴�������ͬ��
            Debug.Log(gameObject.name + " Flipper input detected: " + inputName + " - Position: " + currentPosition);
            lastPosition = currentPosition;  // ������һ�ε�״̬
        }
    }

    // ʹ�� RPC �����пͻ���ͬ�� flipper ��״̬
    [PunRPC]
    void SyncFlipper(float targetPosition)
    {
        // ������־����� RPC �Ƿ����пͻ��˽��յ�
        Debug.Log(gameObject.name + " received SyncFlipper RPC with position: " + targetPosition);

        spring.targetPosition = targetPosition;  // ����Ŀ��λ��
        hinge.spring = spring;  // Ӧ�õ�������
        hinge.useLimits = true;  // ʹ�ùؽڵ�����
    }
}