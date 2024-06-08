using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
   public void LoadScene(string sceneName)
{
    Debug.Log("Loading scene: " + sceneName);
    SceneManager.LoadScene(sceneName);
}
public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
