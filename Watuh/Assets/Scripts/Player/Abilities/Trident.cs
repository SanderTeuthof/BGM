using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trident : MonoBehaviour
{
    [SerializeField] private float _momentumMultiplier;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _tridentHolder;

    [SerializeField] private float _trowStrength;
    [SerializeField] private float _stabLength;
    [SerializeField] private int _teleportLayer;

    private GameObject _player;
    private float _playerMomentum;
    private Rigidbody _rb;

    private bool _isTrown;
    private bool _didTeleport;
    private bool _canTeleport;

    private void Start()
    {
        Movement movement = FindObjectOfType<Movement>();
        _player = movement.gameObject;
        _playerMomentum = movement.Momentum;

        _rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != _teleportLayer) return;
        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;
        _canTeleport = true;
    }
    public void Stab()
    {
        if (_isTrown) return;
    }

    public void Trow(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_isTrown && !_didTeleport && _canTeleport)
        {
            Teleport();
            return;
        }
        if (_isTrown && _didTeleport && !_canTeleport)
        {
            RetrieveTrident();
            return;
        }
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
        transform.parent = _tridentHolder;
        while (Vector3.Distance(transform.localPosition, Vector3.zero) >= 0.001f)
        {
            timer += 0.2f * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(Vector3.zero, transform.localPosition, timer);
        }
        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;
        transform.Rotate(Vector3.zero);
        _didTeleport = false;
        _isTrown = false;
    }
}