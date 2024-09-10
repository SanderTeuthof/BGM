using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float _maxWalkSpeed = 10f;
    [SerializeField]
    private float _maxSpeed = 70f;
    [SerializeField]
    private float _speedBuildup = 10f;
    [SerializeField]
    private float _breakTime = 0.3f;

    [Header("Dash Settings")]
    [SerializeField]
    private float _dashMomentumMultiplier = 2f;
    [SerializeField, Range(0, 1)]
    private float _dashMomentumTakeOver = 0.5f;
    [SerializeField]
    private float _dashTime = 1f;

    [Header("Fall Settings")]
    [SerializeField]
    private float _fallMomentumBuild = 1f;
    [SerializeField]
    private float _minFallTime = 0.1f; 
    [SerializeField]
    private float _maxFallTime = 1f;
    [SerializeField]
    private float _fallingSpeedMulti = 1f;
    [SerializeField]
    private float _maxFallForce = -100f;
    [SerializeField]
    private float _fallInputEffectiveness = 1f; // Control fall input reduction
    [SerializeField]
    private float _fallTimeInputCutoff = 1f; // When the input becomes ineffective
    [SerializeField]
    private float _groundCheckDistance = 0.1f;
    [SerializeField]
    private float _fallTimeMulti = 1f;

    [Header("Jump Settings")]
    [SerializeField]
    private float _jumpPower = 10f;
    [SerializeField]
    private float _gravityMultiplier = 1.2f;
    [SerializeField]
    private float _cayoteTime = 0.3f;
    [SerializeField]
    private float _minJumpMomentum = 5;

    [Header("Input Settings")]
    [SerializeField]
    private InputActionReference _moveInput;
    [SerializeField]
    private InputActionReference _rotationInput;
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private float _rotationSpeed = 0.1f;

    private const float _gravity = -9.81f;
    private float _velocityY = 0f;
    private bool _jumped = false;
    private float _fallTime = 0f;
    private bool _fallingValue = false;

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
                else 
                { 
                    _jumped = false;
                }
            }
        }
    }

    [HideInInspector]
    public float Momentum = 0;

    private Vector3 _movement = Vector3.zero;
    private Vector3 _prevInput = Vector3.zero;
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
        Vector2 inputDirection = _moveInput.action.ReadValue<Vector2>();

        Vector3 cameraForward = _camera.forward;
        cameraForward.y = 0;
        cameraForward = cameraForward.normalized;

        if (inputDirection.magnitude < 0.1f && !_falling)
        {
            _movement += _prevInput.normalized;
            return;
        }

        Vector3 cameraSidewards = Vector3.Cross(cameraForward, Vector3.up).normalized;

        float horizontalInput = -inputDirection.x;
        float verticalInput = inputDirection.y;

        // Reduce input impact while falling
        float fallInputMultiplier = Mathf.Lerp(_fallInputEffectiveness, 0f, Mathf.Clamp01(_fallTime / _fallTimeInputCutoff));

        _prevInput = (cameraForward * verticalInput + cameraSidewards * horizontalInput);

        if (_dashing)
            _prevInput.Normalize();

        _movement += _prevInput * fallInputMultiplier;
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
        bool grounded = true;

        if (!_controller.isGrounded)
            grounded = IsGrounded();

        if (grounded)
        {
            if (_velocityY < 0f) _velocityY = -1f;
            _falling = false;
        }
        else
        {
            _falling = true;
            float gravityEffect = _gravity * _gravityMultiplier;
            _velocityY = MathF.Max(_velocityY + gravityEffect * _fallTime * _fallTime, _maxFallForce);
        }

        _movement.y = _velocityY * Time.deltaTime;
        Momentum += _fallMomentumBuild * _fallTime;
    }

    private void CalculateMomentum()
    {
        if (_dashing)
            return;

        if (!_falling)
        {
            float min = MathF.Min(_maxSpeed, _controller.velocity.magnitude);
            if (min < Momentum)
                Momentum = Mathf.Lerp(Momentum, min, 0.1f);
        }

        float input = _moveInput.action.ReadValue<Vector2>().magnitude;

        if (input > 0.1f)
        {
            Momentum = MathF.Max(Momentum, MathF.Min(Momentum + input * _speedBuildup * Time.deltaTime, _maxWalkSpeed));
            return;
        }

        if (Momentum > 0f && !_falling)
        {
            Momentum = MathF.Max(MathF.Min(Momentum - _maxWalkSpeed * (1 / _breakTime) * Time.deltaTime, Momentum - Momentum * (1 / _breakTime) * Time.deltaTime), 0f);
        }
    }

    private void ExecuteMovement()
    {
        _controller.Move(_movement * Time.deltaTime * Momentum);
        _movement = Vector3.zero;
    }

    private void StartFalling()
    {
        StartCoroutine(FallingTime());
    }

    private IEnumerator FallingTime()
    {
        _fallTime = 0;
        float timer = 0f;

        while (_falling && timer < _cayoteTime)
        {
            timer += Time.deltaTime;               

            _fallTime += Time.deltaTime * _fallingSpeedMulti;
            _fallTime = Mathf.Clamp(_fallTime, _minFallTime, _maxFallTime); // Apply min/max constraints
            yield return null;
        }

        if(_falling) 
            _jumped = true;

        while (_falling)
        {
            _fallTime += Time.deltaTime * _fallingSpeedMulti;
            _fallTime = Mathf.Clamp(_fallTime, _minFallTime, _maxFallTime); // Apply min/max constraints
            yield return null;
        }

        _fallTime = 0f;
    }

    public void StartDash(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || _dashing)
            return;

        StartCoroutine(StartDashing());
    }

    public void StartJump(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || _jumped)
            return;
        if (Momentum < _minJumpMomentum)
            Momentum = _minJumpMomentum;

        _jumped = true;
        _fallTime = 0;
        _velocityY = _jumpPower;
    }

    private IEnumerator StartDashing()
    {
        _dashing = true;
        float targetMomentum = Momentum * _dashMomentumMultiplier;
        float momentumAfterDash = Mathf.Lerp(Momentum, targetMomentum, _dashMomentumTakeOver);

        Momentum = targetMomentum;

        yield return new WaitForSeconds(_dashTime);

        Momentum = momentumAfterDash;
        _dashing = false;
    }

    private bool IsGrounded()
    {
        // Cast a ray from the character slightly downwards to check for the ground
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);
    }
}


