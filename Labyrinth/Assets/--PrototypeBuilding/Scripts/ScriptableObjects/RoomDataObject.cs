using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Room Data", menuName = "Data/Room Data", order = 0)]
public class RoomDataObject : ScriptableObject {
    public GameObject RoomModel;
    [SerializeReference] public List<ISpawnable> spawnables = new List<ISpawnable> ();
    public RoomDataObject[] compatableRooms;

    [Header ("Doorways")]
    [SerializeField] public bool doorN = false;

    [SerializeField] public bool doorE = false;
    [SerializeField] public bool doorS = false;
    [SerializeField] public bool doorW = false;

    public RoomDataObject GetRandomCompatibleRoom () {
        return compatableRooms[Random.Range (0, compatableRooms.Length)];
    }
}