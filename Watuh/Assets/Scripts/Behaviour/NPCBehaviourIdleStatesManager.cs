using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBehaviourIdleStatesManager : MonoBehaviour, INPCBehaviourStatesTypeManager
{
    private NPCBehaviourStates _behaviourState = NPCBehaviourStates.Idle;
    private List<INPCBehaviourState> _states = new();

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
        }
    }

    public INPCBehaviourState GetState(object data = null)
    {
        int randomWeight = UnityEngine.Random.Range(0, _totalWeight );
        int currentWeight = 0;

        foreach (var state in _states)
        {
            currentWeight += state.Weight;
            if (randomWeight < currentWeight)
            {
                return state;
            }
        }
        return _states[0]; 
    }


    public void UpdateTotalWeight()
    {
        _totalWeight = _states.Sum(state => state.Weight);
    }
}
