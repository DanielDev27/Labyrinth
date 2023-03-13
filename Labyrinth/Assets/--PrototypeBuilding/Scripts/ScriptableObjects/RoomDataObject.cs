using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Room Data", menuName = "Data/Room Data", order = 0)]
public class RoomDataObject : ScriptableObject {
    public GameObject RoomModel;
    [SerializeReference] public List<ISpawnable> spawnables = new List<ISpawnable> ();
    public RoomDataObject[] compatableRooms;

    [Header ("Entrance")]
    [SerializeField] public bool doorN = false;
    [SerializeField] public bool doorE = false;
    [SerializeField] public bool doorS = false;
    [SerializeField] public bool doorW = false;

    [Header ("Exits")]
    [SerializeField] public bool exitN = false;
    [SerializeField] public bool exitE = false;
    [SerializeField] public bool exitS = false;
    [SerializeField] public bool exitW = false;

    public RoomDataObject GetRandomCompatibleRoom () {
        return compatableRooms[Random.Range (0, compatableRooms.Length)];
    }

    public List<bool> GetEntrances () {
        List<bool> entrances = new List<bool> () { doorN, doorE, doorS, doorW };
        return entrances;
    }

    public List<bool> GetExitDoors () {
        List<bool> exitDoors = new List<bool> () { exitN, exitE, exitS, exitW };
        return exitDoors;
    }
}