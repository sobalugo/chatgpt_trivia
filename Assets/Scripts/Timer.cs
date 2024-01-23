using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Event that is triggered when the timer reaches zero
    public event Action OnTimerEnd;
    public UIController uiController;

    // Private variables to store timer duration, current time, and running state
    private float duration;
    private float currentTime;
    private bool isRunning;

    // Starts the countdown timer with the given duration
    public void StartTimer(float duration)
    {
        this.duration = duration;
        currentTime = duration;
        isRunning = true;
    }

    // Stops the countdown timer
    public void StopTimer()
    {
        isRunning = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // If the timer is running, update the current time and check if it reached zero
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            uiController.UpdateTimerDisplay(currentTime);
            if (currentTime <= 0)
            {
                isRunning = false;
                currentTime = 0;
                // Invoke the OnTimerEnd event when the timer reaches zero
                OnTimerEnd?.Invoke();
            }
        }
    }

    // Returns the remaining time on the timer
    public float GetTimeRemaining()
    {
        return currentTime;
    }
}