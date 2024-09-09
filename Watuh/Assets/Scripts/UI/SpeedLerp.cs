using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpeedLerp : MonoBehaviour
{
    [SerializeField]
    private Movement _playerMovement;
    [SerializeField]
    private float _maxSpeed = 30f;

    private void Start()
    {
        StartCoroutine(UpdateScale());
    }

    private IEnumerator UpdateScale()
    {
        while (true)
        {
            float yScale = Mathf.Lerp(0, 1, _playerMovement.Momentum / _maxSpeed);
            transform.localScale = new Vector3(transform.localScale.x, yScale, 0);
            yield return null;
        }
    }
}
