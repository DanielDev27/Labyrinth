using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum DoorDirection {
    North,
    East,
    South,
    West,
    Non
}

public class RoomGenerator : MonoBehaviour {
    public static RoomGenerator Instance;

    [Header ("Debug Info")]
    [SerializeField] Room previousRoom;

    [SerializeField] List<DoorDirection> possibleDirections;
    [SerializeField] DoorDirection chosenDoor;
    [SerializeField] DoorDirection moveDirection;

    [Header ("Input Info")]
    [SerializeField] RoomDataObject startRoom;

    [SerializeField] RoomDataObject nextRoom;

    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] int maxBranches = 1;

    [SerializeField] Room roomPrefab;

    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    void Awake () {
        Instance = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        StartCoroutine (SpawnRoom (startRoom));
    }

    IEnumerator SpawnRoom (RoomDataObject roomData) {
        RoomDataObject currentRoom = roomData;
        DoorDirection selectedDoorDirection = DoorDirection.Non;
        Vector3 roomPosition = Vector3.zero;
        List<KeyValuePair<RoomDataObject, DoorDirection>> nextRooms = new List<KeyValuePair<RoomDataObject, DoorDirection>> ();
        Vector2Int currentCoordinate = Vector2Int.zero;
        List<Room> spawnedRooms = new List<Room> ();

        //Starting Room
        Room newRoom = Instantiate (roomPrefab); //Starting Room
        newRoom.SetRoomData (currentRoom); //Spawn random doors and return next rooms
        nextRooms = newRoom.GetNextRooms ();
        spawnedCoordinates.Add (currentCoordinate);
        spawnedRooms.Add (newRoom);

        yield return new WaitForSeconds (1);

        while (maxInbetweenRooms > 0) {
            int _maxBranches = Mathf.Min (maxBranches, nextRooms.Count);
            for (int i = 0; i < _maxBranches; i++) {
                KeyValuePair<RoomDataObject, DoorDirection> nextRoomDataObject = nextRooms[Random.Range (0, nextRooms.Count)];
                currentRoom = nextRoomDataObject.Key;
                nextRooms.Remove (nextRoomDataObject);

                Debug.Log ($"{nextRoomDataObject.Key.name} spawning {nextRoomDataObject.Value}", nextRoomDataObject.Key);

                if (nextRoomDataObject.Value == DoorDirection.North) currentCoordinate += Vector2Int.up;
                if (nextRoomDataObject.Value == DoorDirection.East) currentCoordinate += Vector2Int.right;
                if (nextRoomDataObject.Value == DoorDirection.South) currentCoordinate += Vector2Int.down;
                if (nextRoomDataObject.Value == DoorDirection.West) currentCoordinate += Vector2Int.left;
                if (!spawnedCoordinates.Contains (currentCoordinate)) {
                    spawnedCoordinates.Add (currentCoordinate);

                    newRoom = Instantiate (roomPrefab, newRoom.transform.position, quaternion.identity); //Next Room
                    newRoom.SetRoomData (currentRoom); //Spawn random doors and return next rooms
                    newRoom.transform.position = GetRoomPosition (newRoom, nextRoomDataObject.Value);
                } else {
                    if (i + 1 == _maxBranches) {
                        nextRooms = newRoom.GetNextRooms ();
                    }

                    Debug.Log ($"<color=red>Skipping to avoid overlap</color>");

                    continue;
                }

                if (i + 1 == _maxBranches) {
                    //Only reset next rooms when last in the branch
                    nextRooms = newRoom.GetNextRooms ();
                }

                maxInbetweenRooms -= 1;

                yield return new WaitForSeconds (1);
                if (maxInbetweenRooms == 0) break;

                // StartCoroutine (SpawnRoom (roomData, chosenDoor, moveDirection));
            }
        }

        if (maxInbetweenRooms == 0) {
            //yield return new WaitForSeconds (5);
            // roomData = endRooms[(int) chosenDoor];
            // newRoom = Instantiate (roomPrefab, newRoom.transform.position, quaternion.identity); //Next Room
            // newRoom.SetRoomData (roomData, out List<KeyValuePair<RoomDataObject, DoorDirection>> _nextRooms); //Spawn random doors and return next rooms
            // newRoom.transform.position = GetNextRoomPosition (newRoom, selectedDoorDirection);

            // newRoom.SetRoomData (roomData, chosenDoor, moveDirection, out List<RoomDataObject> nextRooms);
            
            KeyValuePair<RoomDataObject, DoorDirection> nextRoomDataObject = nextRooms[Random.Range (0, nextRooms.Count)];
            newRoom = Instantiate (roomPrefab, newRoom.transform.position, quaternion.identity); //Next Room
            newRoom.SetRoomData (endRooms[(int) nextRoomDataObject.Value]); //Spawn random doors and return next rooms
            newRoom.transform.position = GetRoomPosition (newRoom, nextRoomDataObject.Value);
        }
    }

    Vector3 GetRoomPosition (Room newRoom, DoorDirection lastDoorDirection) {
        Vector3 roomPosition;
        Vector3 directionOffset = Vector3.zero;
        switch (lastDoorDirection) {
            case (DoorDirection.North):
                directionOffset = Vector3.forward;
                moveDirection = DoorDirection.North;
                break;
            case (DoorDirection.East):
                directionOffset = Vector3.right;
                moveDirection = DoorDirection.East;
                break;
            case (DoorDirection.South):
                directionOffset = Vector3.back;
                moveDirection = DoorDirection.South;
                break;
            case (DoorDirection.West):
                directionOffset = Vector3.left;
                moveDirection = DoorDirection.West;
                break;
            case (DoorDirection.Non):
                directionOffset = Vector3.zero;
                moveDirection = DoorDirection.Non;
                break;
        }

        roomPosition = new Vector3 (newRoom.transform.position.x, 0, newRoom.transform.position.z) + directionOffset * 3;
        return roomPosition;
    }
}