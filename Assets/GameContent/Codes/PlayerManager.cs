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
        // 自动查找场景中的摄像机
        southPlayerCamera = GameObject.FindWithTag("SouthCamera")?.GetComponent<Camera>();
        northPlayerCamera = GameObject.FindWithTag("NorthCamera")?.GetComponent<Camera>();

        // 自动查找场景中的控制器
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
            Debug.Log("这是本地客户端的 PlayerManager");
        }
        else
        {
            Debug.Log("这是其他客户端的 PlayerManager");
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("已经在房间中，手动调用 OnJoinedRoom()");
            OnJoinedRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (photonView.IsMine)
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                SetPlayerPosition(true); // 南侧（房主）
            }
            else
            {
                SetPlayerPosition(false); // 北侧（客机）
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

        Debug.Log("生成了玩家的plunger：" + "plunger" + " 在位置：" + spawnPosition);

    }

    void SpawnPlayerBall()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("selectedBallPrefab", out object selectedBallPrefabNameObj))
        {
            string selectedBallPrefabName = selectedBallPrefabNameObj as string;

            if (string.IsNullOrEmpty(selectedBallPrefabName))
            {
                Debug.LogError("未找到玩家选择的球的预制体名称！");
                return;
            }

            // 生成球的位置：房主在 p1BallPosition，客机在 p2BallPosition
            Vector3 spawnPosition = PhotonNetwork.LocalPlayer.IsMasterClient ? p1BallPosition : p2BallPosition;

            // 生成球，并自动分配控制权给本地客户端
            GameObject playerBall = PhotonNetwork.Instantiate(selectedBallPrefabName, spawnPosition, Quaternion.identity);

            Debug.Log("生成了玩家的球：" + selectedBallPrefabName + " 在位置：" + spawnPosition);
        }
        else
        {
            Debug.LogError("玩家未选择球的预制体名称！");
        }
    }

    void TransferFlipperOwnership(GameObject controller)
    {
        // 转移控制器内的所有 PhotonView 的所有权
        var photonViews = controller.GetComponentsInChildren<PhotonView>();
        foreach (var view in photonViews)
        {
            if (PhotonNetwork.LocalPlayer != view.Owner)
            {
                view.TransferOwnership(PhotonNetwork.LocalPlayer);
                Debug.Log("所有权转移: " + view.ViewID + " 转给了: " + PhotonNetwork.LocalPlayer.NickName);
            }
        }
    }


    void SetPlayerPosition(bool isSouthSide)
    {
        if (isSouthSide)
        {
            Debug.Log("启用南侧玩家摄像机，禁用北侧玩家摄像机。");
            southPlayerCamera.enabled = true;
            northPlayerCamera.enabled = false;

            SetFlippersControl(southController, true);
            SetFlippersControl(northController, false);
        }
        else
        {
            Debug.Log("启用北侧玩家摄像机，禁用南侧玩家摄像机。");
            southPlayerCamera.enabled = false;
            northPlayerCamera.enabled = true;

            SetFlippersControl(northController, true);
            SetFlippersControl(southController, false);
        }
    }

    // 启用或禁用 flippers 的控制
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