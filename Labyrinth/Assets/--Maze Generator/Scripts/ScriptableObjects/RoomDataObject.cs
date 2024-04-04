using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu (fileName = "Room Data", menuName = "Data/Room Data", order = 0)]
public class RoomDataObject : ScriptableObject {
    public string nameRoom;
    public GameObject RoomModel;

    public List<RoomDataObject> CompatibleRooms;

    [Header ("Entrance")]
    public bool DoorN;

    public bool DoorE;
    public bool DoorS;
    public bool DoorW;

    [Header ("Exits")]
    public bool ExitN;

    public bool ExitE;
    public bool ExitS;
    public bool ExitW;

    public RoomDataObject GetRandomCompatibleRoom () {
        return CompatibleRooms[Random.Range (0, CompatibleRooms.Count)];
    }

    public List<RoomDataObject> GetCompatibleRooms () { return CompatibleRooms; }

    public List<bool> GetEntrances () {
        List<bool> entrances = new List<bool> () { DoorN, DoorE, DoorS, DoorW };
        return entrances;
    }

    public List<bool> GetExitDoors () {
        List<bool> exitDoors = new List<bool> () { ExitN, ExitE, ExitS, ExitW };
        return exitDoors;
    }
}