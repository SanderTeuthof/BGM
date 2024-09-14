using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject _settings;

    private void Awake()
    {
        _settings.SetActive(false);
    }

    public void SettingsClicked(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

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