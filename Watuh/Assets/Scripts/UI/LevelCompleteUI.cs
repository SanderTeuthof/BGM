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
    private string _nextLvlName;
    [SerializeField]
    private FloatReference _completionTime;
    [SerializeField]
    private TextMeshProUGUI _completionTimeText;

    private string _currentLvlName;

    private void Start()
    {
        _currentLvlName = SceneManager.GetActiveScene().name;
        _completionTimeText.text = $"{_completionTime} seconds";
    }
    public void LoadNextLvl()
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

    public void OpenlvlDoneUI()
    {
        _lvlDoneUi.SetActive(true);
    }
}
