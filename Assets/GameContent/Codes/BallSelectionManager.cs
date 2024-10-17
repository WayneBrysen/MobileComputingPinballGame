using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class BallSelectionManager : MonoBehaviourPunCallbacks
{
    private RawImage p1Image, p2Image; // P1和P2的图片显示区域
    private RawImage ballImage; // 当前选中的球图片显示区域
    private Texture[] ballTextures; // 2D球图片数组
    private GameObject[] ballPrefabs; // 3D球Prefab数组，保存BallRed和BallBlue的Prefab

    private Button selectButton, prevButton, nextButton; // 场景中的按钮

    private int currentBallIndex = 0; // 当前选中的2D球索引

    void Start()
    {
        // 查找场景中的 UI 元素
        p1Image = GameObject.Find("p1Image").GetComponent<RawImage>();
        p2Image = GameObject.Find("p2Image").GetComponent<RawImage>();
        ballImage = GameObject.Find("ballImage").GetComponent<RawImage>();

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
        selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton").GetComponent<Button>();
        nextButton = GameObject.Find("NextButton").GetComponent<Button>();

        // 绑定按钮点击事件
        selectButton.onClick.AddListener(OnSelectBall);
        prevButton.onClick.AddListener(OnPreviousBall);
        nextButton.onClick.AddListener(OnNextBall);

        ShowBall(currentBallIndex); // 初始化时显示第一个球
        Debug.Log("Start: 当前选中的球索引为 " + currentBallIndex);
    }

    // 显示当前选择的球（2D图片）
    void ShowBall(int index)
    {
        if (index >= 0 && index < ballTextures.Length)
        {
            ballImage.texture = ballTextures[index]; // 更新当前2D球图片
            Debug.Log("ShowBall: 显示球图片，索引为 " + index);
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

        // 将选择的球的Prefab名称存储到Photon的自定义属性中
        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["selectedBallPrefab"] = selectedBallPrefabName;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnSelectBall: 玩家选择了球，Prefab名称为 " + selectedBallPrefabName);
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
                Debug.Log("OnPlayerPropertiesUpdate: 更新P1的球显示");
            }
            else
            {
                UpdateBallSelection(p2Image, selectedBall); // P2的选择更新
                Debug.Log("OnPlayerPropertiesUpdate: 更新P2的球显示");
            }
        }
        else
        {
            Debug.LogWarning("OnPlayerPropertiesUpdate: 更新的属性中没有找到 selectedBallPrefab");
        }
    }

    // 根据球选择更新图片
    void UpdateBallSelection(RawImage image, string selectedBallPrefabName)
    {
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
}