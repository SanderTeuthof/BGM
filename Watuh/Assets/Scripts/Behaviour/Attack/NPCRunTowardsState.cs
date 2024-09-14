using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourStateManager))]
public class NPCRunTowardsState : MonoBehaviour, INPCAttackState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;
    [SerializeField]
    private float _minimumDistance = 1f;        // Minimum distance to stop running towards the target
    [SerializeField]
    private float _maximumDistance = 10f;       // Maximum distance within which the NPC runs towards the target
    [SerializeField]
    private float _runSpeed = 5f;               // Running speed towards the enemy
    [SerializeField]
    private float _rotationSpeed = 5f;          // Speed of rotation when running
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
        StartCoroutine(RunTowards());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true); // Enable running animation
    }

    public void EndState(object data = null)
    {
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false); // Disable running animation
    }

    private IEnumerator RunTowards()
    {
        while (_target != null && Vector3.Distance(transform.position, _target.position) > _minimumDistance)
        {
            // Calculate the direction towards the target, keeping movement horizontal (no y-axis change)
            Vector3 directionTowardsTarget = (_target.position - transform.position).normalized;
            directionTowardsTarget.y = 0;

            // Rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(directionTowardsTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            // Move towards the target
            transform.position += directionTowardsTarget * _runSpeed * Time.deltaTime;

            yield return null;
        }

        // Invoke the next behavior or stay close to the target
        _stateManager.DoNewAttack.Invoke(this, EventArgs.Empty);
    }
}