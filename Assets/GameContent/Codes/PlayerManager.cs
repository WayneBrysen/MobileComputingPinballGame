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

    private Vector3 p1PlungerPosition;
    private Vector3 p2PlungerPosition;

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

        GameObject p1PlungerPositionObj = GameObject.Find("p1PlungerPosition");
        GameObject p2PlungerPositionObj = GameObject.Find("p2PlungerPosition");


        if (p1PlungerPositionObj != null)
        {
            p1PlungerPosition = p1PlungerPositionObj.transform.position;
        }

        if (p2PlungerPositionObj != null)
        {
            p2PlungerPosition = p2PlungerPositionObj.transform.position;
        }



        if (p1PositionObj != null)
        {
            p1BallPosition = p1PositionObj.transform.position;
        }

        if (p2PositionObj != null)
        {
            p2BallPosition = p2PositionObj.transform.position;
        }

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

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("�Ѿ��ڷ����У��ֶ����� OnJoinedRoom()");
            OnJoinedRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (photonView.IsMine)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                SetPlayerPosition(true); // �ϲࣨ������
            }
            else
            {
                SetPlayerPosition(false); // ���ࣨ�ͻ���
                TransferFlipperOwnership(northController);
            }

            SpawnPlayerplunger();

            SpawnPlayerBall();
        }
    }


    void SpawnPlayerplunger()
    {
        Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1PlungerPosition : p2PlungerPosition;

        GameObject playerplunger = PhotonNetwork.Instantiate("plunger", spawnPosition, Quaternion.identity);

        Debug.Log("��������ҵ�plunger��" + "plunger" + " ��λ�ã�" + spawnPosition);

    }

    void SpawnPlayerBall()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedBallPrefab", out object selectedBallPrefabNameObj))
        {
            string selectedBallPrefabName = selectedBallPrefabNameObj as string;

            if (string.IsNullOrEmpty(selectedBallPrefabName))
            {
                Debug.LogError("δ�ҵ����ѡ������Ԥ�������ƣ�");
                return;
            }

            // �������λ�ã������� p1BallPosition���ͻ��� p2BallPosition
            Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1BallPosition : p2BallPosition;

            // �����򣬲��Զ��������Ȩ�����ؿͻ���
            GameObject playerBall = PhotonNetwork.Instantiate(selectedBallPrefabName, spawnPosition, Quaternion.identity);

            Debug.Log("��������ҵ���" + selectedBallPrefabName + " ��λ�ã�" + spawnPosition);
        }
        else
        {
            Debug.LogError("���δѡ�����Ԥ�������ƣ�");
        }
    }

    void TransferFlipperOwnership(GameObject controller)
    {
        // ת�ƿ������ڵ����� PhotonView ������Ȩ
        var photonViews = controller.GetComponentsInChildren<PhotonView>();
        foreach (var view in photonViews)
        {
            if (PhotonNetwork.LocalPlayer != view.Owner)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("����Ȩת��: " + view.ViewID + " ת����: " + PhotonNetwork.LocalPlayer.NickName);
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