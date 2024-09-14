using UnityEngine;

public class SettingsMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _settings;
    
    public void SettingsClicked()
    {
        bool newSet = _settings.activeSelf;
        _settings.SetActive(!newSet);
        if (newSet)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        } 
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
        }
    }
}