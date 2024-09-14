using System;
using UnityEngine;

public class WaterKeepOnLevel : MonoBehaviour
{
    [SerializeField]
    private FloatReference _waterHeight;

    private Vector3 _originLocation;

    void Awake()
    {
        _originLocation = transform.position;
        _waterHeight.UseEvent();
        _waterHeight.ValueChanged += WaterLevelChanged;
    }

    private void WaterLevelChanged(object sender, EventArgs e)
    {
        _originLocation.y = _waterHeight.value;
        transform.position = _originLocation;
    }

    private void OnDestroy()
    {
        _waterHeight.OnDestroy();
    }
}
