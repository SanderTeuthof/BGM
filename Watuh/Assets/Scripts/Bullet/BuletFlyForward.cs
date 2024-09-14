using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody))]
public class BuletFlyForward : MonoBehaviour
{
    [SerializeField]
    private float _force = 10f;
    [SerializeField]
    private GameEvent _looseGame;
    [SerializeField]
    private LayerMask _ignoreLayers;

    private Rigidbody _rigidbody;
    private IDestroyable _destroyable;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _destroyable = GetComponent<IDestroyable>();
        _rigidbody.velocity = transform.forward * _force;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if ((_ignoreLayers.value & (1 << other.gameObject.layer)) != 0)
            return;

        if (other.gameObject.layer == 3)
        {
            _looseGame.Raise();
        }
        else
            _destroyable.Destroy();
    }
}
