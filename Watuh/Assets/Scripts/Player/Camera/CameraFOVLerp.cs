using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOVLerp : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _camera;    
    [SerializeField]
    private Movement _playerMovement;            
    [SerializeField]
    private float _minFOV = 60f;                 
    [SerializeField]
    private float _maxFOV = 90f;                 
    [SerializeField]
    private float _minMomentum = 0f;             
    [SerializeField]
    private float _maxMomentum = 70f;            
    [SerializeField]
    private float _fovLerpSpeed = 5f;


    private void Start()
    {
        StartCoroutine(UpdateFOV());
    }

    private IEnumerator UpdateFOV()
    {
        while (true)
        {
            float currentMomentum = _playerMovement._momentum;

            float normalizedMomentum = Mathf.Clamp01((currentMomentum - _minMomentum) / (_maxMomentum - _minMomentum));

            float targetFOV = Mathf.Lerp(_minFOV, _maxFOV, normalizedMomentum);

            _camera.m_Lens.FieldOfView = Mathf.Lerp(_camera.m_Lens.FieldOfView, targetFOV, _fovLerpSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
