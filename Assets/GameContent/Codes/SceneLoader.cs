using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadSceneOnButtonClick : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
          SceneManager.LoadScene(sceneName);
    }
}