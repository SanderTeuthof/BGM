using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trident : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _tridentHolder;

    [Header("Stats")]
    [SerializeField] private float _momentumMultiplier;
    [SerializeField] private float _trowStrength;
    [SerializeField] private float _stabLength;
    [SerializeField] private float _retrievSpeed;
    [SerializeField] private int _teleportLayer;
    [SerializeField] private float _dashCooldown;

    private GameObject _player;
    private float _playerMomentum;
    private Rigidbody _rb;

    private bool _isTrown;
    private bool _didTeleport;
    private bool _canTeleport;
    private bool _canDash = true;

    private Task _taskManager;

    private void Start()
    {
        Movement movement = FindObjectOfType<Movement>();
        _player = movement.gameObject;
        _playerMomentum = movement.Momentum;

        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _rb.isKinematic = true;
        if (other.gameObject.layer != _teleportLayer) return;
        _rb.velocity = Vector3.zero;
        _canTeleport = true;
    }
    public void Stab(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_isTrown || !_canDash) return;
        if (_taskManager == null || !_taskManager.Running)
        {
            _canDash = false;
            _player.GetComponent<Movement>().StartDash();
            //_taskManager = new Task(DoStabCoolDown(_dashCooldown));
            //_taskManager.Start();
            StartCoroutine(DoStabCoolDown(_dashCooldown));
        }
    }

    public void Trow(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_isTrown && !_didTeleport && _canTeleport)
        {
            Teleport();
            return;
        }
        if (_isTrown && _didTeleport && !_canTeleport || _isTrown)
        {
            RetrieveTrident();
            return;
        }
        _rb.isKinematic = false;
        transform.parent = null;
        _rb.AddForce(transform.forward * (_trowStrength + _playerMomentum));
        _isTrown = true;

    }

    private void Teleport()
    {
        _player.GetComponent<CharacterController>().enabled = false;
        _player.transform.position = transform.position;
        _player.GetComponent<CharacterController>().enabled = true;
        _didTeleport = true;
        _canTeleport = false;
    }

    private void RetrieveTrident()
    {
        float timer = 0;
        _rb.isKinematic = false;
        transform.parent = _tridentHolder.transform;
        if (_taskManager == null || !_taskManager.Running)
        {
            _taskManager = new Task(LerpToPos(transform.localPosition, Vector3.zero, this.gameObject, true));
            _taskManager.Finished += RetrieveTrident_Finished;
            _taskManager.Start();
        }
    }

    private void RetrieveTrident_Finished(bool manual)
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localPosition = Vector3.zero;
        _rb.isKinematic = true;
        _didTeleport = false;
        _canTeleport = false;
        _isTrown = false;
        _taskManager.Finished -= RetrieveTrident_Finished;
    }

    private IEnumerator LerpToPos(Vector3 startpos, Vector3 toPos, GameObject MoveObject, bool moveLocal)
    {
        float time = 0;
        while (time < _retrievSpeed - 2f)
        {
            time += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startpos, toPos, time);
            if(moveLocal) MoveObject.transform.localPosition = newPos;
            else MoveObject.transform.position = newPos;
            yield return null;
        }
    }

    private IEnumerator DoStabCoolDown(float timer)
    {
        yield return new WaitForSeconds(timer);
        _canDash = true;
    }
}