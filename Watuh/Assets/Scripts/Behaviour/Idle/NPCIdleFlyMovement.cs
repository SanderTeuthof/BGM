using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourIdleStatesManager))]
public class NPCIdleFlyMovement : MonoBehaviour, INPCBehaviourState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;

    [SerializeField]
    private float _moveSpeed = 2.0f;          // Speed at which the NPC moves
    [SerializeField]
    private float _rotationSpeed = 5.0f;      // Speed of rotation towards move direction
    [SerializeField]
    private Transform _movementOrigin;
    [SerializeField]
    private float _moveRadius = 10.0f;        // Radius of the random point in the sphere
    [SerializeField]
    private int _checkIntervalFrames = 15;      // Time between raycasts
    [SerializeField]
    private float _rayDistance = 2.0f;        // Length of the raycast to detect obstacles

    private Vector3 _targetPoint;
    private bool _isActive = false;
    private bool _moving = false;

    public NPCBehaviourStates State => NPCBehaviourStates.Idle;

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

    private void Awake()
    {
        _stateManager = GetComponent<NPCBehaviourStateManager>();

        if (_movementOrigin == null)
            _movementOrigin = transform;
    }

    public void StartState(object data = null)
    {
        _isActive = true;
        StartCoroutine(MoveToRandomPoint());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true);
    }

    public void EndState(object data = null)
    {
        _isActive = false;
        _moving = false;
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false);
    }

    private IEnumerator MoveToRandomPoint()
    {
        while (_isActive)
        {
            _targetPoint = FindRandomPoint();
            _moving = true;
            int frames = 0;

            while (_moving)
            {
                frames++;

                MoveTowardsTarget();

                if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
                {
                    _moving = false;
                }

                if (frames % _checkIntervalFrames == 0 && CheckForObstacles())
                {
                    _targetPoint = FindRandomPoint();
                }                

                yield return null;
            }
        }

        _stateManager.SetNewState(NPCBehaviourStates.Idle);
    }

    private Vector3 FindRandomPoint()
    {
        // Find a random point within a sphere around the NPC's current position
        Vector3 randomDirection = Random.insideUnitSphere * _moveRadius;
        randomDirection += _movementOrigin.position;
        return randomDirection;
    }

    private void MoveTowardsTarget()
    {
        // Calculate the direction towards the target point
        Vector3 direction = (_targetPoint - transform.position).normalized;

        // Rotate towards the direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        // Move towards the target point
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    private bool CheckForObstacles()
    {
        // Raycast in the forward direction of movement
        Ray ray = new Ray(transform.position, (_targetPoint - transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            return true;
        }

        return false;
    }
}