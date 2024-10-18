using Photon.Pun;
using UnityEngine;

public class BallSceneInitializer : MonoBehaviourPunCallbacks
{
    public GameObject ballSelectionManagerPrefab; // 你的 BallSelectionManager 预制体

    void Start()
    {
        // 提高 Photon 的发送频率和序列化频率（可选）
        PhotonNetwork.SendRate = 60;  // 每秒发送 60 次
        PhotonNetwork.SerializationRate = 60;  // 每秒序列化 60 次

        if (PhotonNetwork.IsConnected)
        {
            // 在 BallChoosingUI 场景中动态实例化 BallSelectionManager
            Debug.Log("在 BallChoosingUI 场景中实例化 BallSelectionManager 预制体。");

            GameObject instance = PhotonNetwork.Instantiate(ballSelectionManagerPrefab.name, Vector3.zero, Quaternion.identity);

            PhotonView photonView = instance.GetComponent<PhotonView>();
            if (photonView != null)
            {
                Debug.Log("BallSelectionManager 被实例化。PhotonView ID: " + photonView.ViewID);
                Debug.Log("PhotonView 是否是本地拥有: " + photonView.IsMine);
            }
            else
            {
                Debug.LogError("BallSelectionManager 缺少 PhotonView 组件！");
            }
        }
        else
        {
            Debug.LogError("Photon 尚未连接，无法实例化 BallSelectionManager！");
        }
    }
}