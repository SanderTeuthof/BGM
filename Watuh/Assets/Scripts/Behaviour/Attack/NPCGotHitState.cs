using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NPCGotHitState : MonoBehaviour, INPCBehaviourState
{
    public NPCBehaviourStates State => NPCBehaviourStates.GotHit;

    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;

    [SerializeField]
    private float _stabStrength;
    [SerializeField]
    private float _trowstrength;

    private NPCBehaviourStateManager _stateManager;
    private bool _isActive = false;

    public int Weight => _weight;
    public string[] AnimationNames => _animationNames;
    public NPCBehaviourStateManager StateManager { get => _stateManager; }

    private GameObject _trident;
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

    public void EndState(object data = null)
    {
        StopAllCoroutines();
        _stateManager.LockState = false;
        IsActive = false;
    }

    public void StartState(object data = null)
    {
        IsActive = true;
        _stateManager.LockState = true;

        _trident = data as GameObject;
        if (_trident == null)
        {
            Debug.LogWarning("Trident not given as data for state switch!");
            _stateManager.LockState = false;
            _stateManager.SetNewState(NPCBehaviourStates.Idle);
            return;
        }
        Trident trident = _trident.GetComponent<Trident>();

        if (trident == null)
        {
            Debug.LogWarning($"Trident component not found on {_trident}!");
            _stateManager.LockState = false;
            _stateManager.SetNewState(NPCBehaviourStates.Idle);
            return;
        }

        Vector3 toPos;
        if (trident.IsTrown)
        {
            toPos = GetDestination(_trident, _trowstrength, true);
            StartCoroutine(LerpToPos(transform.position, toPos, this.gameObject, false, _trowstrength / 2));
        }
        else
        {
            toPos = GetDestination(_trident, _stabStrength, false);
            StartCoroutine(LerpToPos(transform.position, toPos, this.gameObject, false, _stabStrength / 2));
        }
    }

    private Vector3 GetDestination(GameObject hitObject, float power, bool isTrown)
    {
        Vector3 direction;
        if (isTrown)
        {
            direction = hitObject.transform.forward;
            RaycastHit hit;
            Physics.Raycast(transform.position, direction, out hit);
            return hit.point;
        }
        direction = transform.position - hitObject.transform.position;
        Vector3 Destination = direction * power;
        return Destination;
    }
    private IEnumerator LerpToPos(Vector3 startpos, Vector3 toPos, GameObject MoveObject, bool moveLocal, float timer)
    {
        StartCoroutine(IsTridentInside());
        float time = 0;
        while (time < timer - 2f)
        {
            time += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startpos, toPos, time);
            if (moveLocal) MoveObject.transform.localPosition = newPos;
            else MoveObject.transform.position = newPos;
            yield return null;
        }
        EndState();
    }

    private IEnumerator IsTridentInside()
    {
        while (_trident.transform.root == transform)
        {
            yield return new WaitForSeconds(0.3f);
        }
        GetComponent<IDestroyable>().Destroy();
    }
}
