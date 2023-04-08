using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameConstants.TimeSystem;
public class ClockArrowUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform arrowTransform;
    private void Awake()
    {
        TimeTickManager.OnChangeMinute += MoveArrow;
    }
    private void Destroy()
    {
        TimeTickManager.OnChangeMinute -= MoveArrow;
    }
    private void MoveArrow(int hour, int minute)
    {
        float eulerAngle = TimeDefinitions.NormalizeArrowRotation(hour, minute);
        // Debug.Log($"ClockArrowUIManager(MoveArrow): Converted Rad Is {eulerAngle} ");
        arrowTransform.localEulerAngles = new Vector3(0, 0, eulerAngle);
    }
}
