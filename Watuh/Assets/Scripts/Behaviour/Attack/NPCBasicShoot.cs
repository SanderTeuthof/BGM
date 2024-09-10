using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourAttackStatesManager))]
public class NPCBasicShoot : MonoBehaviour, INPCAttackState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;
    [SerializeField] 
    private float _minDistance = 5f;
    [SerializeField]
    private float _maxDistance = 30f;
    [SerializeField]
    private GameObject _thingToShoot;
    [SerializeField]
    private float _rotationSpeed = 5.0f;
    [SerializeField]
    private float _timeBeforeShoot = 0.5f;
    [SerializeField]
    private float _timeShootAnimation = 0.5f;

    private Transform _target;
    private bool _isActive = false;

    public NPCBehaviourStates State => NPCBehaviourStates.Attack;

    public int Weight => _weight;
    public string[] AnimationNames => _animationNames;


    private NPCBehaviourStateManager _stateManager;
    public NPCBehaviourStateManager StateManager { get => _stateManager; }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (value != _isActive)
                _isActive = value;
        }
    }

    public float MinimumDistance => _minDistance;

    public float MaximumDistance => _maxDistance;

    private void Awake()
    {
        _stateManager = GetComponent<NPCBehaviourStateManager>();
    }

    public void StartState(object data = null)
    {
        Transform target = data as Transform;
        _target = target;

        _isActive = true;
        StartCoroutine(ShootFlyRoutine());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true);
    }

    public void EndState(object data = null)
    {
        _isActive = false;
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false);
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[1], false);
    }

    private IEnumerator ShootFlyRoutine()
    {
        while (_isActive && _target != null)
        {
            Vector3 directionToTarget = (_target.position - transform.position).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

            float angle = Vector3.Angle(transform.forward, directionToTarget);
            if (angle < 5f)  
            {
                break; 
            }

            yield return null; 
        }

        yield return new WaitForSeconds(_timeBeforeShoot);
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[1], true);

        GameObject projectile = Instantiate(_thingToShoot, transform.position + transform.forward, Quaternion.identity);

        projectile.transform.LookAt(_target.position);


        yield return new WaitForSeconds(_timeShootAnimation);
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[1], false);

        _stateManager.DoNewAttack.Invoke(this, EventArgs.Empty);
    }

}
