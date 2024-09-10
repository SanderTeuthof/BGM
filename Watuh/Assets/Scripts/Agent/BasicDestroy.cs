using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDestroy : MonoBehaviour, IDestroyable
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
