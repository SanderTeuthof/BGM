using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCBehaviourGotHitStatesManager : MonoBehaviour, INPCBehaviourStatesTypeManager
{
    private NPCBehaviourStates _behaviourState = NPCBehaviourStates.GotHit;
    private List<INPCBehaviourState> _states = new();

    private int _totalWeight = 0;

    public NPCBehaviourStates StateType => _behaviourState;

    private void Awake()
    {
        GetCorrectStates();
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
        return _states[0];
    }


    public void UpdateTotalWeight()
    {
        
    }
}