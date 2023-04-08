using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameConstants.TimeSystem;
public class TimePanelUIManager : MonoBehaviour
{
        [SerializeField] private TMP_Text timeText;
        private void Awake()
        {
            TimeTickManager.OnChangeMinute += ChangeTime;
        }
        private void Destroy()
        {
             TimeTickManager.OnChangeMinute -= ChangeTime;
        }
        private void ChangeTime(int hour, int minute) {
            string meridian = hour >= 12 ? (hour >= 24 ? "AM" : "PM") : "AM";
        int h12 = TimeDefinitions.MilitaryTo12H(hour);
            timeText.text = $"{h12}:{minute:00} {meridian}";

            if (hour>=24 && timeText.color != Color.red)
            {
                timeText.color = Color.red;
            } else if (hour < 24 && timeText.color != Color.black)
            {
                timeText.color = Color.black;
            }
        }
}
