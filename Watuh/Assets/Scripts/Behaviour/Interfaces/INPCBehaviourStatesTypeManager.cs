public interface INPCBehaviourStatesTypeManager
{
    NPCBehaviourStates StateType { get; }
    INPCBehaviourState GetState(object data = null);
    void UpdateTotalWeight();
}
