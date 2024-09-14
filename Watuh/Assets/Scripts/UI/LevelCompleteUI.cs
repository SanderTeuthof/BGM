using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _lvlDoneUi;
    [SerializeField]
    private GameObject _lvlFailUi;
    [SerializeField]
    private string _nextLvlName;
    [SerializeField]
    private FloatReference _completionTime;
    [SerializeField]
    private TextMeshProUGUI _completionTimeText;

    private string _currentLvlName;

    public void LoadNextLvl()
    {
        SceneManager.LoadScene(_nextLvlName);
        Time.timeScale = 1;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(_currentLvlName);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void OpenlvlDoneUI()
    {
        _lvlDoneUi.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        _currentLvlName = SceneManager.GetActiveScene().name;
        _completionTimeText.text = $"{_completionTime.value} seconds";
    }

    public void OpenlvlFailUI()
    {
        _lvlFailUi.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        _currentLvlName = SceneManager.GetActiveScene().name;
    }
}
