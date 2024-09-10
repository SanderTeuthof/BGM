using System;
using System.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourAttackStatesManager))]
public class NPCFlyAwayState : MonoBehaviour, INPCAttackState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;
    [SerializeField]
    private float _minimumDistance = 0f;
    [SerializeField]
    private float _maximumDistance = 10f;
    [SerializeField]
    private float _flySpeed = 5f;
    [SerializeField]
    private float _rotationSpeed = 5f;
    private Transform _target;

    public float MinimumDistance => _minimumDistance;
    public float MaximumDistance => _maximumDistance;
    public int Weight => _weight;

    public NPCBehaviourStates State => NPCBehaviourStates.Attack;
    public string[] AnimationNames => _animationNames;

    private NPCBehaviourStateManager _stateManager;
    public NPCBehaviourStateManager StateManager { get => _stateManager; }

    private bool _isActive = false;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value != _isActive)
                _isActive = value;
        }
    }


    private void Awake()
    {
        _stateManager = GetComponent<NPCBehaviourStateManager>();
    }

    public void StartState(object data = null)
    {
        _target = data as Transform;
        StartCoroutine(FlyAway());
    }

    public void EndState(object data = null)
    {
        StopAllCoroutines();
    }

    private IEnumerator FlyAway()
    {
        while (_target != null && Vector3.Distance(transform.position, _target.position) < _maximumDistance)
        {
            Vector3 directionAwayFromTarget = (transform.position - _target.position).normalized;

            // Rotate away from the target
            Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            // Move away from the target
            transform.position += directionAwayFromTarget * _flySpeed * Time.deltaTime;

            yield return null;
        }

        _stateManager.DoNewAttack.Invoke(this, EventArgs.Empty);
    }
}
