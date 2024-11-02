using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class BallSelectionManager : MonoBehaviourPunCallbacks
{
    private RawImage p1Image, p2Image;
    private RawImage ballImage;
    private Texture[] ballTextures;
    private GameObject[] ballPrefabs;

    private Button selectButton, prevButton, nextButton, readyButton, startGameButton;
    private bool isReady = false;
    private bool isSceneLoading = false;

    private int currentBallIndex = 0;

    private string lastSelectedBall = "";

    void Start()
    {
        p1Image = GameObject.Find("p1Image")?.GetComponent<RawImage>();
        p2Image = GameObject.Find("p2Image")?.GetComponent<RawImage>();
        ballImage = GameObject.Find("ballImage")?.GetComponent<RawImage>();

        if (p1Image == null) Debug.LogError("cannot find p1 image");
        if (p2Image == null) Debug.LogError("cannot find p2 image");
        if (ballImage == null) Debug.LogError("cannot find ballImage!");

        ballTextures = new Texture[]
        {
            Resources.Load<Texture>("BallBlue"),
            Resources.Load<Texture>("BallRed")
        };

        ballPrefabs = new GameObject[]
        {
            Resources.Load<GameObject>("BallBlue"),
            Resources.Load<GameObject>("BallRed")
        };

        selectButton = GameObject.Find("SelectButton")?.GetComponent<Button>();
        prevButton = GameObject.Find("PrevButton")?.GetComponent<Button>();
        nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
        readyButton = GameObject.Find("Ready")?.GetComponent<Button>();
        startGameButton = GameObject.Find("StartGame")?.GetComponent<Button>();

        if (selectButton == null) Debug.LogError("cannot find SelectButton£¡");
        if (prevButton == null) Debug.LogError("cannot find  PrevButton£¡");
        if (nextButton == null) Debug.LogError("cannot find  NextButton£¡");
        if (readyButton == null) Debug.LogError("cannot find  Ready£¡");
        if (startGameButton == null) Debug.LogError("cannot find  StartGame£¡");

        if (selectButton != null) selectButton.onClick.AddListener(OnSelectBall);
        if (prevButton != null) prevButton.onClick.AddListener(OnPreviousBall);
        if (nextButton != null) nextButton.onClick.AddListener(OnNextBall);
        if (readyButton != null) readyButton.onClick.AddListener(OnReadyButtonClicked);
        if (startGameButton != null) startGameButton.onClick.AddListener(OnStartGameButtonClicked);

        ShowBall(currentBallIndex);
        Debug.Log("Start: " + currentBallIndex);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.gameObject.SetActive(false);
            if (startGameButton != null)
            {
                startGameButton.gameObject.SetActive(true);
                startGameButton.interactable = false;
            }
            Debug.Log("StartGameButton binded");
        }
        else
        {
            if (readyButton != null)
            {
                readyButton.gameObject.SetActive(true);
                readyButton.interactable = false;
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

    void ShowBall(int index)
    {
        if (index >= 0 && index < ballTextures.Length)
        {
            if (ballImage != null)
            {
                ballImage.texture = ballTextures[index];
                Debug.Log("ShowBall: null" + index);
            }
        }
        else
        {
            Debug.LogWarning("ShowBall: null " + index);
        }
    }

    public void OnPreviousBall()
    {
        currentBallIndex = (currentBallIndex - 1 + ballTextures.Length) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnPreviousBall: current ball index: " + currentBallIndex);
    }

    public void OnNextBall()
    {
        currentBallIndex = (currentBallIndex + 1) % ballTextures.Length;
        ShowBall(currentBallIndex);
        Debug.Log("OnNextBall: current ball index: " + currentBallIndex);
    }

    public void OnSelectBall()
    {
        string selectedBallPrefabName = ballPrefabs[currentBallIndex].name;

        if (lastSelectedBall == selectedBallPrefabName)
        {
            Debug.Log("select the same ball, skip update");
            return;
        }

        lastSelectedBall = selectedBallPrefabName;

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["selectedBallPrefab"] = selectedBallPrefabName;
        playerProps["hasSelectedBall"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnSelectBall: players choose the ball: " + selectedBallPrefabName);

        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (readyButton != null) readyButton.interactable = true;
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdateStartGameButtonInteractable();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom: current the number of players: " + PhotonNetwork.PlayerList.Length);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Debug.Log("OnJoinedRoom: check players " + player.NickName + " already have the ball");

            if (player.CustomProperties.ContainsKey("selectedBallPrefab"))
            {
                string selectedBall = player.CustomProperties["selectedBallPrefab"] as string;

                if (player.IsMasterClient)
                {
                    UpdateBallSelection(p1Image, selectedBall);
                    Debug.Log("OnJoinedRoom: show p1 ball selection");
                }
                else
                {
                    UpdateBallSelection(p2Image, selectedBall);
                    Debug.Log("OnJoinedRoom: show p2 ball selection");
                }
            }
            else
            {
                Debug.LogWarning("OnJoinedRoom: player " + player.NickName + " doesn't select ball");
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("OnPlayerPropertiesUpdate: attributes of player is being used");

        if (changedProps.ContainsKey("selectedBallPrefab"))
        {
            string selectedBall = changedProps["selectedBallPrefab"] as string;
            Debug.Log("OnPlayerPropertiesUpdate: player " + targetPlayer.NickName + " update ball selection: " + selectedBall);

            if (targetPlayer.IsMasterClient)
            {
                UpdateBallSelection(p1Image, selectedBall);
            }
            else
            {
                UpdateBallSelection(p2Image, selectedBall);
            }
        }

        if (changedProps.ContainsKey("hasSelectedBall") || changedProps.ContainsKey("isReady"))
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                UpdateStartGameButtonInteractable();
            }

            if (!PhotonNetwork.LocalPlayer.IsMasterClient && targetPlayer == PhotonNetwork.LocalPlayer)
            {
                if (changedProps.ContainsKey("isReady"))
                {
                    isReady = (bool)changedProps["isReady"];
                    UpdateReadyButtonAppearance();
                }
            }
        }
    }

    void UpdateBallSelection(RawImage image, string selectedBallPrefabName)
    {
        if (image == null)
        {
            return;
        }

        Debug.Log("UpdateBallSelection: update selection" + selectedBallPrefabName);

        for (int i = 0; i < ballPrefabs.Length; i++)
        {
            if (ballPrefabs[i].name == selectedBallPrefabName)
            {
                image.texture = ballTextures[i];
                Debug.Log("UpdateBallSelection: " + ballPrefabs[i].name);
                break;
            }
        }
    }

    public void OnReadyButtonClicked()
    {
        isReady = !isReady;

        UpdateReadyButtonAppearance();

        if (selectButton != null) selectButton.interactable = !isReady;

        ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
        playerProps["isReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

        Debug.Log("OnReadyButtonClicked: player " + PhotonNetwork.LocalPlayer.NickName + " the ready state is " + isReady);
    }

    void UpdateReadyButtonAppearance()
    {
        if (readyButton == null)
        {
            Debug.LogError("Ready cannot find");
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
            isSceneLoading = true;

            photonView.RPC("LoadSceneForClients", RpcTarget.All, "MainGame_PC");
        }
        else
        {
            Debug.LogWarning("scene is loading");
        }
    }

    [PunRPC]
    private void LoadSceneForClients(string sceneName)
    {

            try
            {
                Debug.Log("update scene: " + sceneName);
                PhotonNetwork.LoadLevel(sceneName);
            }
            catch (Exception e)
            {
                Debug.LogError("loading scene got error: " + e.Message);
                isSceneLoading = false;
            }
    }

    void UpdateStartGameButtonInteractable()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame cannot find!");
            return;
        }

        bool p1HasSelected = false;
        bool p2IsReady = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("hasSelectedBall"))
        {
            p1HasSelected = (bool)PhotonNetwork.LocalPlayer.CustomProperties["hasSelectedBall"];
        }

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

        startGameButton.interactable = p1HasSelected && p2IsReady;

        UpdateStartGameButtonAppearance();
    }

    void UpdateStartGameButtonAppearance()
    {
        if (startGameButton == null)
        {
            Debug.LogError("StartGame cannot find");
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