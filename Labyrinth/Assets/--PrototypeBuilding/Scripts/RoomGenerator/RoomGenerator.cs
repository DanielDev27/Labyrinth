using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    public static RoomGenerator Instance;
    [SerializeField] RoomDataObject startRoom;
    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] int maxInbetweenRooms;

    [SerializeField] Room roomPrefab;

    void Awake () {
        Instance = this;
        //Debug.Log (maxInbetweenRooms);
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        //RoomDataObject startRoom = inbetweenRooms[Random.Range (0, inbetweenRooms.Length)];
        SpawnRoom (startRoom);
    }

    void SpawnRoom (RoomDataObject roomData) {
        maxInbetweenRooms -= 1;
        if (maxInbetweenRooms > 0) {
            Room newRoom = Instantiate (roomPrefab);
            newRoom.SetRoomData (roomData, out List<RoomDataObject> nextRooms);

            foreach (var _nextRoom in nextRooms) {
                SpawnRoom (_nextRoom);
                //roomGameObject.transform.position = ;
            }

            Debug.Log (message: $"Inbetween Rooms Left {maxInbetweenRooms}");
        }

        if (maxInbetweenRooms == 0) {
            Room newRoom = Instantiate (roomPrefab);
            roomData = endRooms[Random.Range (0, endRooms.Length)];
            newRoom.SetRoomData (roomData, out List<RoomDataObject> nextRooms);
            //SpawnRoom (roomData);
            Debug.Log (message: $"Inbetween Rooms Left {maxInbetweenRooms}");
        }
    }
}