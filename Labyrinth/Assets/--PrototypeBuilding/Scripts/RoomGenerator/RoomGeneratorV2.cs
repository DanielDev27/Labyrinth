using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExitDirection {
    North,
    East,
    South,
    West
}

public class RoomGeneratorV2 : MonoBehaviour {
    public static RoomGeneratorV2 Instance;
    ExitDirection exitDirection;

    [Header ("Debug Info")]
    [SerializeField] List<ExitDirection> possibleDirections;

    [SerializeField] ExitDirection chosenExit;
    [SerializeField] ExitDirection moveDirection;

    [SerializeField] RoomDataObject nextRoom;
    [SerializeField] Vector3 nextRoomPosition;

    [Header ("Input Info")]
    [SerializeField] RoomDataObject startRoom;

    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] RoomV2 roomPrefab;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] int maxBranches = 1;


    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    void Awake () {
        Instance = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        StartCoroutine (SpawnRoom (startRoom, new Vector3 (0, 0, 0)));
    }

    IEnumerator SpawnRoom (RoomDataObject roomData, Vector3 roomPosition) {
        RoomV2 newRoom = Instantiate (roomPrefab);
        RoomDataObject currentRoom = roomData;
        newRoom.SetRoomDataObject (currentRoom);
        List<bool> _exitDoors = roomData.GetExitDoors (); //get exit doors
        int i = 0;
        //choose exit door from possible exit doors
        foreach (var exit in _exitDoors) {
            if (exit) {
                int exitIndex = (int) exitDirection;
                exitDirection = (ExitDirection) exitIndex;
                possibleDirections.Add (exitDirection);
            }

            i++;
        }

        chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
        //Debug.Log (chosenExit);
        //get the direction of the exit door
        if (chosenExit == ExitDirection.North) {
            nextRoomPosition = newRoom.transform.position + Vector3.forward * 3;
        }

        if (chosenExit == ExitDirection.East) {
            nextRoomPosition = newRoom.transform.position + Vector3.right * 3;
        }

        if (chosenExit == ExitDirection.South) {
            nextRoomPosition = newRoom.transform.position + Vector3.back * 3;
        }

        if (chosenExit == ExitDirection.West) {
            nextRoomPosition = newRoom.transform.position + Vector3.left * 3;
        }

        //Debug.Log (nextRoomPosition);

        //Get comnpatible room based on exit door


        yield return new WaitForSeconds (1);
        //StartCoroutine (SpawnRoom (nextRoom, roomPosition));
    }
}