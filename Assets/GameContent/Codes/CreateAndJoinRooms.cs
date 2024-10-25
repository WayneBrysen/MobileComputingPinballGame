using TMPro;
using Photon.Pun;
using UnityEngine;


public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField nicknameInput;
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public GameObject createRoomButton;
    public GameObject joinRoomButton;

    void Start()
    {
        ToggleRoomUI(false);
    }


    public void SetPlayerNickname()
    {
        if (!string.IsNullOrEmpty(nicknameInput.text))
        {
            PhotonNetwork.NickName = nicknameInput.text;
            Debug.Log("Player's nickname£º" + PhotonNetwork.NickName);
            ToggleRoomUI(true);
        }
        else
        {
            Debug.LogWarning("give a valid name£¡");
            ToggleRoomUI(false);

        }
    }

    private void ToggleRoomUI(bool isVisible)
    {
        createRoomButton.SetActive(isVisible);
        joinRoomButton.SetActive(isVisible);
        createInput.gameObject.SetActive(isVisible);
        joinInput.gameObject.SetActive(isVisible);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("BallChoosingUI");
    }
}
