using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField]
    private float _maxWalkSpeed = 10f;
    [SerializeField]
    private float _maxSpeed = 70f;
    [SerializeField]
    private float _speedBuildup = 10f;
    [SerializeField]
    private float _breakTime = 0.3f;
    [SerializeField]
    private float _fallMomentumBuild = 1f;
    [SerializeField]
    private float _dashMomentumMultiplier = 2f;
    [SerializeField, Range(0,1)]
    private float _dashMomentumTakeOver = 0.5f;
    [SerializeField]
    private float _dashTime = 1f;
    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _rotationInput;
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private float _rotationSpeed = 0.1f;
    [SerializeField]
    private float _jumpPower = 10f;
    [SerializeField]
    private float _gravityMultiplier = 1.2f;

    private const float _gravity = -9.81f;
    private float _velocityY = 0f;
    private float _fallTime = 0f;
    private bool _fallingValue = false;

    [HideInInspector]
    public float _momentum = 0;

    private bool _falling
    {
        get { return _fallingValue; }
        set
        {
            if (value != _fallingValue)
            {
                _fallingValue = value;

                if (value)
                {
                    StartFalling();
                }
            }
        }
    }

    private Vector3 _movement = Vector3.zero;

    private Transform _target;

    private bool _dashing;

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        RotateWithInput();

        GetHorizontalInput();       

        ApplyGravity();

        CalculateMomentum();

        ExecuteMovement();
    }

    private void GetHorizontalInput()
    {
        if (_dashing) 
            return;

        Vector2 inputDirection = _moveInput.action.ReadValue<Vector2>();

        Vector3 cameraForward = _camera.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        if (inputDirection.magnitude < 0.1f && !_falling)
        {
            _movement += cameraForward;
            return;
        }

        Vector3 cameraSidewards = Vector3.Cross(cameraForward, Vector3.up).normalized;

        float horizontalInput = -inputDirection.x;
        float verticalInput = inputDirection.y;
               
        _movement += (cameraForward * verticalInput + cameraSidewards * horizontalInput);
    }

    private void RotateWithInput()
    {
        Vector2 inputDirection = _rotationInput.action.ReadValue<Vector2>();
        if (inputDirection.sqrMagnitude < 0.1)
        {
            return;
        }

        float horizontalRotation = inputDirection.x * _rotationSpeed * Time.deltaTime;

        Vector3 currentRot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y + horizontalRotation, currentRot.z);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            if (_velocityY < 0f) _velocityY = -1f;
            _falling = false;
        }
        else
        {
            _falling = true;
            float gravityEffect = _gravity * _gravityMultiplier;
            _velocityY += Mathf.Min(-1, gravityEffect * _fallTime * _fallTime);
        }

        _movement.y = _velocityY * Time.deltaTime;
        _momentum += _fallMomentumBuild * _fallTime;
    }

    private void CalculateMomentum()
    {
        if (_dashing)
            return;

        _momentum = MathF.Min(_momentum, MathF.Min(_maxSpeed, _controller.velocity.magnitude));

        float input = _moveInput.action.ReadValue<Vector2>().magnitude;

        if (input > 0.1f)
        {
            _momentum = MathF.Max(_momentum, MathF.Min(_momentum + input * _speedBuildup * Time.deltaTime, _maxWalkSpeed));
            return;
        }
        if (_momentum > 0f)
        {
            _momentum = MathF.Max(MathF.Min(_momentum - _maxWalkSpeed * (1/_breakTime) * Time.deltaTime, _momentum - _momentum * (1 / _breakTime) * Time.deltaTime), 0f);
        }
    }

    private void ExecuteMovement()
    {
        Debug.Log(_momentum);
        _controller.Move(_movement * Time.deltaTime * _momentum);
        _movement = Vector3.zero;
    }

    private void StartFalling()
    {
        // _animator.SetAnimationBool(PlayerAnimationBools.Falling, true);
        StartCoroutine(FallingTime());
    }

    private IEnumerator FallingTime()
    {
        _fallTime = 0;

        while (_falling)
        {
            _fallTime += Time.deltaTime;
            _fallTime = Mathf.Min(_fallTime, 1f);
            yield return null;
        }

        _fallTime = 0f;
    }

    public float GetYVelocity()
    {
        return _velocityY;
    }

    public Vector3 GetVelocity()
    {
        return _controller.velocity;
    }

    public void StartDash(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || _dashing)
            return;

        StartCoroutine(StartDashing());
    }

    public void StartJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || _falling)
            return;

        _velocityY = _jumpPower;
    }

    private IEnumerator StartDashing()
    {
        _dashing = true;
        float targetMomentum = _momentum * _dashMomentumMultiplier;
        float momentumAfterDash = Mathf.Lerp(_momentum, targetMomentum, _dashMomentumTakeOver);
        
        _momentum = targetMomentum;

        float time = 0;
        while (time < _dashTime)
        {
            Vector3 forwardVec = _camera.transform.forward;
            forwardVec.y = 0f;
            _movement = forwardVec.normalized;

            time += Time.deltaTime;
            yield return null;
        }

        _momentum = momentumAfterDash;

        _dashing = false;
    }
}
