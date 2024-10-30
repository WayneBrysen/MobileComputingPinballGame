using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    public Loader loader;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if(string.Equals(Loader.lobbyType,"join")){
            StartCoroutine(GoToSelectedScene("JoinLobbyUI"));
        } else {
            StartCoroutine(GoToSelectedScene("CreateLobby"));
        }
    }

    public Animator transition;
    public float transitionTime = 1f;

    IEnumerator GoToSelectedScene(string pageName)
    {
        //Play
        transition.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load
        SceneManager.LoadScene(pageName);
    }

}
