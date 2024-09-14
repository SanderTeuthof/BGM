using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrownCheck : MonoBehaviour
{
    [SerializeField]
    private FloatReference _waterHeight;
    [SerializeField]
    private float _timeBetweenChecks = 0.3f;
    [SerializeField]
    private float _yOffsetToDrown = 0;

    private IDestroyable _destroyScript;

    private bool _toDestroy;

    private void Awake()
    {
        _destroyScript = GetComponent<IDestroyable>();
        StartCoroutine(CheckWaterLevel());
    }

    private IEnumerator CheckWaterLevel()
    {
        while (true)
        {
            yield return new WaitForSeconds(_timeBetweenChecks);
            if (_waterHeight.value > transform.position.y + _yOffsetToDrown)
            {
                if (_toDestroy)
                {
                    _destroyScript.Destroy();
                    break;
                }
                _toDestroy = true;
                continue;
            }
            _toDestroy = false; 
        }
    }
}
