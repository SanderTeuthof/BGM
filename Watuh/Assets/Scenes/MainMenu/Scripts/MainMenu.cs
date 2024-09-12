using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameEvent _loadNewScene;

    public void GoToScene(string sceneName)
    {
        _loadNewScene.Raise(this, sceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
