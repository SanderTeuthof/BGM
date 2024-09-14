using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCBehaviourStateManager))]
public class NPCIdleWalkMovement : MonoBehaviour, INPCBehaviourState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;

    [SerializeField]
    private float _moveSpeed = 1.5f;            // Speed of walking
    [SerializeField]
    private float _rotationSpeed = 5.0f;        // Rotation speed towards move direction
    [SerializeField]
    private Transform _movementOrigin;
    [SerializeField]
    private float _moveRadius = 7.0f;           // Radius to select a random walk point
    [SerializeField]
    private int _checkIntervalFramesObstacle = 15;   // Time between obstacle checks
    [SerializeField]
    private int _checkIntervalFramesGround = 25;     // Time between ground checks
    [SerializeField]
    private float _rayDistance = 2.0f;          // Distance of obstacle raycast
    [SerializeField]
    private float _groundCheckDistance = 0.5f;  // Distance for ground check (ahead of NPC)

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
        StartCoroutine(WalkToRandomPoint());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true);
    }

    public void EndState(object data = null)
    {
        _isActive = false;
        _moving = false;
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false);
    }

    private IEnumerator WalkToRandomPoint()
    {
        while (_isActive)
        {
            _targetPoint = FindRandomPoint();
            _moving = true;
            int frameCount = 0;

            while (_moving)
            {
                frameCount++;

                MoveTowardsTarget();

                if (Vector3.Distance(transform.position, _targetPoint) < 0.2f)
                {
                    _moving = false;
                }

                if (frameCount % _checkIntervalFramesObstacle == 0 && CheckForObstacles())
                {
                    _targetPoint = FindRandomPoint();
                }

                if (frameCount % _checkIntervalFramesGround == 0 && !IsGroundAhead())
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
        // Find a random point within a sphere around the NPC's current position (for walking)
        Vector3 randomDirection = Random.insideUnitSphere * _moveRadius;
        randomDirection.y = 0;  // Ignore vertical component for ground movement
        randomDirection += _movementOrigin.position;
        return randomDirection;
    }

    private void MoveTowardsTarget()
    {
        // Calculate direction towards the target point
        Vector3 direction = (_targetPoint - transform.position).normalized;

        // Rotate towards the direction
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);

        // Move forward in the calculated direction
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    private bool CheckForObstacles()
    {
        // Raycast to check for obstacles ahead in the NPC's forward direction
        Ray ray = new Ray(transform.position, (_targetPoint - transform.position).normalized);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            return true;
        }

        return false;
    }

    private bool IsGroundAhead()
    {
        // Raycast to check if there is ground ahead of the NPC
        Vector3 rayStart = transform.position + transform.forward * _rayDistance;
        Ray groundRay = new Ray(rayStart, Vector3.down);
        RaycastHit hit;

        // Cast the ray downwards from a point ahead of the NPC
        if (Physics.Raycast(groundRay, out hit, _groundCheckDistance))
        {
            return true;
        }

        return false;
    }
}