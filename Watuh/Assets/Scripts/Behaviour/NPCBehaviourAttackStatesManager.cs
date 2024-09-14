using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBehaviourAttackStatesManager : MonoBehaviour, INPCBehaviourStatesTypeManager
{
    private INPCBehaviourState _backupState;

    private NPCBehaviourStates _behaviourState = NPCBehaviourStates.Attack;
    private List<INPCBehaviourState> _states = new();
    private List<INPCAttackState> _possibleAttacks = new();

    private int _totalWeight = 0;

    public NPCBehaviourStates StateType => _behaviourState;

    private void Awake()
    {
        GetCorrectStates();
        UpdateTotalWeight();
    }

    private void GetCorrectStates()
    {
        List<INPCBehaviourState> allstates = GetComponents<INPCBehaviourState>().ToList();
        foreach (INPCBehaviourState state in allstates)
        {
            if (state.State == _behaviourState)
                _states.Add(state);
            if (_backupState == null && state.State == NPCBehaviourStates.Idle)
                _backupState = state;
        }
    }

    public INPCBehaviourState GetState(object data = null)
    {
        Transform target = data as Transform;

        if (target == null)
        {
            Debug.LogWarning("No target given to attack!");
            return _backupState;
        }

        Vector3 directionToTarget = transform.position - target.position;

        float distance = Vector3.Magnitude(directionToTarget);

        _possibleAttacks.Clear();

        foreach (INPCBehaviourState state in _states)
        {
            if (state is INPCAttackState attack && (attack.MinimumDistance < distance && distance < attack.MaximumDistance))
            {
                _possibleAttacks.Add(attack);
            }
        }

        if (_possibleAttacks.Count == 0)
        {
            Debug.LogWarning("No possible attack found");
            return _backupState;
        }
        if (_possibleAttacks.Count == 1)
        {
            return _possibleAttacks[0];
        }

        UpdateTotalWeight();

        int randomWeight = Random.Range(0, _totalWeight);
        int currentWeight = 0;

        foreach (var state in _possibleAttacks)
        {
            currentWeight += state.Weight;
            if (randomWeight < currentWeight)
            {
                return state;
            }
        }
        return _possibleAttacks[0];
    }

    public void UpdateTotalWeight()
    {
        _totalWeight = _possibleAttacks.Sum(state => state.Weight);
    }
}
