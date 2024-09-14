using UnityEngine;

public class DestroyPlayer : MonoBehaviour, IDestroyable
{
    [SerializeField]
    private GameEvent _eventToLaunch;

    public void Destroy()
    {
        _eventToLaunch.Raise(this);
    }
}