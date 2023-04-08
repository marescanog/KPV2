using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConstants.TimeSystem;

public class TimeTickManager : MonoBehaviour
{
    public static event Action OnTick;
    public static event Action<int,int> OnChangeMinute;
    //     public static event Action TriggerEndOfDay;

    private int currentTick;
    private float tickTimer;

    private int minutes;
    private int hours;

    private bool resetTime = true;
    private bool pauseTimer = false;

    private void Awake()
    {
        currentTick = 0;
        tickTimer = 0;
        minutes = 0;
        hours = TimeDefinitions.DEFAULT_START_HOUR;
    }

    private void Update()
    {
        if (!pauseTimer && !resetTime)
        {
            tickTimer += Time.deltaTime;

            if (tickTimer >= TimeDefinitions.TICK_TIMER_MAX)
            {
                tickTimer -= TimeDefinitions.TICK_TIMER_MAX;
                currentTick++;

                if (OnTick != null) OnTick();

                if (currentTick >= TimeDefinitions.MINUTE_MAX_TICKS)
                {
                    currentTick = 0;
                    minutes += 10;

                    if (minutes > TimeDefinitions.DAY_MAX_MIN)
                    {
                        minutes = 0;
                        hours++;
                    }

                    // Debug.Log($"TickTimer clock: {hours}:{minutes:00}");

                    if (OnChangeMinute != null) OnChangeMinute(hours, minutes);

                    if (hours >= TimeDefinitions.DAY_MAX_HOURS)
                    {
                        // if (TriggerEndOfDay != null) TriggerEndOfDay(); // Insert Timer Subscribe Keep Track, Timed Event. Timer Pauses and Timed Event Plays
                        // hours = TimeDefinitions.DEFAULT_START_HOUR; // Reset Value will be dependent on players energy & sleep time, for now just hardcode 5 for default
                        resetTime = true;
                        pauseTimer = true;
                        Debug.Log("TimeTickManager(Update): Paused Timer!");
                    }


                }
            }

        } else if (!pauseTimer && resetTime)
        {
            ResetTime(TimeDefinitions.DEFAULT_START_HOUR, 0);
            // Debug.Log("TimeTickManager(Update): Reset Time!");
        }
        
    }

    private void ResetTime(int newStartHour, int newStartMinute)
    {
        resetTime = false;
        if (OnChangeMinute != null) OnChangeMinute(newStartHour, newStartMinute);
    }
}
