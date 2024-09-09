using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerVisual : MonoBehaviour
{
    private TextMeshProUGUI _timerText;  // Reference to the TextMeshPro component

    private Coroutine _timerCoroutine;   // To keep track of the running timer coroutine
    private float _currentTime = 0f;     // Variable to track the current time
    private bool _isTimerRunning = false;

    private void Start()
    {

        _timerText = GetComponent<TextMeshProUGUI>();
        StartTimer();

    }

    // Public method to start the timer
    public void StartTimer(Component sender = null, object data = null)
    {
        if (!_isTimerRunning)  // Check if the timer isn't already running
        {
            _timerCoroutine = StartCoroutine(TimerCoroutine());
            _isTimerRunning = true;
        }
    }

    // Public method to stop the timer
    public void StopTimer(Component sender = null, object data = null)
    {
        if (_isTimerRunning)
        {
            StopCoroutine(_timerCoroutine);  // Stop the running coroutine
            _isTimerRunning = false;
        }
    }

    // Optionally reset the timer to 0
    public void ResetTimer()
    {
        StopTimer();  // Stop the timer if it's running
        _currentTime = 0f;      // Reset the time
        UpdateTimerText();      // Update the UI with the reset time
    }

    // Coroutine for handling the timer's functionality
    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            _currentTime += Time.deltaTime;  // Increment the timer
            UpdateTimerText();               // Update the displayed time
            yield return null;               // Wait for the next frame
        }
    }

    // Method to update the timer text
    private void UpdateTimerText()
    {
        _timerText.text = FormatTime(_currentTime);  // Set the text to formatted time
    }

    // Utility function to format time into minutes and seconds
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);        // Calculate minutes
        int seconds = Mathf.FloorToInt(time % 60);        // Calculate seconds
        int hundredths = Mathf.FloorToInt((time * 100) % 100);  // Calculate hundredths of a second

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundredths);
    }
}
