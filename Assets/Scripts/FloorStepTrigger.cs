using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorStepTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] TileMapsToDisable;
    [SerializeField] private int _myfloorID;
    [SerializeField] private int _changeToFloorID;
    public int MyFloorID { get; private set; }
    public int ChangeToFloorID { get; private set; }
    private void Awake()
    {
        MyFloorID = _myfloorID;
        ChangeToFloorID = _changeToFloorID;
    }

    public void DisableFloors()
    {
        // Debug.Log($"FloorStepTrigger(DisableFloors) for floor ID {_myfloorID}");
        gameObject.SetActive(false);
        foreach (var map in TileMapsToDisable)
        {
            map.SetActive(false);
        }
    }

    public void EnableFloors()
    {
        // Debug.Log($"FloorStepTrigger(EnableFloors) for floor ID {_myfloorID}");
        gameObject.SetActive(true);
        foreach (var map in TileMapsToDisable)
        {
            map.SetActive(true);
        }
    }
}
