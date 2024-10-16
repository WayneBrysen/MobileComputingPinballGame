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
        // 自动查找场景中的摄像机
        southPlayerCamera = GameObject.FindWithTag("SouthCamera")?.GetComponent<Camera>();
        northPlayerCamera = GameObject.FindWithTag("NorthCamera")?.GetComponent<Camera>();

        // 自动查找场景中的控制器
        southController = GameObject.Find("southController");
        northController = GameObject.Find("northController");

        // 打印调试信息，确保对象正确找到
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

        // 手动调用 OnJoinedRoom，确保初始化逻辑正常执行
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("已经在房间中，手动调用 OnJoinedRoom()");
            OnJoinedRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 房主玩家：启用南侧摄像机和 flippers 控制
            SetPlayerPosition(true); // true 表示是南侧（房主）
        }
        else
        {
            // 加入玩家：启用北侧摄像机和 flippers 控制
            SetPlayerPosition(false); // false 表示是北侧（客机）
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
                Debug.Log("所有权转移: " + photonView.ViewID + " 转给了: " + PhotonNetwork.LocalPlayer.NickName);
            }
        }
    }

    // 设置玩家的位置和 flippers 的控制
    void SetPlayerPosition(bool isSouthSide)
    {
        if (isSouthSide)
        {
            Debug.Log("启用南侧玩家摄像机，禁用北侧玩家摄像机。");
            southPlayerCamera.enabled = true; // 启用南侧摄像机
            northPlayerCamera.enabled = false; // 禁用北侧摄像机

            SetFlippersControl(southController, true);  // 启用南侧 flippers 的控制
            SetFlippersControl(northController, false); // 禁用北侧 flippers 的控制
        }
        else
        {
            Debug.Log("启用北侧玩家摄像机，禁用南侧玩家摄像机。");
            southPlayerCamera.enabled = false; // 禁用南侧摄像机
            northPlayerCamera.enabled = true; // 启用北侧摄像机

            SetFlippersControl(northController, true);  // 启用北侧 flippers 的控制
            SetFlippersControl(southController, false); // 禁用南侧 flippers 的控制
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