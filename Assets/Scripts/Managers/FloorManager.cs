using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] List<FloorStepTrigger> _allFloorTriggers = new List<FloorStepTrigger>();

    private void Awake()
    {
        Player.OnPlayerEnterFloorTrigger += ChangeFloors;
    }

    private void OnDestroy()
    {
        Player.OnPlayerEnterFloorTrigger -= ChangeFloors;
    }

    private void ChangeFloors(int changeFromFloorID, int changeToFloorID)
    {
        // Debug.Log($"FloorManager(ChangeFloors): Change from ID {changeFromFloorID} to ID {changeToFloorID}");
        FloorStepTrigger changeFromFloor = _allFloorTriggers[changeFromFloorID-1];
        FloorStepTrigger changeToFloor = _allFloorTriggers[changeToFloorID-1];

        if(changeFromFloor && changeToFloor)
        {
            changeFromFloor.DisableFloors();
            changeToFloor.EnableFloors();
        } else
        {
            Debug.LogError("FloorManager(ChangeFloors): Change ID is not in Floor Trigger List list");
        }
    }
}
