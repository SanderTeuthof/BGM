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
    [SerializeField] private float _retrievSpeed;
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
        _rb.isKinematic = true;
        if (other.gameObject.layer != _teleportLayer) return;
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
        Task retrieveTrident = new Task(LerpToPos(transform.localPosition, Vector3.zero, this.gameObject, true));
        retrieveTrident.Finished += RetrieveTrident_Finished;
        retrieveTrident.Start();
    }

    private void RetrieveTrident_Finished(bool manual)
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localPosition = Vector3.zero;
        _rb.isKinematic = true;
        _didTeleport = false;
        _canTeleport = false;
        _isTrown = false;
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
}