using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    
    public ButtonClicked buttonClicked;
    
    // Update is called once per frame
    void Update()
    {
        if (buttonClicked.isClicked == true)
        {
            if(string.Equals(buttonClicked.buttonName, "ExitButton")){
                ExitScene();
            } else if(string.Equals(buttonClicked.buttonName, "JOIN LOBBY")){
                GoToJoinLobby();
            } else if(string.Equals(buttonClicked.buttonName, "menuButton")){
                GoToMainMenu();
            }
        }
    }

    public Animator transition;
    public float transitionTime = 1f;
    
    public void ExitScene()
    {
        StartCoroutine(ReturnToPreviousPage(SceneManager.GetActiveScene().buildIndex - 1));
    }

    IEnumerator ReturnToPreviousPage(int pageNum)
    {
        //Play
        transition.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load
        SceneManager.LoadScene(pageNum);
    }

    public void GoToJoinLobby()
    {
        SceneManager.LoadScene("LobbyUI");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
