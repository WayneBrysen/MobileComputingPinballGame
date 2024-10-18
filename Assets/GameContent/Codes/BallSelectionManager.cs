using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class BallSelectionManager : MonoBehaviourPunCallbacks
{
    private RawImage p1Image, p2Image; // P1��P2��ͼƬ��ʾ����
    private RawImage ballImage; // ��ǰѡ�е���ͼƬ��ʾ����
    private Texture[] ballTextures; // 2D��ͼƬ����
    private GameObject[] ballPrefabs; // 3D��Prefab���飬����BallRed��BallBlue��Prefab

    private Button selectButton, prevButton, nextButton, readyButton, startGameButton; // �����еİ�ť
    private bool isReady = false;
    private bool isSceneLoading = false;

    private int currentBallIndex = 0; // ��ǰѡ�е�2D������

    void Start()
    {
        // ���ҳ����е� UI Ԫ��
        p1Image = GameObject.Find("p1Image")?.GetComponent<RawImage>();
        p2Image = GameObject.Find("p2Image")?.GetComponent<RawImage>();
        ballImage = GameObject.Find("ballImage")?.GetComponent<RawImage>();

        // ����Ƿ�ɹ��ҵ� UI Ԫ��
        if (p1Image == null) Debug.LogError("δ�ҵ� p1Image��");
        if (p2Image == null) Debug.LogError("δ�ҵ� p2Image��");
        if (ballImage == null) Debug.LogError("δ�ҵ� ballImage��");

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
        selectButton = GameObject.Find("SelectButton")?.GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton")?.GetComponent<Button>();
        nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
        readyButton = GameObject.Find("Ready")?.GetComponent<Button>();
        startGameButton = GameObject.Find("StartGame")?.GetComponent<Button>();

        // ����Ƿ�ɹ��ҵ���ť
        if (selectButton == null) Debug.LogError("δ�ҵ� SelectButton��");
        if (prevButton == null) Debug.LogError("δ�ҵ� PrevButton��");
        if (nextButton == null) Debug.LogError("δ�ҵ� NextButton��");
        if (readyButton == null) Debug.LogError("δ�ҵ� Ready ��ť��");
        if (startGameButton == null) Debug.LogError("δ�ҵ� StartGame ��ť��");

        // �󶨰�ť����¼�
        if (selectButton != null) selectButton.onClick.AddListener(OnSelectBall);
        if (prevButton != null) prevButton.onClick.AddListener(OnPreviousBall);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextBall);
        if (readyButton != null) readyButton.onClick.AddListener(OnReadyButtonClicked);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        ShowBall(currentBallIndex); // ��ʼ��ʱ��ʾ��һ����
        Debug.Log("Start: ��ǰѡ�е�������Ϊ " + currentBallIndex);

        // ����������������ǿͻ������ð�ť�Ŀɼ��ԺͿɽ�����
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.gameObject.SetActive(false);
            if (startGameButton != null)
            {
                startGameButton.gameObject.SetActive(true);
                startGameButton.interactable = false; // ��ʼ���ɵ��
            }
            Debug.Log("StartGameButton �ĵ���¼��Ѱ󶨡�");
        }
        else
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(true);
                readyButton.interactable = false; // ��ʼ���ɵ��
            }
            if (startGameButton != null) startGameButton.gameObject.SetActive(false);
        }

        UpdateReadyButtonAppearance();
        UpdateStartGameButtonAppearance();

        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }

        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    // ��ʾ��ǰѡ�����2DͼƬ��
    void ShowBall(int index)
    {
        if (index >= 0 && index < ballTextures.Length)
        {
            if (ballImage != null)
            {
                ballImage.texture = ballTextures[index]; // ���µ�ǰ2D��ͼƬ
                Debug.Log("ShowBall: ��ʾ��ͼƬ������Ϊ " + index);
            }
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

        // ��ѡ������Prefab���ƺ�ѡ��״̬�洢��Photon���Զ���������
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["selectedBallPrefab"] = selectedBallPrefabName;
        playerProps["hasSelectedBall"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnSelectBall: ���ѡ������Prefab����Ϊ " + selectedBallPrefabName);

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.interactable = true; // P2 ��ѡ�������ܰ� Ready ��ť
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdateStartGameButtonInteractable(); // P1 ���� StartGame ��ť״̬
        }
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
            }
            else
            {
                UpdateBallSelection(p2Image, selectedBall); // P2��ѡ�����
            }
        }

        if (changedProps.ContainsKey("hasSelectedBall") || changedProps.ContainsKey("isReady"))
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                UpdateStartGameButtonInteractable(); // P1 ���� StartGame ��ť״̬
            }

            if (!PhotonNetwork.LocalPlayer.IsMasterClient && targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps.ContainsKey("isReady"))
                {
                    isReady = (bool)changedProps["isReady"];
                    UpdateReadyButtonAppearance(); // ���� Ready ��ť���
                }
            }
        }
    }

    // ������ѡ�����ͼƬ
    void UpdateBallSelection(RawImage image, string selectedBallPrefabName)
    {
        if (image == null)
        {
            Debug.LogError("������ѡ��ʱ��ͼ�����Ϊ�գ�");
            return;
        }

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

    public void OnReadyButtonClicked()
    {
        isReady = !isReady; // �л�׼��״̬

        // ���� Ready ��ť���
        UpdateReadyButtonAppearance();

        // ���� Select ��ť�Ŀɽ�����
        if (selectButton != null) selectButton.interactable = !isReady;

        // ��׼��״̬�洢�� Photon ���Զ���������
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["isReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnReadyButtonClicked: ��� " + PhotonNetwork.LocalPlayer.NickName + " ��׼��״̬Ϊ " + isReady);
    }

    void UpdateReadyButtonAppearance()
    {
        if (readyButton == null)
        {
            Debug.LogError("Ready ��ťδ�ҵ����޷�������ۣ�");
            return;
        }

        ColorBlock colors = readyButton.colors;
        if (isReady)
        {
            colors.normalColor = Color.green;
            readyButton.colors = colors;
            Text buttonText = readyButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Unready";
        }
        else
        {
            colors.normalColor = Color.white;
            readyButton.colors = colors;
            Text buttonText = readyButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Ready";
        }
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient && !isSceneLoading)
        {
            isSceneLoading = true; // ��ǳ��������ѿ�ʼ

            // ����������س�������֪ͨ�����ͻ���
            photonView.RPC("LoadSceneForClients", RpcTarget.All, "MainGame_PC");
        }
        else
        {
            Debug.LogWarning("�������ڼ����л�Ƿ������Լ��س�����");
        }
    }

    // Ϊ������Ҽ��س���
    [PunRPC]
    private void LoadSceneForClients(string sceneName)
    {
        try
        {
            Debug.Log("���ڼ��س���: " + sceneName);
            PhotonNetwork.LoadLevel(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError("���س���ʱ����" + e.Message);
        }
    }

    // ���� StartGame ��ť�Ŀɽ����Ժ����
    void UpdateStartGameButtonInteractable()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame ��ťδ�ҵ����޷�����״̬��");
            return;
        }

        bool p1HasSelected = false;
        bool p2IsReady = false;

        // ��� P1 �Ƿ�ѡ������
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("hasSelectedBall"))
        {
            p1HasSelected = (bool)PhotonNetwork.LocalPlayer.CustomProperties["hasSelectedBall"];
        }

        // ��ȡ P2 ��׼��״̬
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.IsMasterClient)
            {
                if (player.CustomProperties.ContainsKey("isReady"))
                {
                    p2IsReady = (bool)player.CustomProperties["isReady"];
                }
                break;
            }
        }

        // ���� StartGame ��ť�Ŀɽ�����
        startGameButton.interactable = p1HasSelected && p2IsReady;

        // ���� StartGame ��ť�����
        UpdateStartGameButtonAppearance();
    }

    // ���� StartGame ��ť�����
    void UpdateStartGameButtonAppearance()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame ��ťδ�ҵ����޷�������ۣ�");
            return;
        }

        ColorBlock colors = startGameButton.colors;
        if (startGameButton.interactable)
        {
            colors.normalColor = Color.green;
        }
        else
        {
            colors.normalColor = Color.gray;
        }
        startGameButton.colors = colors;
    }
}