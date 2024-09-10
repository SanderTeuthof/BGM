using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCBehaviourIdleStatesManager))]
public class NPCStandStillState : MonoBehaviour, INPCBehaviourState
{
    [SerializeField]
    private int _weight = 1;
    [SerializeField]
    private string[] _animationNames;

    [SerializeField]
    private float _minStandStillTime = 1;
    [SerializeField]
    private float _maxStandStillTime = 2;

    public NPCBehaviourStates State => NPCBehaviourStates.Idle;

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

    public int Weight => _weight;

    public string[] AnimationNames => _animationNames;

    private void Awake()
    {
        _stateManager = GetComponent<NPCBehaviourStateManager>();
    }

    public void StartState(object data = null)
    {
        _isActive = true;
        StartCoroutine(StandStill());
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], true);
    }

    public void EndState(object data = null)
    {
        _isActive = false;
        StopAllCoroutines();
        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false);
    }


    private IEnumerator StandStill()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(_minStandStillTime, _maxStandStillTime));

        _stateManager.AnimationHanler.SetAnimationBool(_animationNames[0], false);
        _stateManager.SetNewState(NPCBehaviourStates.Idle);
    }
}
