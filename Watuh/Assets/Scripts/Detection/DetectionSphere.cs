using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class DetectionSphere : MonoBehaviour
{
    [SerializeField]
    private float _detectionDistance = 1;
    [SerializeField]
    private LayerMask _layersToLookFor;

    [HideInInspector]
    public List<GameObject> DetectedObjects = new();

    public EventHandler<GameObject> ObjectEntered;
    public EventHandler<GameObject> ObjectExited;

    private void Awake()
    {
        transform.localScale = _detectionDistance * Vector3.one;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root) 
            return;

        if (((1 << other.gameObject.layer) & _layersToLookFor) != 0)
        {
            if (!DetectedObjects.Contains(other.gameObject))
            {
                DetectedObjects.Add(other.gameObject);
                ObjectEntered?.Invoke(this, other.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other) 
    {
        if (DetectedObjects.Contains(other.gameObject))
        {
            DetectedObjects.Remove(other.gameObject);
            ObjectExited?.Invoke(this, other.gameObject);
        }
    }
}
