using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloatRef : MonoBehaviour
{
    [SerializeField]
    private FloatReference _floatToChange;

    public void ChangeValue(float newValue)
    {
        _floatToChange.SetValue(newValue);
    }
}
