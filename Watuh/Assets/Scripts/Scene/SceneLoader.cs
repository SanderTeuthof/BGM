using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ReloadCurrentScene(Component sender, object data)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Time.timeScale = 1;
        SceneManager.LoadScene(currentScene.name);
    }
    public void LoadNewScene(Component sender, object data)
    {
        if (data is string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }            
    }
}
