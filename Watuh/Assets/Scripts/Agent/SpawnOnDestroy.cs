using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour, IDestroyable
{
    [SerializeField]
    private GameObject _thingToSpawn;
    public void Destroy()
    {
        Instantiate(_thingToSpawn, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}