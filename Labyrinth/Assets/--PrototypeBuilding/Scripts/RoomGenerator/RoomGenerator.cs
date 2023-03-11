using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

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

    [SerializeField] Room roomPrefab;

    void Awake () {
        Instance = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        StartCoroutine (SpawnRoom (startRoom, DoorDirection.North, DoorDirection.Non));
    }

    IEnumerator SpawnRoom (RoomDataObject roomData, DoorDirection doorDirection, DoorDirection movedDirection) {
        Vector3 roomPosition;
        Room newRoom = Instantiate (roomPrefab); //make new room
        //Debug.Log ($"New Empty Room");
        if (previousRoom) {
            Vector3 directionOffset = Vector3.zero;
            switch (doorDirection) {
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

            newRoom.transform.position = previousRoom.transform.position + directionOffset * 3; //move new room based on exit door
            roomPosition = new Vector3 (previousRoom.transform.position.x, 0, previousRoom.transform.position.z);
        }

        if (!previousRoom) {
            roomPosition = new Vector3 (0, 0, 0);
        }

        //yield return new WaitForSeconds (5);
        if (maxInbetweenRooms > 0) {
            //roomPosition = new Vector3 (previousRoom.transform.position.x, 0, previousRoom.transform.position.z);
            newRoom.SetRoomData (roomData, chosenDoor, moveDirection, out List<RoomDataObject> nextRooms); //get room data
            previousRoom = newRoom;
            foreach (var _nextRoom in nextRooms) {
                possibleDirections = new List<DoorDirection> ();
                if (_nextRoom.doorN) possibleDirections.Add (DoorDirection.North);
                if (_nextRoom.doorE) possibleDirections.Add (DoorDirection.East);
                if (_nextRoom.doorS) possibleDirections.Add (DoorDirection.South);
                if (_nextRoom.doorW) possibleDirections.Add (DoorDirection.West);
                chosenDoor = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose next exit door
                roomData = _nextRoom;
            }

            maxInbetweenRooms -= 1;
            possibleDirections.Clear ();
            yield return new WaitForSeconds (5);
            StartCoroutine (SpawnRoom (roomData, chosenDoor, moveDirection));


        }

        if (maxInbetweenRooms == 0) {
            //yield return new WaitForSeconds (5);
            roomData = endRooms[(int) chosenDoor];
            newRoom.SetRoomData (roomData, chosenDoor, moveDirection, out List<RoomDataObject> nextRooms);
        }
    }
}