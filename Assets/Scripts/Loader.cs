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
            ExitScene();
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
}
