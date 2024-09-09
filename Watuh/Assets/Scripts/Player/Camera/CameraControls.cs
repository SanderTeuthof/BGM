using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _camera;
    [SerializeField]
    private float _speedVertical = 1.0f;    
    [SerializeField, Tooltip("This is in degrees"), Range(-90f, 0f)]
    private float _minVerticalRotation = -60f; 
    [SerializeField, Tooltip("This is in degrees"), Range(0f, 90f)]
    private float _maxVerticalRotation = 60f;  
    [SerializeField]
    private InputActionReference _lookInput;   

    private float _verticalRotation = 0f;      
    private Vector2 _mouseInput;

    private void Start()
    {
        Vector3 initialRotation = _camera.transform.localRotation.eulerAngles;
        _verticalRotation = initialRotation.x;

        StartCoroutine(DoCameraLogic());
    }

    private IEnumerator DoCameraLogic()
    {
        while (true)
        {
            _mouseInput = _lookInput.action.ReadValue<Vector2>();
            if (_mouseInput.sqrMagnitude < 0.1)
            {
                yield return null;
                continue;
            }

            _verticalRotation -= _mouseInput.y * _speedVertical * Time.deltaTime;

            _verticalRotation = Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation);

            _camera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

            yield return null;
        }
    }
}
