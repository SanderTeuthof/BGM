using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourStateManager))]
public class NPCRunAwayState : MonoBehaviour, INPCAttackState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;
    [SerializeField]
    private float _minimumDistance = 0f;         // Minimum distance before fleeing
    [SerializeField]
    private float _maximumDistance = 10f;        // Maximum distance to maintain while fleeing
    [SerializeField]
    private float _runSpeed = 5f;                // Running speed of NPC
    [SerializeField]
    private float _rotationSpeed = 5f;           // Speed of rotation when fleeing
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
        StartCoroutine(RunAway());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true); // Enable running animation
    }

    public void EndState(object data = null)
    {
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false); // Disable running animation
    }

    private IEnumerator RunAway()
    {
        while (_target != null && Vector3.Distance(transform.position, _target.position) < _maximumDistance)
        {
            Vector3 directionAwayFromTarget = (transform.position - _target.position).normalized;

            // Only move horizontally, by keeping Y-axis constant
            directionAwayFromTarget.y = 0;

            // Rotate away from the target
            Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            // Move away from the target
            transform.position += directionAwayFromTarget * _runSpeed * Time.deltaTime;

            yield return null;
        }

        // Return to the default state or trigger the next attack phase
        _stateManager.DoNewAttack.Invoke(this, EventArgs.Empty);
    }
}