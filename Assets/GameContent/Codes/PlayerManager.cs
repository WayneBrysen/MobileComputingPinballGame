using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    private Camera southPlayerCamera;
    private Camera northPlayerCamera;
    private GameObject southController;
    private GameObject northController;

    private Vector3 p1BallPosition;
    private Vector3 p2BallPosition;

    private bool hasSpawnedBall = false;

    void Start()
    {
        // �Զ����ҳ����е������
        southPlayerCamera = GameObject.FindWithTag("SouthCamera")?.GetComponent<Camera>();
        northPlayerCamera = GameObject.FindWithTag("NorthCamera")?.GetComponent<Camera>();

        // �Զ����ҳ����еĿ�����
        southController = GameObject.Find("southController");
        northController = GameObject.Find("northController");

        GameObject p1PositionObj = GameObject.Find("P1BallPosition");
        GameObject p2PositionObj = GameObject.Find("P2BallPosition");

        if (p1PositionObj != null)
        {
            p1BallPosition = p1PositionObj.transform.position;
        }
        else
        {
            Debug.LogError("δ�ҵ� P1BallPosition ����");
        }

        if (p2PositionObj != null)
        {
            p2BallPosition = p2PositionObj.transform.position;
        }
        else
        {
            Debug.LogError("δ�ҵ� P2BallPosition ����");
        }


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
        // ������Ƿ��Ѿ�����
        if (!hasSpawnedBall)
        {
            // ÿ��������ɲ������Լ�����
            SpawnPlayerBall();
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // ������ң�P1���������ϲ��������flippers����
            SetPlayerPosition(true); // true ��ʾ���ϲࣨ������
        }
        else
        {
            // ������ң�P2�������ñ����������flippers����
            SetPlayerPosition(false); // false ��ʾ�Ǳ��ࣨ�ͻ���
        }
    }


    void SpawnPlayerBall()
    {
        // �ٴμ���־��ȷ�����ظ�������
        if (hasSpawnedBall)
        {
            Debug.LogWarning("���Ѿ����ɣ��������ɡ�");
            return;
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedBallPrefab", out object selectedBallPrefabNameObj))
        {
            string selectedBallPrefabName = selectedBallPrefabNameObj as string;

            if (string.IsNullOrEmpty(selectedBallPrefabName))
            {
                Debug.LogError("δ�ҵ����ѡ������Ԥ�������ƣ�");
                return;
            }

            // �������λ�ã�������p1BallPosition���ͻ���p2BallPosition
            Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1BallPosition : p2BallPosition;

            // �����򣬲��Զ��������Ȩ�����ؿͻ���
            GameObject playerBall = PhotonNetwork.Instantiate(selectedBallPrefabName, spawnPosition, Quaternion.identity);

            Debug.Log("��������ҵ���" + selectedBallPrefabName + " ��λ�ã�" + spawnPosition);

            // ���ñ�־Ϊtrue����ʾ���Ѿ�����
            hasSpawnedBall = true;
        }
        else
        {
            Debug.LogError("���δѡ�����Ԥ�������ƣ�");
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


    void SetPlayerPosition(bool isSouthSide)
    {
        if (isSouthSide)
        {
            Debug.Log("�����ϲ��������������ñ�������������");
            southPlayerCamera.enabled = true;
            northPlayerCamera.enabled = false;

            SetFlippersControl(southController, true);
            SetFlippersControl(northController, false);
        }
        else
        {
            Debug.Log("���ñ������������������ϲ�����������");
            southPlayerCamera.enabled = false;
            northPlayerCamera.enabled = true;

            SetFlippersControl(northController, true);
            SetFlippersControl(southController, false);
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