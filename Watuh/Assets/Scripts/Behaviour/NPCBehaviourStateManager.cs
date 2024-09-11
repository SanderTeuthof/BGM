using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AnimationHandler))]
public class NPCBehaviourStateManager : MonoBehaviour
{
    [SerializeField]
    private NPCBehaviourStates startState = NPCBehaviourStates.Idle;

    private List<INPCBehaviourStatesTypeManager> _states;

    [HideInInspector]
    public AnimationHandler AnimationHanler;

    private INPCBehaviourState _currentState;

    public EventHandler DoNewAttack;

    private void Start()
    {
        _states = GetComponents<INPCBehaviourStatesTypeManager>().ToList();
        AnimationHanler = GetComponent<AnimationHandler>();
        SetNewState(startState);
    }

    public void SetNewState(NPCBehaviourStates newState, object data = null)
    {
        foreach (INPCBehaviourStatesTypeManager state in _states)
        {
            if (state.StateType == newState)
            {
                if (_currentState != null) 
                    _currentState.EndState(data);
                _currentState = state.GetState(data);
                _currentState.StartState(data);
                return;
            }
        }

        if (_currentState == null)
        {
            Debug.LogWarning($"{gameObject} couldn't find the off state to start");
            return;
        }
        Debug.LogWarning($"{gameObject} couldn't find the {newState} state");
    }
}

public enum NPCBehaviourStates
{
    Inactive,
    Idle, 
    Attack,
    Stagger, 
    Flee,
    GotHit
}
