using Photon.Pun;
using UnityEngine;

public class GameInitializer : MonoBehaviourPunCallbacks
{
    public GameObject playerManager;
    void Start()
    {
        // 提高 Photon 的发送频率和序列化频率
        PhotonNetwork.SendRate = 60;  // 每秒发送 60 次
        PhotonNetwork.SerializationRate = 60;  // 每秒序列化 60 次
        if (PhotonNetwork.IsConnected)
        {
            // 在房间中动态实例化 PlayerManager
            Debug.Log("在 MainGame_PC 场景中实例化 PlayerManager 预制体。");
            PhotonNetwork.Instantiate(playerManager.name, Vector3.zero, Quaternion.identity);
        }
    }
}