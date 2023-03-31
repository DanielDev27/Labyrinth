using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using UnityEngine;

public class RoomGeneratorV2 : MonoBehaviour {
    [Header ("Debug Info")]
    [SerializeField] List<RoomV2> roomV2s = new List<RoomV2> ();

    [SerializeField] List<ExitDirection> possibleDirections;
    SpaceOccupied[,] roomPlacement;
    Vector2Int currentPlacement;
    RoomDataObject currentRoom;
    ExitDirection chosenExit;
    Vector3 nextRoomPosition;
    RoomDataObject nextRoom;

    [Header ("Input Info")]
    [SerializeField] int width;

    [SerializeField] int height;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] RoomDataObject startRoom;
    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] RoomV2 roomPrefab;
    [SerializeField] Transform roomParent;
    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    //[SerializeField] int maxBranches = 1;


    [Button ("Generate Rooms")]
    void Generate () {
        roomPlacement = new SpaceOccupied[width, height]; //create new grid array
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) { roomPlacement[x, z] = SpaceOccupied.No; } //set all indexes to unoccupied
        }

        if (roomV2s.Count > 0) { ClearRooms (); } //Clear rooms on button push if there are rooms

        StartCoroutine (SpawningRoom (startRoom, new Vector3 (0, 0, 0))); //Start generating dungeon
    }

    void ClearRooms () {
        maxInbetweenRooms = roomV2s.Count - 2; //Reset the max number of rooms
        while (roomV2s.Count > 0) {
            Destroy (roomV2s[0].gameObject);
            roomV2s.Remove (roomV2s[0]);
        } //Destroy every room and remove from the list while there are rooms
    }

    IEnumerator SpawningRoom (RoomDataObject roomData, Vector3 roomPosition) {
        //Spawn room based on room DataObject and position
        if (maxInbetweenRooms >= -1) {
            /**/
            possibleDirections.Clear (); //clear possible directions at the start
            /**/
            currentPlacement = new Vector2Int (((int) roomPosition.x / 3) + width / 2, (int) roomPosition.z / 3); //set the current placement in the grid to the incoming room position in the plane
            /**/
            RoomV2 _newRoom = null; //empty the new room variable
            currentRoom = roomData; // current room variable set as incoming room data object
            /**/
            roomPlacement[currentPlacement.x, currentPlacement.y] = SpaceOccupied.Yes; //set the spase to Occupied
            /**/
            _newRoom = Instantiate (roomPrefab, roomParent); //create room parent
            roomV2s.Add (_newRoom); //add to list
            _newRoom.transform.position = roomPosition; //set the room position as the incoming room position
            _newRoom.SetRoomDataObject (currentRoom); //set new room with the current room
            /**/
            List<bool> _exitDoors = currentRoom.GetExitDoors ();
            /**/
            int i = 0;
            foreach (var exit in _exitDoors) {
                if (exit) {
                    ExitDirection _exitDirection = (ExitDirection) i;
                    possibleDirections.Add (_exitDirection);
                } // get and add the exits doors for the current room from enum indexes

                i++;
            }

            if (maxInbetweenRooms >= 0) {
                NextPositionCheck (_newRoom, possibleDirections); //check function to see if the next position is viable
                _newRoom.SetExit (chosenExit); //choose exit door from possible exit doors
            }

            if (maxInbetweenRooms > 0) {
                NextRoomPosition (_newRoom, chosenExit); //get the direction of the exit door
                GetCompatibleRooms (currentRoom, chosenExit); //Get compatible room based on exit door
            }

            if (maxInbetweenRooms == 0) {
                NextRoomPosition (_newRoom, chosenExit); //get the direction of the exit door, Get compatible end room based on exit door
                SpawnEndRoom (chosenExit); //Spawn an end room
            }

            yield return new WaitForSeconds (1f);
            /**/
            maxInbetweenRooms--;
            if (maxInbetweenRooms >= -1) {
                //restart coroutine whilst there are inbetween rooms
                StartCoroutine (SpawningRoom (nextRoom, nextRoomPosition));
            }
        }
    }

    RoomDataObject SpawnEndRoom (ExitDirection chosenExit) {
        switch (chosenExit) {
            case ExitDirection.North:
                nextRoom = endRooms[2];
                break;
            case ExitDirection.East:
                nextRoom = endRooms[3];
                break;
            case ExitDirection.South:
                nextRoom = endRooms[0];
                break;
            case ExitDirection.West:
                nextRoom = endRooms[1];
                break;
        }

        return nextRoom;
    }

    ExitDirection NextPositionCheck (RoomV2 newRoom, List<ExitDirection> possibleExits) /*is the next position within the grid*/ {
        chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
        NextRoomPosition (newRoom, chosenExit); //get the direction of the exit door
        /**/
        Debug.Log (nextRoomPosition.x / 3 + width / 2 + ", " + nextRoomPosition.z / 3);
        /**/
        if ((nextRoomPosition.x / 3 + width / 2) < 0 || (nextRoomPosition.x / 3 + width / 2) > width || nextRoomPosition.z / 3 < 0 || nextRoomPosition.z / 3 > height) {
            /**/
            Debug.Log ("Next room will be out of range of grid " + (nextRoomPosition.x / 3 + width / 2) + ", " + nextRoomPosition.z / 3);
            /**/
            possibleDirections.Remove (chosenExit); //remove the possible exit that leads out of the grid
            /**/
            if (possibleDirections.Count < 1) {
                Debug.Log ("No possible exits that stay in the grid");
                //change the current room for a different compatible room
            } else {
                chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
            }

            NextPositionCheck (newRoom, possibleDirections); //check function to see if the next position is viable
        }

        return chosenExit;
    }

    Vector3 NextRoomPosition (RoomV2 newRoom, ExitDirection chosenExit) {
        //set the next room's position based on exit from current room
        switch (chosenExit) {
            case ExitDirection.North:
                nextRoomPosition = newRoom.transform.position + Vector3.forward * 3;
                break;
            case ExitDirection.East:
                nextRoomPosition = newRoom.transform.position + Vector3.right * 3;
                break;
            case ExitDirection.South:
                nextRoomPosition = newRoom.transform.position + Vector3.back * 3;
                break;
            case ExitDirection.West:
                nextRoomPosition = newRoom.transform.position + Vector3.left * 3;
                break;
        }

        return nextRoomPosition;
    }

    RoomDataObject GetCompatibleRooms (RoomDataObject currentRoom, ExitDirection chosenExit) {
        //get compatible rooms for the current room and exit
        List<RoomDataObject> _currentCompatibleRooms = new List<RoomDataObject> ();
        List<RoomDataObject> _compatibleRoom = new List<RoomDataObject> ();
        List<ExitDirection> _compatibleRoomEntrance = new List<ExitDirection> ();
        foreach (RoomDataObject compatibleRoom in currentRoom.compatibleRooms) {
            //searching through compatible rooms
            _currentCompatibleRooms.Add (compatibleRoom);
            for (int i = 0; i < compatibleRoom.GetEntrances ().Count; i++) {
                if (compatibleRoom.GetEntrances ()[i]) {
                    _compatibleRoomEntrance.Add ((ExitDirection) i); //add the room entrances to list
                }
            }
        }

        int _v = 0;
        foreach (var _room in _compatibleRoomEntrance) {
            // if a room exit aligns with a room entrance, add to compatible room list
            if (chosenExit == ExitDirection.North && _room == ExitDirection.South) { _compatibleRoom.Add (currentRoom.compatibleRooms[_v]); }

            if (chosenExit == ExitDirection.East && _room == ExitDirection.West) { _compatibleRoom.Add (currentRoom.compatibleRooms[_v]); }

            if (chosenExit == ExitDirection.South && _room == ExitDirection.North) { _compatibleRoom.Add (currentRoom.compatibleRooms[_v]); }

            if (chosenExit == ExitDirection.West && _room == ExitDirection.East) { _compatibleRoom.Add (currentRoom.compatibleRooms[_v]); }

            _v++;
        }

        nextRoom = _compatibleRoom[Random.Range (0, _compatibleRoom.Count)]; //choose a compatible room at random
        return nextRoom;
    }
}

public enum ExitDirection {
    //enum for exit directions, can be used for entrance directions as well
    North,
    East,
    South,
    West,
}

public enum SpaceOccupied {
    //enum for positional check in array grid
    Yes,
    No
}