using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class BallSelectionManager : MonoBehaviourPunCallbacks
{
    private RawImage p1Image, p2Image; // P1��P2��ͼƬ��ʾ����
    private RawImage ballImage; // ��ǰѡ�е���ͼƬ��ʾ����
    private Texture[] ballTextures; // 2D��ͼƬ����
    private GameObject[] ballPrefabs; // 3D��Prefab���飬����BallRed��BallBlue��Prefab

    private Button selectButton, prevButton, nextButton; // �����еİ�ť

    private int currentBallIndex = 0; // ��ǰѡ�е�2D������

    void Start()
    {
        // ���ҳ����е� UI Ԫ��
        p1Image = GameObject.Find("p1Image").GetComponent<RawImage>();
        p2Image = GameObject.Find("p2Image").GetComponent<RawImage>();
        ballImage = GameObject.Find("ballImage").GetComponent<RawImage>();

        // �� Resources �ļ����м��������ͼ
        ballTextures = new Texture[]
        {
            Resources.Load<Texture>("BallBlue"),
            Resources.Load<Texture>("BallRed")
        };

        // �� Resources �ļ����м������ prefab
        ballPrefabs = new GameObject[]
        {
            Resources.Load<GameObject>("BallBlue"),
            Resources.Load<GameObject>("BallRed")
        };

        // ���ҳ����еİ�ť�����¼�
        selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton").GetComponent<Button>();
        nextButton = GameObject.Find("NextButton").GetComponent<Button>();

        // �󶨰�ť����¼�
        selectButton.onClick.AddListener(OnSelectBall);
        prevButton.onClick.AddListener(OnPreviousBall);
        nextButton.onClick.AddListener(OnNextBall);

        ShowBall(currentBallIndex); // ��ʼ��ʱ��ʾ��һ����
        Debug.Log("Start: ��ǰѡ�е�������Ϊ " + currentBallIndex);
    }

    // ��ʾ��ǰѡ�����2DͼƬ��
    void ShowBall(int index)
    {
        if (index >= 0 && index < ballTextures.Length)
        {
            ballImage.texture = ballTextures[index]; // ���µ�ǰ2D��ͼƬ
            Debug.Log("ShowBall: ��ʾ��ͼƬ������Ϊ " + index);
        }
        else
        {
            Debug.LogWarning("ShowBall: ��Ч�������� " + index);
        }
    }

    // �л���ǰһ����
    public void OnPreviousBall()
    {
        currentBallIndex = (currentBallIndex - 1 + ballTextures.Length) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnPreviousBall: �л���ǰһ���򣬵�ǰ����Ϊ " + currentBallIndex);
    }

    // �л�����һ����
    public void OnNextBall()
    {
        currentBallIndex = (currentBallIndex + 1) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnNextBall: �л�����һ���򣬵�ǰ����Ϊ " + currentBallIndex);
    }

    // ���ȷ��ѡ����
    public void OnSelectBall()
    {
        string selectedBallPrefabName = ballPrefabs[currentBallIndex].name;

        // ��ѡ������Prefab���ƴ洢��Photon���Զ���������
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["selectedBallPrefab"] = selectedBallPrefabName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnSelectBall: ���ѡ������Prefab����Ϊ " + selectedBallPrefabName);
    }

    // ����Ҽ��뷿��ʱ�����¼��������ҵ�ѡ�񲢸���UI
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: ��ǰ�����е��������: " + PhotonNetwork.PlayerList.Length);

        // ����������ң�������ÿ����ҵ�ѡ�����UI
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("OnJoinedRoom: ������ " + player.NickName + " �Ƿ�����ѡ�����");

            if (player.CustomProperties.ContainsKey("selectedBallPrefab"))
            {
                string selectedBall = player.CustomProperties["selectedBallPrefab"] as string;

                if (player.IsMasterClient)
                {
                    UpdateBallSelection(p1Image, selectedBall); // P1��ѡ�����
                    Debug.Log("OnJoinedRoom: ����P1������ʾ");
                }
                else
                {
                    UpdateBallSelection(p2Image, selectedBall); // P2��ѡ�����
                    Debug.Log("OnJoinedRoom: ����P2������ʾ");
                }
            }
            else
            {
                Debug.LogWarning("OnJoinedRoom: ��� " + player.NickName + " û��ѡ������Զ�������");
            }
        }
    }

    // ��������Ը���ʱ������ѡ�����
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate: ������Ը��±�����");

        if (changedProps.ContainsKey("selectedBallPrefab"))
        {
            string selectedBall = changedProps["selectedBallPrefab"] as string;
            Debug.Log("OnPlayerPropertiesUpdate: ��� " + targetPlayer.NickName + " ��������ѡ��Prefab����Ϊ " + selectedBall);

            if (targetPlayer.IsMasterClient)
            {
                UpdateBallSelection(p1Image, selectedBall); // P1��ѡ�����
                Debug.Log("OnPlayerPropertiesUpdate: ����P1������ʾ");
            }
            else
            {
                UpdateBallSelection(p2Image, selectedBall); // P2��ѡ�����
                Debug.Log("OnPlayerPropertiesUpdate: ����P2������ʾ");
            }
        }
        else
        {
            Debug.LogWarning("OnPlayerPropertiesUpdate: ���µ�������û���ҵ� selectedBallPrefab");
        }
    }

    // ������ѡ�����ͼƬ
    void UpdateBallSelection(RawImage image, string selectedBallPrefabName)
    {
        Debug.Log("UpdateBallSelection: ���ڸ�����ѡ��Prefab����Ϊ " + selectedBallPrefabName);

        // ����ballPrefabs���飬�ҵ���selectedBallPrefabNameƥ���Prefab����ʹ�ö�Ӧ��2DͼƬ������ʾ
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i].name == selectedBallPrefabName)
            {
                image.texture = ballTextures[i]; // ������ʾ����ͼƬ
                Debug.Log("UpdateBallSelection: �Ѹ���ͼƬ��������Ϊ " + ballPrefabs[i].name);
                break;
            }
        }
    }
}