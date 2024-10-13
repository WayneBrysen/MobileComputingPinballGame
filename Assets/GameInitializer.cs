using Photon.Pun;
using UnityEngine;

public class GameInitializer : MonoBehaviourPunCallbacks
{
    public GameObject playerManager;
    void Start()
    {
        // ��� Photon �ķ���Ƶ�ʺ����л�Ƶ��
        PhotonNetwork.SendRate = 60;  // ÿ�뷢�� 60 ��
        PhotonNetwork.SerializationRate = 60;  // ÿ�����л� 60 ��
        if (PhotonNetwork.IsConnected)
        {
            // �ڷ����ж�̬ʵ���� PlayerManager
            Debug.Log("�� MainGame_PC ������ʵ���� PlayerManager Ԥ���塣");
            PhotonNetwork.Instantiate(playerManager.name, Vector3.zero, Quaternion.identity);
        }
    }
}