using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnAxis : MonoBehaviour
{
    [SerializeField]
    private Vector3 _axis = Vector3.right;
    [SerializeField]
    private float _moveDistance = 3f;
    [SerializeField]
    private float _moveSpeed = 1f;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.localPosition;
        StartCoroutine(StartMoving());
    }

    private IEnumerator StartMoving()
    {
        float time = 0f;

        while (true)
        {
            // Calculate the sine value over time to create smooth oscillation
            float offset = Mathf.Sin(time * _moveSpeed) * _moveDistance;

            // Apply the offset along the selected axis
            transform.localPosition = _startPosition + _axis.normalized * offset;

            // Increase time for the next frame
            time += Time.deltaTime;

            yield return null;
        }
    }
}
