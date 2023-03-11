using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    public static RoomGenerator Instance;

    [Header ("Debug Info")]
    [SerializeField] int i = 0;

    [SerializeField] List<Vector3> roomPosition_Current = new List<Vector3> ();

    [SerializeField] List<Vector3> roomPosition_Next = new List<Vector3> ();

    [Header ("Input Info")]
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
        SpawnRoom (startRoom);
    }

    void SpawnRoom (RoomDataObject roomData) {
        maxInbetweenRooms -= 1;
        Room newRoom = Instantiate (roomPrefab);
        Vector3 roomPosition;
        if (maxInbetweenRooms > 0 && i == 0) {
            roomPosition = new Vector3 (newRoom.transform.position.x + i * 3, 0, newRoom.transform.position.z + i * 3);
            roomPosition_Current.Add (roomPosition);
            roomPosition_Next.Add (roomPosition);
            newRoom.SetRoomData (roomData, roomPosition, out List<RoomDataObject> nextRooms);
            i++;
            foreach (var _nextRoom in nextRooms) {
                SpawnRoom (_nextRoom);
                //roomGameObject.transform.position = ;
            }
        }

        if (maxInbetweenRooms > 0 && i > 0) {
            roomPosition = new Vector3 (roomPosition_Next[i - 1].x + 3, 0, roomPosition_Next[i - 1].z + 3);
            roomPosition_Current.Add (roomPosition);
            roomPosition_Next.Add (roomPosition);
            newRoom.SetRoomData (roomData, roomPosition, out List<RoomDataObject> nextRooms);
            i++;
            foreach (var _nextRoom in nextRooms) {
                SpawnRoom (_nextRoom);
                //roomGameObject.transform.position = ;
            }
        }

        if (maxInbetweenRooms == 0) {
            //Room newRoom = Instantiate (roomPrefab);
            roomPosition = new Vector3 (roomPosition_Next[i - 1].x + 3, 0, roomPosition_Next[i - 1].z + 3);
            roomPosition_Current.Add (roomPosition);
            roomPosition_Next.Add (roomPosition);
            roomData = endRooms[Random.Range (0, endRooms.Length)];
            newRoom.SetRoomData (roomData, roomPosition, out List<RoomDataObject> nextRooms);
            i++;
        }
    }
}