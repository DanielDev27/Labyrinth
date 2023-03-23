using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public enum ExitDirection {
    North,
    East,
    South,
    West,
}

public class RoomGeneratorV2 : MonoBehaviour {
    public static RoomGeneratorV2 InstanceV2;
    ExitDirection exitDirection;
    [SerializeField] List<RoomV2> roomV2s = new List<RoomV2> ();

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
    [SerializeField] GameObject roomParent;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] int maxBranches = 1;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    void Awake () {
        InstanceV2 = this;
    }

    [Button ("Generate Rooms")]
    void Generate () {
        if (roomV2s.Count > 0) {
            ClearRooms ();
        }

        StartCoroutine (SpawnRoom (startRoom, new Vector3 (0, 0, 0)));
    }

    void ClearRooms () {
        maxInbetweenRooms = roomV2s.Count - 2;
        while (roomV2s.Count > 0) {
            Destroy (roomV2s[0].gameObject);
            roomV2s.Remove (roomV2s[0]);
        }
    }

    IEnumerator SpawnRoom (RoomDataObject roomData, Vector3 roomPosition) {
        if (maxInbetweenRooms >= -1) {
            possibleDirections.Clear ();
            RoomV2 newRoom = null;
            currentRoom = roomData;
            newRoom = Instantiate (roomPrefab, roomParent.transform);
            roomV2s.Add (newRoom);
            newRoom.transform.position = roomPosition;
            newRoom.SetRoomDataObject (currentRoom);
            _exitDoors = currentRoom.GetExitDoors (); //get exit doors
            int i = 0;
            foreach (var exit in _exitDoors) {
                if (exit) {
                    int exitIndex = (int) exitDirection;
                    exitDirection = (ExitDirection) i;
                    possibleDirections.Add (exitDirection);
                }

                i++;
            }

            if (maxInbetweenRooms >= 0) {
                chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
            }

            if (maxInbetweenRooms > 0) {
                //choose exit door from possible exit doors
                newRoom.SetExit (chosenExit);

                //get the direction of the exit door
                NextRoomPosition (newRoom, chosenExit);

                //Get compatible room based on exit door
                GetCompatibleRooms (currentRoom, chosenExit);
            }

            if (maxInbetweenRooms == 0) {
                Debug.Log ("Create an end room");

                //get the direction of the exit door
                NextRoomPosition (newRoom, chosenExit);

                //Get comnpatible room based on exit door
                if (chosenExit == ExitDirection.North) {
                    nextRoom = endRooms[2];
                }

                if (chosenExit == ExitDirection.East) {
                    nextRoom = nextRoom = endRooms[3];
                }

                if (chosenExit == ExitDirection.South) {
                    nextRoom = endRooms[0];
                }

                if (chosenExit == ExitDirection.West) {
                    nextRoom = endRooms[1];
                }
            }

            yield return new WaitForSeconds (0.1f);
            compatibleRoomEntrance.Clear ();
            currentCompatibleRooms.Clear ();
            _compatibleRoom.Clear ();
            maxInbetweenRooms--;
            if (maxInbetweenRooms >= -1) {
                StartCoroutine (SpawnRoom (nextRoom, nextRoomPosition));
            }
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