using Photon.Pun;
using UnityEngine;


public class PlayerManager : MonoBehaviourPunCallbacks
{
    private Camera southPlayerCamera;
    private Camera northPlayerCamera;
    private GameObject southController;
    private GameObject northController;
    private GameObject southPlunger;
    private GameObject northPlunger;

    private Vector3 p1BallPosition;
    private Vector3 p2BallPosition;
    void Start()
    {
        // �Զ����ҳ����е������
        southPlayerCamera = GameObject.FindWithTag("SouthCamera")?.GetComponent<Camera>();
        northPlayerCamera = GameObject.FindWithTag("NorthCamera")?.GetComponent<Camera>();

        southPlunger = GameObject.Find("southPlunger");
        northPlunger = GameObject.Find("northPlunger");

        // �Զ����ҳ����еĿ�����
        southController = GameObject.Find("southController");
        northController = GameObject.Find("northController");

        GameObject p1PositionObj = GameObject.Find("P1BallPosition");
        GameObject p2PositionObj = GameObject.Find("P2BallPosition");

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
                TransferFlipperOwnership(northController,northPlunger);
            }

            SpawnPlayerBall();
        }
    }

/*    void GenerateBallsForAllPlayers()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("selectedBallPrefab", out object selectedBallPrefabNameObj))
            {
                string selectedBallPrefabName = selectedBallPrefabNameObj as string;
                Vector3 spawnPosition = player.IsMasterClient ? p1BallPosition : p2BallPosition;

                // �ɷ���������
                GameObject playerBall = PhotonNetwork.Instantiate(selectedBallPrefabName, spawnPosition, Quaternion.identity);

                if (playerBall != null)
                {
                    Debug.Log($"�ɹ�������: {selectedBallPrefabName} ��λ��: {spawnPosition}�������˿ɼ�");
                }
                else
                {
                    Debug.LogError($"������������ʧ�ܣ�{selectedBallPrefabName}");
                }

                // ֻ�������ɷǱ�����ҵ���ʱ����ת�ƿ���Ȩ
                PhotonView ballPhotonView = playerBall.GetComponent<PhotonView>();
                if (ballPhotonView != null && ballPhotonView.Owner != player)
                {
                    ballPhotonView.TransferOwnership(player);
                    Debug.Log("����Ȩת�Ƹ���ң�" + player.NickName);
                }

                Debug.Log("������������ҵ���" + selectedBallPrefabName + " ��λ�ã�" + spawnPosition);
            }
        }
    }*/

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

    void TransferFlipperOwnership(GameObject controller, GameObject plunger)
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

        // ת�� plunger �� PhotonView ������Ȩ
        if (plunger != null)
        {
            var plungerView = plunger.GetComponent<PhotonView>();
            if (plungerView != null && plungerView.Owner != PhotonNetwork.LocalPlayer)
            {
                plungerView.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("Plunger ������Ȩת��: " + plungerView.ViewID + " ת����: " + PhotonNetwork.LocalPlayer.NickName);
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

            SetFlippersControl(southController,southPlunger, true);
            SetFlippersControl(northController, northPlunger, false);
        }
        else
        {
            Debug.Log("���ñ������������������ϲ�����������");
            southPlayerCamera.enabled = false;
            northPlayerCamera.enabled = true;

            SetFlippersControl(northController, northPlunger, true);
            SetFlippersControl(southController, southPlunger, false);
        }
    }

    // ���û���� flippers �Ŀ���
    void SetFlippersControl(GameObject controller, GameObject plunger, bool isEnabled)
    {
        var leftFlipper = controller.transform.Find("LeftFlipper").GetComponent<FlipperScript>();
        var rightFlipper = controller.transform.Find("RightFlipper").GetComponent<FlipperScript>();
        var plungerScript = plunger.GetComponent<PlungerLauncher>();

        if (leftFlipper != null)
        {
            leftFlipper.enabled = isEnabled;
        }

        if (rightFlipper != null)
        {
            rightFlipper.enabled = isEnabled;
        }

        if (plunger != null)
        {
            plungerScript.enabled = isEnabled;
        }
    }
}