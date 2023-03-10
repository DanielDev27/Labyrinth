using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    public static RoomGenerator Instance;
    [SerializeField] RoomDataObject startRoom;
    [SerializeField] RoomDataObject endRoom;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] int maxInbetweenRooms;

    [SerializeField] Room roomPrefab;

    void Awake () {
        Instance = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        RoomDataObject _inbetweenRoom = inbetweenRooms[Random.Range (0, inbetweenRooms.Length)];
        SpawnRoom (_inbetweenRoom);
    }

    void SpawnRoom (RoomDataObject roomData) {
        Room newRoom = Instantiate (roomPrefab);
        newRoom.SetRoomData (roomData);
    }
}