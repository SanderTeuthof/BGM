using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRise : MonoBehaviour
{
    [SerializeField]
    private FloatReference _waterHeight;
    [SerializeField]
    private float _timeToRise = 10f;
    [SerializeField]
    private float _minWaterHeight = -0.5f;
    [SerializeField]
    private float _maxWaterHeight = 100f;
    [SerializeField]
    private float _bufferTimeBeforeRise = 1f;
    [SerializeField]
    private EasingType _waterRiseType = EasingType.Linear;

    void Awake()
    {
        _waterHeight.SetValue(_minWaterHeight);
        StartCoroutine(MakeWaterRise());
    }

    private IEnumerator MakeWaterRise()
    {
        yield return new WaitForSeconds(_bufferTimeBeforeRise);

        float time = 0;
        while (time < _timeToRise)
        {
            time += Time.deltaTime;
            float newHeight = Mathf.Lerp(_minWaterHeight, _maxWaterHeight, time / _timeToRise);
            _waterHeight.SetValue( newHeight );
            yield return null;
        }
    }
}
