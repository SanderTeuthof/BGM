using UnityEngine;

public class DestroyGameEvent : MonoBehaviour, IDestroyable
{
    [SerializeField]
    private GameEvent _eventToLaunch;

    public void Destroy()
    {
        _eventToLaunch.Raise(this);
        Destroy(gameObject);
    }
}
