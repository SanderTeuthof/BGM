using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlowDownTime : MonoBehaviour
{

    private float _time;

    public void slowDown(Component sender, object data)
    {
        if(data is float)
        {
            float time = (float)data;
            StartCoroutine(DoSlowDown(time));
            Time.timeScale = 0.2f;
        }
    }

    private IEnumerator DoSlowDown(float timer)
    {
        _time = 0;
        while (_time < timer) 
        {
            _time += Time.deltaTime / Time.timeScale;
            Time.timeScale = Mathf.Lerp(0, 1, _time);
            yield return null;
        }
    }
}
