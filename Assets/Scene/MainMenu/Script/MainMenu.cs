using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    // Reference to the Animator component
    public Animator animator;

    // Function to be triggered when the Play button is clicked
    public void OnPlayButtonClicked()
    {
        animator.SetTrigger("PlayTransition"); // Ensure this matches your animation clip's name
    }

public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Tutorial()
    {

    }
}
