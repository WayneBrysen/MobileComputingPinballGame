using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private Camera southPlayerCamera;
    private Camera northPlayerCamera;
    private GameObject southController;
    private GameObject northController;

    void Start()
    {
        // �Զ����ҳ����е������
        southPlayerCamera = GameObject.FindWithTag("SouthCamera")?.GetComponent<Camera>();
        northPlayerCamera = GameObject.FindWithTag("NorthCamera")?.GetComponent<Camera>();

        // �Զ����ҳ����еĿ�����
        southController = GameObject.Find("southController");
        northController = GameObject.Find("northController");

        // ��ӡ������Ϣ��ȷ��������ȷ�ҵ�
        Debug.Log("South Camera: " + (southPlayerCamera != null ? "Found" : "Not Found"));
        Debug.Log("North Camera: " + (northPlayerCamera != null ? "Found" : "Not Found"));
        Debug.Log("South Controller: " + (southController != null ? "Found" : "Not Found"));
        Debug.Log("North Controller: " + (northController != null ? "Found" : "Not Found"));

        if (photonView.IsMine)
        {
            Debug.Log("���Ǳ��ؿͻ��˵� PlayerManager");
        }
        else
        {
            Debug.Log("���������ͻ��˵� PlayerManager");
        }

        // �ֶ����� OnJoinedRoom��ȷ����ʼ���߼�����ִ��
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("�Ѿ��ڷ����У��ֶ����� OnJoinedRoom()");
            OnJoinedRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ������ң������ϲ�������� flippers ����
            SetPlayerPosition(true); // true ��ʾ���ϲࣨ������
        }
        else
        {
            // ������ң����ñ���������� flippers ����
            SetPlayerPosition(false); // false ��ʾ�Ǳ��ࣨ�ͻ���
            TransferFlipperOwnership(northController);
        }
    }

    void TransferFlipperOwnership(GameObject controller)
    {
        var photonViews = controller.GetComponentsInChildren<PhotonView>();
        foreach (var photonView in photonViews)
        {
            if (PhotonNetwork.LocalPlayer != photonView.Owner)
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("����Ȩת��: " + photonView.ViewID + " ת����: " + PhotonNetwork.LocalPlayer.NickName);
            }
        }
    }

    // ������ҵ�λ�ú� flippers �Ŀ���
    void SetPlayerPosition(bool isSouthSide)
    {
        if (isSouthSide)
        {
            Debug.Log("�����ϲ��������������ñ�������������");
            southPlayerCamera.enabled = true; // �����ϲ������
            northPlayerCamera.enabled = false; // ���ñ��������

            SetFlippersControl(southController, true);  // �����ϲ� flippers �Ŀ���
            SetFlippersControl(northController, false); // ���ñ��� flippers �Ŀ���
        }
        else
        {
            Debug.Log("���ñ������������������ϲ�����������");
            southPlayerCamera.enabled = false; // �����ϲ������
            northPlayerCamera.enabled = true; // ���ñ��������

            SetFlippersControl(northController, true);  // ���ñ��� flippers �Ŀ���
            SetFlippersControl(southController, false); // �����ϲ� flippers �Ŀ���
        }
    }

    // ���û���� flippers �Ŀ���
    void SetFlippersControl(GameObject controller, bool isEnabled)
    {
        var leftFlipper = controller.transform.Find("LeftFlipper").GetComponent<FlipperScript>();
        var rightFlipper = controller.transform.Find("RightFlipper").GetComponent<FlipperScript>();

        if (leftFlipper != null)
        {
            leftFlipper.enabled = isEnabled;
        }

        if (rightFlipper != null)
        {
            rightFlipper.enabled = isEnabled;
        }
    }
}