using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField]
    private string _nextLvlName;

    private string _currentLvlName;

    private void Start()
    {
        _currentLvlName = SceneManager.GetActiveScene().name;
    }
    public void LoadNectLvl()
    {
        SceneManager.LoadScene(_nextLvlName);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(_currentLvlName);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
