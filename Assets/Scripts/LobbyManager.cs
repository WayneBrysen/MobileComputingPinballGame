using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviour
{

    public GameObject lobbyPrefab;
    public Transform lobbyContainer;
    private List<GameObject> lobbies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateLobbyUI("LobbyName1");
        GenerateLobbyUI("Lobby");
        GenerateLobbyUI("Lobby");
        GenerateLobbyUI("Lobby");
        GenerateLobbyUI("Lobby");
        GenerateLobbyUI("Testing");

    }

    public void GenerateLobbyUI(string name)
    {
        GameObject newLobby = Instantiate(lobbyPrefab, lobbyContainer);
        newLobby.GetComponentInChildren<Canvas>().sortingOrder = 1;

        Transform lobbyCanvas = newLobby.transform.Find("Canvas");
        lobbyCanvas.transform.Find("LobbyName").GetComponent<TextMeshProUGUI>().text = name;
        
        newLobby.SetActive(true);
        // RectTransform rectTransform = newLobby.GetComponentInChildren<RectTransform>();
        // if (rectTransform != null)
        // {
        //     rectTransform.localScale = Vector3.one;
        // }

        lobbies.Add(newLobby);
    }

    
}
