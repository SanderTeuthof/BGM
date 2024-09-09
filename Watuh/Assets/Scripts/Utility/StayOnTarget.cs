using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnTarget : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    void Start()
    {
        StartCoroutine(FollowTarget());
    }

    private IEnumerator FollowTarget()
    {
        while (true)
        {
            transform.position = _target.position;
            yield return null;
        }
    }
}
