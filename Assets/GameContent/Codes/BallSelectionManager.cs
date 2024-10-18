using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class BallSelectionManager : MonoBehaviourPunCallbacks
{
    private RawImage p1Image, p2Image; // P1和P2的图片显示区域
    private RawImage ballImage; // 当前选中的球图片显示区域
    private Texture[] ballTextures; // 2D球图片数组
    private GameObject[] ballPrefabs; // 3D球Prefab数组，保存BallRed和BallBlue的Prefab

    private Button selectButton, prevButton, nextButton, readyButton, startGameButton; // 场景中的按钮
    private bool isReady = false;
    private bool isSceneLoading = false;

    private int currentBallIndex = 0; // 当前选中的2D球索引

    void Start()
    {
        // 查找场景中的 UI 元素
        p1Image = GameObject.Find("p1Image")?.GetComponent<RawImage>();
        p2Image = GameObject.Find("p2Image")?.GetComponent<RawImage>();
        ballImage = GameObject.Find("ballImage")?.GetComponent<RawImage>();

        // 检查是否成功找到 UI 元素
        if (p1Image == null) Debug.LogError("未找到 p1Image！");
        if (p2Image == null) Debug.LogError("未找到 p2Image！");
        if (ballImage == null) Debug.LogError("未找到 ballImage！");

        // 从 Resources 文件夹中加载球的贴图
        ballTextures = new Texture[]
        {
            Resources.Load<Texture>("BallBlue"),
            Resources.Load<Texture>("BallRed")
        };

        // 从 Resources 文件夹中加载球的 prefab
        ballPrefabs = new GameObject[]
        {
            Resources.Load<GameObject>("BallBlue"),
            Resources.Load<GameObject>("BallRed")
        };

        // 查找场景中的按钮并绑定事件
        selectButton = GameObject.Find("SelectButton")?.GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton")?.GetComponent<Button>();
        nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
        readyButton = GameObject.Find("Ready")?.GetComponent<Button>();
        startGameButton = GameObject.Find("StartGame")?.GetComponent<Button>();

        // 检查是否成功找到按钮
        if (selectButton == null) Debug.LogError("未找到 SelectButton！");
        if (prevButton == null) Debug.LogError("未找到 PrevButton！");
        if (nextButton == null) Debug.LogError("未找到 NextButton！");
        if (readyButton == null) Debug.LogError("未找到 Ready 按钮！");
        if (startGameButton == null) Debug.LogError("未找到 StartGame 按钮！");

        // 绑定按钮点击事件
        if (selectButton != null) selectButton.onClick.AddListener(OnSelectBall);
        if (prevButton != null) prevButton.onClick.AddListener(OnPreviousBall);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextBall);
        if (readyButton != null) readyButton.onClick.AddListener(OnReadyButtonClicked);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        ShowBall(currentBallIndex); // 初始化时显示第一个球
        Debug.Log("Start: 当前选中的球索引为 " + currentBallIndex);

        // 根据玩家是主机还是客机，设置按钮的可见性和可交互性
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.gameObject.SetActive(false);
            if (startGameButton != null)
            {
                startGameButton.gameObject.SetActive(true);
                startGameButton.interactable = false; // 初始不可点击
            }
            Debug.Log("StartGameButton 的点击事件已绑定。");
        }
        else
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(true);
                readyButton.interactable = false; // 初始不可点击
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

    // 显示当前选择的球（2D图片）
    void ShowBall(int index)
    {
        if (index >= 0 && index < ballTextures.Length)
        {
            if (ballImage != null)
            {
                ballImage.texture = ballTextures[index]; // 更新当前2D球图片
                Debug.Log("ShowBall: 显示球图片，索引为 " + index);
            }
        }
        else
        {
            Debug.LogWarning("ShowBall: 无效的球索引 " + index);
        }
    }

    // 切换到前一个球
    public void OnPreviousBall()
    {
        currentBallIndex = (currentBallIndex - 1 + ballTextures.Length) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnPreviousBall: 切换到前一个球，当前索引为 " + currentBallIndex);
    }

    // 切换到下一个球
    public void OnNextBall()
    {
        currentBallIndex = (currentBallIndex + 1) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnNextBall: 切换到下一个球，当前索引为 " + currentBallIndex);
    }

    // 玩家确认选择球
    public void OnSelectBall()
    {
        string selectedBallPrefabName = ballPrefabs[currentBallIndex].name;

        // 将选择的球的Prefab名称和选择状态存储到Photon的自定义属性中
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["selectedBallPrefab"] = selectedBallPrefabName;
        playerProps["hasSelectedBall"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnSelectBall: 玩家选择了球，Prefab名称为 " + selectedBallPrefabName);

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.interactable = true; // P2 在选择球后才能按 Ready 按钮
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdateStartGameButtonInteractable(); // P1 更新 StartGame 按钮状态
        }
    }

    // 当玩家加入房间时，重新检查所有玩家的选择并更新UI
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: 当前房间中的玩家数量: " + PhotonNetwork.PlayerList.Length);

        // 遍历所有玩家，并根据每个玩家的选择更新UI
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("OnJoinedRoom: 检查玩家 " + player.NickName + " 是否有已选择的球");

            if (player.CustomProperties.ContainsKey("selectedBallPrefab"))
            {
                string selectedBall = player.CustomProperties["selectedBallPrefab"] as string;

                if (player.IsMasterClient)
                {
                    UpdateBallSelection(p1Image, selectedBall); // P1的选择更新
                    Debug.Log("OnJoinedRoom: 更新P1的球显示");
                }
                else
                {
                    UpdateBallSelection(p2Image, selectedBall); // P2的选择更新
                    Debug.Log("OnJoinedRoom: 更新P2的球显示");
                }
            }
            else
            {
                Debug.LogWarning("OnJoinedRoom: 玩家 " + player.NickName + " 没有选择球的自定义属性");
            }
        }
    }

    // 当玩家属性更新时，更新选择的球
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate: 玩家属性更新被调用");

        if (changedProps.ContainsKey("selectedBallPrefab"))
        {
            string selectedBall = changedProps["selectedBallPrefab"] as string;
            Debug.Log("OnPlayerPropertiesUpdate: 玩家 " + targetPlayer.NickName + " 更新了球选择，Prefab名称为 " + selectedBall);

            if (targetPlayer.IsMasterClient)
            {
                UpdateBallSelection(p1Image, selectedBall); // P1的选择更新
            }
            else
            {
                UpdateBallSelection(p2Image, selectedBall); // P2的选择更新
            }
        }

        if (changedProps.ContainsKey("hasSelectedBall") || changedProps.ContainsKey("isReady"))
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                UpdateStartGameButtonInteractable(); // P1 更新 StartGame 按钮状态
            }

            if (!PhotonNetwork.LocalPlayer.IsMasterClient && targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps.ContainsKey("isReady"))
                {
                    isReady = (bool)changedProps["isReady"];
                    UpdateReadyButtonAppearance(); // 更新 Ready 按钮外观
                }
            }
        }
    }

    // 根据球选择更新图片
    void UpdateBallSelection(RawImage image, string selectedBallPrefabName)
    {
        if (image == null)
        {
            Debug.LogError("更新球选择时，图像组件为空！");
            return;
        }

        Debug.Log("UpdateBallSelection: 正在更新球选择，Prefab名称为 " + selectedBallPrefabName);

        // 遍历ballPrefabs数组，找到与selectedBallPrefabName匹配的Prefab，并使用对应的2D图片更新显示
        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i].name == selectedBallPrefabName)
            {
                image.texture = ballTextures[i]; // 更新显示的球图片
                Debug.Log("UpdateBallSelection: 已更新图片，球名称为 " + ballPrefabs[i].name);
                break;
            }
        }
    }

    public void OnReadyButtonClicked()
    {
        isReady = !isReady; // 切换准备状态

        // 更新 Ready 按钮外观
        UpdateReadyButtonAppearance();

        // 更新 Select 按钮的可交互性
        if (selectButton != null) selectButton.interactable = !isReady;

        // 将准备状态存储到 Photon 的自定义属性中
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["isReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnReadyButtonClicked: 玩家 " + PhotonNetwork.LocalPlayer.NickName + " 的准备状态为 " + isReady);
    }

    void UpdateReadyButtonAppearance()
    {
        if (readyButton == null)
        {
            Debug.LogError("Ready 按钮未找到，无法更新外观！");
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
            isSceneLoading = true; // 标记场景加载已开始

            // 主机负责加载场景，并通知其他客户端
            photonView.RPC("LoadSceneForClients", RpcTarget.All, "MainGame_PC");
        }
        else
        {
            Debug.LogWarning("场景已在加载中或非房主尝试加载场景。");
        }
    }

    // 为所有玩家加载场景
    [PunRPC]
    private void LoadSceneForClients(string sceneName)
    {
        try
        {
            Debug.Log("正在加载场景: " + sceneName);
            PhotonNetwork.LoadLevel(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError("加载场景时出错：" + e.Message);
        }
    }

    // 更新 StartGame 按钮的可交互性和外观
    void UpdateStartGameButtonInteractable()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame 按钮未找到，无法更新状态！");
            return;
        }

        bool p1HasSelected = false;
        bool p2IsReady = false;

        // 检查 P1 是否选择了球
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("hasSelectedBall"))
        {
            p1HasSelected = (bool)PhotonNetwork.LocalPlayer.CustomProperties["hasSelectedBall"];
        }

        // 获取 P2 的准备状态
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

        // 更新 StartGame 按钮的可交互性
        startGameButton.interactable = p1HasSelected && p2IsReady;

        // 更新 StartGame 按钮的外观
        UpdateStartGameButtonAppearance();
    }

    // 更新 StartGame 按钮的外观
    void UpdateStartGameButtonAppearance()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame 按钮未找到，无法更新外观！");
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