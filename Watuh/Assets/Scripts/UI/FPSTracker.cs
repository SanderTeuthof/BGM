using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private Queue<int> _fpsQueue = new();
    private const int MaxSamples = 100;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(UpdateFPSText());
    }

    private IEnumerator UpdateFPSText()
    {
        while (true)
        {
            CalculateFPS();
            ShowFPS();
            yield return null;
        }
    }

    private void CalculateFPS()
    {
        int fps = Mathf.RoundToInt(1f / Time.deltaTime);
        if (_fpsQueue.Count >= MaxSamples)
        {
            _fpsQueue.Dequeue();
        }
        _fpsQueue.Enqueue(fps);
    }

    private void ShowFPS()
    {
        if (_fpsQueue.Count == 0) return;

        int currentFPS = _fpsQueue.Last();
        int averageFPS = Mathf.RoundToInt((float)_fpsQueue.Average());
        int lowestFPS = _fpsQueue.Min();
        int highestFPS = _fpsQueue.Max();

        _text.text = $"Current FPS: {currentFPS}\n" +
                     $"Average FPS: {averageFPS}\n" +
                     $"Lowest FPS: {lowestFPS}\n" +
                     $"Highest FPS: {highestFPS}";
    }
}
