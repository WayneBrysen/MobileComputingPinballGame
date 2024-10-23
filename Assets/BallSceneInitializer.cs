using Photon.Pun;
using UnityEngine;

public class BallSceneInitializer : MonoBehaviourPunCallbacks
{
    public GameObject ballSelectionManagerPrefab; // ��� BallSelectionManager Ԥ����

    void Start()
    {
        // ��� Photon �ķ���Ƶ�ʺ����л�Ƶ�ʣ���ѡ��
        PhotonNetwork.SendRate = 60;  // ÿ�뷢�� 60 ��
        PhotonNetwork.SerializationRate = 60;  // ÿ�����л� 60 ��

        if (PhotonNetwork.IsConnected)
        {
            // �� BallChoosingUI �����ж�̬ʵ���� BallSelectionManager
            Debug.Log("�� BallChoosingUI ������ʵ���� BallSelectionManager Ԥ���塣");

            GameObject instance = PhotonNetwork.Instantiate(ballSelectionManagerPrefab.name, Vector3.zero, Quaternion.identity);

            PhotonView photonView = instance.GetComponent<PhotonView>();
            if (photonView != null)
            {
                Debug.Log("BallSelectionManager ��ʵ������PhotonView ID: " + photonView.ViewID);
                Debug.Log("PhotonView �Ƿ��Ǳ���ӵ��: " + photonView.IsMine);
            }
            else
            {
                Debug.LogError("BallSelectionManager ȱ�� PhotonView �����");
            }
        }
        else
        {
            Debug.LogError("Photon ��δ���ӣ��޷�ʵ���� BallSelectionManager��");
        }
    }
}