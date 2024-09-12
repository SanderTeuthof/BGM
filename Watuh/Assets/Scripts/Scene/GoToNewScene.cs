using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNewScene : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;
    [SerializeField]
    private GameEvent _loadNewScene;

    public void NewScene()
    {
        _loadNewScene.Raise(this, _sceneName);
    }
}
