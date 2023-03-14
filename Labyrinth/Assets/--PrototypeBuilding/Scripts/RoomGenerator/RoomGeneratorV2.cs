using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExitDirection {
    North,
    East,
    South,
    West,
    Break
}

public class RoomGeneratorV2 : MonoBehaviour {
    public static RoomGeneratorV2 InstanceV2;
    ExitDirection exitDirection;

    [Header ("Debug Info")]
    [SerializeField] RoomDataObject currentRoom;

    [SerializeField] List<bool> _exitDoors;
    [SerializeField] List<ExitDirection> possibleDirections;
    [SerializeField] ExitDirection chosenExit;
    [SerializeField] Vector3 nextRoomPosition;
    [SerializeField] List<RoomDataObject> currentCompatibleRooms;
    [SerializeField] RoomDataObject nextRoom;
    [SerializeField] List<ExitDirection> compatibleRoomEntrance = new List<ExitDirection> ();
    [SerializeField] List<RoomDataObject> _compatibleRoom = new List<RoomDataObject> ();
    [SerializeField] int v;

    [Header ("Input Info")]
    [SerializeField] RoomDataObject startRoom;

    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] RoomV2 roomPrefab;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] int maxBranches = 1;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    void Awake () {
        InstanceV2 = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        StartCoroutine (SpawnRoom (startRoom, new Vector3 (0, 0, 0)));
    }

    IEnumerator SpawnRoom (RoomDataObject roomData, Vector3 roomPosition) {
        possibleDirections.Clear ();
        RoomV2 newRoom = null;
        currentRoom = roomData;
        newRoom = Instantiate (roomPrefab);
        newRoom.transform.position = roomPosition;
        newRoom.SetRoomDataObject (currentRoom);
        _exitDoors = currentRoom.GetExitDoors (); //get exit doors
        int i = 0;
        if (maxInbetweenRooms > 0) {
            //choose exit door from possible exit doors
            foreach (var exit in _exitDoors) {
                if (exit) {
                    int exitIndex = (int) exitDirection;
                    exitDirection = (ExitDirection) i;
                    possibleDirections.Add (exitDirection);
                }

                i++;
            }

            chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
            newRoom.SetExit (chosenExit);

            //get the direction of the exit door
            NextRoomPosition (newRoom, chosenExit);

            //Get comnpatible room based on exit door
            GetCompatibleRooms (currentRoom, chosenExit);

            yield return new WaitForSeconds (1);
            compatibleRoomEntrance.Clear ();
            currentCompatibleRooms.Clear ();
            _compatibleRoom.Clear ();
            maxInbetweenRooms--;
            StartCoroutine (SpawnRoom (nextRoom, nextRoomPosition));
        }

        if (maxInbetweenRooms == 0) {
            Debug.Log ("Create an end room");
            /*foreach (var exit in _exitDoors) {
                if (exit) {
                    int exitIndex = (int) exitDirection;
                    exitDirection = (ExitDirection) i;
                    possibleDirections.Add (exitDirection);
                }

                i++;
            }

            chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door

            //get the direction of the exit door
            NextRoomPosition (newRoom, chosenExit);

            //Get comnpatible room based on exit door
            GetCompatibleRooms (currentRoom, chosenExit);

            yield return new WaitForSeconds (2);
            maxInbetweenRooms--;
            StartCoroutine (SpawnRoom (nextRoom, nextRoomPosition));
        */
        }
    }

    Vector3 NextRoomPosition (RoomV2 newRoom, ExitDirection chosenExit) {
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

        return nextRoomPosition;
    }

    RoomDataObject GetCompatibleRooms (RoomDataObject currentRoom, ExitDirection chosenExit) {
        currentCompatibleRooms = new List<RoomDataObject> ();

        foreach (RoomDataObject compatibleRoom in currentRoom.compatibleRooms) {
            currentCompatibleRooms.Add (compatibleRoom);
            for (int i = 0; i < compatibleRoom.GetEntrances ().Count; i++) {
                if (compatibleRoom.GetEntrances ()[i]) {
                    //if compatible room has entrance at index i
                    compatibleRoomEntrance.Add ((ExitDirection) i);
                }
            }

            //compatibleRoomEntrance.Add (ExitDirection.Break);
        }

        v = 0;
        foreach (var _room in compatibleRoomEntrance) {
            if (chosenExit == ExitDirection.North && _room == ExitDirection.South) {
                //_compatibleRoom.Add (_room);
                _compatibleRoom.Add (currentRoom.compatibleRooms[v]);
            }

            if (chosenExit == ExitDirection.East && _room == ExitDirection.West) {
                //_compatibleRoom.Add (_room);
                _compatibleRoom.Add (currentRoom.compatibleRooms[v]);
            }

            if (chosenExit == ExitDirection.South && _room == ExitDirection.North) {
                //_compatibleRoom.Add (_room);
                _compatibleRoom.Add (currentRoom.compatibleRooms[v]);
            }

            if (chosenExit == ExitDirection.West && _room == ExitDirection.East) {
                //_compatibleRoom.Add (_room);
                _compatibleRoom.Add (currentRoom.compatibleRooms[v]);
            }

            v++;
        }

        nextRoom = _compatibleRoom[Random.Range (0, _compatibleRoom.Count)];
        return nextRoom;
    }
}