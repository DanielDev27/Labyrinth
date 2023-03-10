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
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        int inbLength = inbetweenRooms.Length;
        Debug.Log (inbLength);
        RoomDataObject _inbetweenRoom = inbetweenRooms[Random.Range (0, inbetweenRooms.Length)];
        SpawnRoom (_inbetweenRoom);
    }

    void SpawnRoom (RoomDataObject roomData) {
        Room newRoom = Instantiate (roomPrefab);
        newRoom.SetRoomData (roomData, out List<RoomDataObject> nextRooms);

        maxInbetweenRooms--;
        if (maxInbetweenRooms > 0) {
            foreach (var _nextRoom in nextRooms) {
                SpawnRoom (_nextRoom);
            }
        }

        if (maxInbetweenRooms == 0) {
            roomData = endRooms[Random.Range (0, endRooms.Length)];
            SpawnRoom (roomData);
        }
    }
}