using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[System.Serializable]
public struct Dungeon {
    public int roomHeight;
    public int roomWidth;
    public int maxInbetweenRooms;
    public int maxExtensionRooms;
    public int width;
    public int height;
    public RoomDataObject startRoom;
    public List<RoomDataObject> endRooms;
    public Room roomHolderPrefab;
    public Transform roomParent;
    public Room baseRoom;
}

public class PCGDundeonGenerator : MonoBehaviour {
    [FormerlySerializedAs ("dungeonSettings")] [SerializeField]
    Dungeon dungeon;
    Room newRoom;
    [Header ("Settings")]
    [SerializeField] Canvas canvasInputs;
    [Header ("Debug Info")]
    [SerializeField] List<Room> roomV2s = new List<Room> ();
    RoomDataObject[] inbetweenRooms;
    int currentInbetweenRooms;
    int currentExtensionRooms;
    SpaceOccupied[,] spaceOccupied;
    SpaceOccupied[,] gridOccupied;
    Vector2Int roomPlacement;
    RoomDataObject currentRoom;
    RoomDataObject previousRoom;
    ExitDirection chosenExit;
    Vector3 nextRoomPosition;
    RoomDataObject nextRoom;
    [SerializeField] List<ExitDirection> possibleDirections;
    List<RoomDataObject> currentCompatibleRooms;
    List<RoomDataObject> previousCompatibleRooms;
    List<RoomDataObject> _previousNext;

    List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    Dictionary<Room, Vector2> dungeonDict = new Dictionary<Room, Vector2> ();

    Dictionary<Vector2, SpaceOccupied> occupiedDict = new Dictionary<Vector2, SpaceOccupied> ();
    float dungeonRoomCount = 0;
    int rooms2VCount = 0;
    int roomParentCount;

    public void ReadInbetweenRoomsInput (string input) {
        dungeon.maxInbetweenRooms = Int32.Parse (input);
    }

    public void ReadWidthInput (string input) {
        dungeon.width = Int32.Parse (input);
    }

    public void ReadHeightRoomsInput (string input) {
        dungeon.height = Int32.Parse (input);
    }

    public void ReadExtesionRoomsInput (string input) {
        dungeon.maxExtensionRooms = Int32.Parse (input);
    }

    [ContextMenu ("Generate")]
    public void Generate () {
        canvasInputs.enabled = false;
        spaceOccupied = new SpaceOccupied[dungeon.width, dungeon.height]; //create new grid array
        gridOccupied = new SpaceOccupied[dungeon.width + 4, dungeon.height + 4];
        for (int x = 0; x < dungeon.width; x++) {
            for (int z = 0; z < dungeon.height; z++) { spaceOccupied[x, z] = SpaceOccupied.No; } //set all indexes to unoccupied
        }

        for (int x = 0; x < dungeon.width + 2; x++) {
            for (int z = 0; z < dungeon.height + 2; z++) { gridOccupied[x, z] = SpaceOccupied.No; } //set all indexes to unoccupied
        }

        roomV2s.Add (dungeon.baseRoom);
        dungeon.baseRoom.SetGridPosition (dungeon.width / 2, -1);
        gridOccupied[dungeon.baseRoom.GetGridPosition ().x + 1, dungeon.baseRoom.GetGridPosition ().y + 1] = SpaceOccupied.Yes;
        if (dungeon.roomParent.childCount > 0) { ClearRooms (); } //Clear rooms on button push if there are rooms

        currentInbetweenRooms = dungeon.maxInbetweenRooms;
        currentExtensionRooms = dungeon.maxExtensionRooms;

        SpawningRooms (dungeon.startRoom, new Vector3 (0, 0, 0), null); //Start generating dungeon
    }

    [ContextMenu ("Clear")]
    void ClearRooms () {
        for (int i = 0; i < dungeon.roomParent.childCount; i++) {
            Destroy (dungeon.roomParent.GetChild (i).gameObject);
        } //Destroy every room and remove from the list while there are rooms

        roomV2s.Clear ();
        rooms2VCount = roomV2s.Count;
        dungeonDict.Clear ();
        dungeonRoomCount = dungeonDict.Count;
        occupiedDict.Clear ();
        roomParentCount = 0;
    }

    void SpawningRooms (RoomDataObject roomData, Vector3 roomPosition, RoomDataObject _previousRoom) {
        //Spawn room based on room DataObject and position (IEnumerator for debug, set waitTime to zero for play)
        previousCompatibleRooms = null;
        previousRoom = _previousRoom;
        if (previousRoom != null) {
            previousCompatibleRooms = _previousRoom.GetCompatibleRooms ();
            _previousNext = new List<RoomDataObject> ();
            foreach (var _room in previousCompatibleRooms) {
                _previousNext.Add (_room);
            }
        }

        if (currentInbetweenRooms >= -1) {
            /**/
            possibleDirections.Clear (); //clear possible directions at the start
            /**/
            roomPlacement = new Vector2Int (((int) roomPosition.x / dungeon.roomWidth) + dungeon.width / 2, (int) roomPosition.z / dungeon.roomHeight); //set the current placement in the grid to the incoming room position in the plane
            /**/
            spaceOccupied[roomPlacement.x, roomPlacement.y] = SpaceOccupied.Yes; //set the spase to Occupied
            gridOccupied[roomPlacement.x + 1, roomPlacement.y + 1] = SpaceOccupied.Yes;
            /**/
            newRoom = null; //empty the new room variable
            currentRoom = roomData; // current room variable set as incoming room data object
            /**/
            newRoom = Instantiate (dungeon.roomHolderPrefab, dungeon.roomParent); //create room parent
            newRoom.SetGridPosition (roomPlacement.x + 1, roomPlacement.y + 1);
            newRoom.SetGridOccupied (gridOccupied[roomPlacement.x + 1, roomPlacement.y + 1]);
            newRoom.SetPreviousRoom (previousRoom);
            /**/
            GetPossibleDirections (currentRoom);

            SetNewRoom (currentRoom, roomPosition);
            if (currentInbetweenRooms >= 0) {
                NextPositionCheck (newRoom, _previousRoom, currentRoom, roomPosition, chosenExit); //check function to see if the next position is viable
                newRoom.SetExit (chosenExit); //choose exit door from possible exit doors
                newRoom.SetPossibleExits (currentRoom, chosenExit);
            }

            if (currentInbetweenRooms > 0) {
                GetNextRoomPosition (newRoom, chosenExit); //get the direction of the exit door
                GetCompatibleRooms (currentRoom, chosenExit); //Get compatible room based on exit door
            }

            if (currentInbetweenRooms == 0) {
                GetNextRoomPosition (newRoom, chosenExit); //get the direction of the exit door, Get compatible end room based on exit door
                SpawnEndRoom (chosenExit); //Spawn an end room
                //Debug.Log ("Ending main branch");
            }

            roomV2s.Add (newRoom); //add to list

            if (currentInbetweenRooms < 0 /*&& currentExtensionRooms >= -1*/) {
                MakeDictionaryRoomPositions ();
                //Debug.Log ("Ending off all other branches");
                currentExtensionRooms = dungeon.maxExtensionRooms;
                DungeonComplete ();
            }
            /**/
            currentInbetweenRooms--;
            if (currentInbetweenRooms >= -1) {
                //restart coroutine whilst there are inbetween rooms
                SpawningRooms (nextRoom, nextRoomPosition, currentRoom);
            }
        }
    }

    void SetNewRoom (RoomDataObject roomDataObject, Vector3 roomPosition) {
        newRoom.transform.position = roomPosition; //set the room position as the incoming room position
        newRoom.SetRoomDataObject (roomDataObject); //set new room with the current room
    }

    List<ExitDirection> GetPossibleDirections (RoomDataObject roomDataObject) {
        List<bool> _exitDoors = roomDataObject.GetExitDoors ();
        /**/
        int i = 0;
        foreach (var exit in _exitDoors) {
            if (exit) {
                ExitDirection _exitDirection = (ExitDirection) i;
                possibleDirections.Add (_exitDirection);
            } // get and add the exits doors for the current room from enum indexes

            i++;
        }

        return possibleDirections;
    }

    ExitDirection NextPositionCheck (Room newRoom, RoomDataObject _previousRoom, RoomDataObject _currentRoom, Vector3 roomPosition, ExitDirection _chosenExit) /*is the next position within the grid*/ {
        if (possibleDirections.Count > 0) {
            chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
            GetNextRoomPosition (newRoom, chosenExit); //get the direction of the exit door
            /**/
            if ((nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) < 0 || (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) >= dungeon.width - 1 || nextRoomPosition.z / dungeon.roomHeight < 0 || nextRoomPosition.z / dungeon.roomHeight >= dungeon.height - 1 || spaceOccupied[(int) (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2), (int) (nextRoomPosition.z / dungeon.roomHeight)] == SpaceOccupied.Yes) {
                /**/
                possibleDirections.Remove (chosenExit); //remove the possible exit that leads out of the grid
                /**/
                if (possibleDirections.Count >= 1) {
                    chosenExit = possibleDirections[Random.Range (0, possibleDirections.Count)]; //choose exit door
                    NextPositionCheck (newRoom, _previousRoom, _currentRoom, roomPosition, _chosenExit); //check function to see if the next position is viable
                }

                if (possibleDirections.Count < 1) {
                    //change the current room for a different compatible room
                    ReplaceCurrentRoom (_currentRoom, _previousRoom, _chosenExit, out RoomDataObject _replaceRoom);
                    newRoom.SetRoomDataObject (_replaceRoom);

                    currentRoom = _replaceRoom;
                    if (currentRoom == dungeon.endRooms[0] || currentRoom == dungeon.endRooms[1] || currentRoom == dungeon.endRooms[2] || currentRoom == dungeon.endRooms[3]) {
                        possibleDirections.Clear ();
                    } else {
                        possibleDirections = GetPossibleDirections (_replaceRoom);
                    }

                    NextPositionCheck (newRoom, _previousRoom, currentRoom, roomPosition, _chosenExit);
                }
            }
        }

        return chosenExit;
    }

    Vector3 GetNextRoomPosition (Room newRoom, ExitDirection chosenExit) {
        //set the next room's position based on exit from current room
        switch (chosenExit) {
            case ExitDirection.North:
                nextRoomPosition = newRoom.transform.position + Vector3.forward * dungeon.roomHeight;
                break;
            case ExitDirection.East:
                nextRoomPosition = newRoom.transform.position + Vector3.right * dungeon.roomWidth;
                break;
            case ExitDirection.South:
                nextRoomPosition = newRoom.transform.position + Vector3.back * dungeon.roomHeight;
                break;
            case ExitDirection.West:
                nextRoomPosition = newRoom.transform.position + Vector3.left * dungeon.roomWidth;
                break;
        }

        return nextRoomPosition;
    }

    void ReplaceCurrentRoom (RoomDataObject _currentRoom, RoomDataObject _previousRoom, ExitDirection _chosenExit, out RoomDataObject replaceRoom) {
        _previousNext.Remove (_currentRoom);
        bool _replaceRoomAcceptable = true;
        replaceRoom = null;
        if (_previousNext.Count == 0) {
            replaceRoom = SpawnEndRoom (_chosenExit);
            currentInbetweenRooms = -2;
        }

        if (_previousNext.Count > 0) {
            RoomDataObject _replaceRoom = null;
            _replaceRoom = _previousNext[Random.Range (0, _previousNext.Count)];
            switch (_chosenExit) {
                case ExitDirection.North:
                    if (_replaceRoom.GetEntrances ()[0] || _replaceRoom.GetEntrances ()[1] || _replaceRoom.GetEntrances ()[3]) {
                        _previousNext.Remove (_replaceRoom);
                        _replaceRoomAcceptable = false;
                    }

                    break;
                case ExitDirection.East:
                    if (_replaceRoom.GetEntrances ()[0] || _replaceRoom.GetEntrances ()[1] || _replaceRoom.GetEntrances ()[2]) {
                        _previousNext.Remove (_replaceRoom);
                        _replaceRoomAcceptable = false;
                    }

                    break;

                case ExitDirection.South:
                    if (_replaceRoom.GetEntrances ()[1] || _replaceRoom.GetEntrances ()[2] || _replaceRoom.GetEntrances ()[3]) {
                        _previousNext.Remove (_replaceRoom);
                        _replaceRoomAcceptable = false;
                    }

                    break;
                case ExitDirection.West:
                    if (_replaceRoom.GetEntrances ()[0] || _replaceRoom.GetEntrances ()[2] || _replaceRoom.GetEntrances ()[3]) {
                        _previousNext.Remove (_replaceRoom);
                        _replaceRoomAcceptable = false;
                    }

                    break;
            }

            if (!_replaceRoomAcceptable) {
                replaceRoom = null;
                ReplaceCurrentRoom (_currentRoom, _previousRoom, _chosenExit, out replaceRoom);
            } else {
                replaceRoom = _replaceRoom;
            }
        }
    }

    RoomDataObject GetCompatibleRooms (RoomDataObject currentRoom, ExitDirection chosenExit) {
        //get compatible rooms for the current room and exit
        currentCompatibleRooms = currentRoom.GetCompatibleRooms ();
        List<RoomDataObject> _compatibleRoom = new List<RoomDataObject> ();
        List<ExitDirection> compatibleRoomEntrance = new List<ExitDirection> ();
        foreach (RoomDataObject compatibleRoom in currentCompatibleRooms) {
            //searching through compatible rooms
            List<bool> _entrances = compatibleRoom.GetEntrances ();
            for (int _j = 0; _j < _entrances.Count; _j++) {
                if (_entrances[_j]) {
                    ExitDirection _compatibleRoomDirection = (ExitDirection) _j;
                    compatibleRoomEntrance.Add ((ExitDirection) _compatibleRoomDirection); //add the room entrances to list
                }
            }
        }

        int _v = 0;
        foreach (ExitDirection _room in compatibleRoomEntrance) {
            // if a room exit aligns with a room entrance, add to compatible room list
            if (chosenExit == ExitDirection.North && _room == ExitDirection.South) { _compatibleRoom.Add (currentRoom.CompatibleRooms[_v]); }

            if (chosenExit == ExitDirection.East && _room == ExitDirection.West) { _compatibleRoom.Add (currentRoom.CompatibleRooms[_v]); }

            if (chosenExit == ExitDirection.South && _room == ExitDirection.North) { _compatibleRoom.Add (currentRoom.CompatibleRooms[_v]); }

            if (chosenExit == ExitDirection.West && _room == ExitDirection.East) { _compatibleRoom.Add (currentRoom.CompatibleRooms[_v]); }

            _v++;
        }

        //Get the next possible Room, Reformat for probability
        nextRoom = _compatibleRoom[Random.Range (0, _compatibleRoom.Count)]; //choose a compatible room at random
        return nextRoom;
    }

    RoomDataObject SpawnEndRoom (ExitDirection chosenExit) {
        switch (chosenExit) {
            case ExitDirection.North:
                nextRoom = dungeon.endRooms[2];
                break;
            case ExitDirection.East:
                nextRoom = dungeon.endRooms[3];
                break;
            case ExitDirection.South:
                nextRoom = dungeon.endRooms[0];
                break;
            case ExitDirection.West:
                nextRoom = dungeon.endRooms[1];
                break;
        }

        newRoom.SetExit (chosenExit);

        return nextRoom;
    }

    void MakeDictionaryRoomPositions () {
        //Debug.Log ("Make room dictionary");
        foreach (Room _roomSlot in roomV2s) {
            _roomSlot.GetRoomDataObject ();
            for (int x = 0; x < dungeon.width + 2; x++) {
                for (int y = 0; y < dungeon.height + 2; y++) {
                    Vector2 _positionVector2 = new Vector2 (x, y);
                    if (_positionVector2 == _roomSlot.GetGridPosition ()) {
                        if (dungeonDict.ContainsKey (_roomSlot)) {
                            continue;
                        }

                        if (occupiedDict.ContainsKey (_positionVector2)) {
                            continue;
                        }

                        dungeonDict.Add (_roomSlot, _roomSlot.GetGridPosition ());
                        gridOccupied[_roomSlot.GetGridPosition ().x, _roomSlot.GetGridPosition ().y] = SpaceOccupied.Yes;
                        occupiedDict.Add (_positionVector2, _roomSlot.GetGridOccupied ());
                        //Debug.Log ($"Evaluating grid position: {_positionVector2}");
                    }
                }
            }
        }

        dungeonRoomCount = dungeonDict.Count;
    }

    void DungeonComplete () {
        foreach (KeyValuePair<Room, Vector2> kvp in dungeonDict) {
            Room _dungRoom = kvp.Key;
            Room _nextRoom;
            List<bool> _validExits = new List<bool> (_dungRoom.GetPossibleExits ());
            if (_validExits.Count != 0) {
                for (int i = 0; i < _validExits.Count; i++) {
                    if (_validExits[i]) {
                        if (currentExtensionRooms > 0) {
                            _nextRoom = SetExtensionRoom (_dungRoom, (ExitDirection) i);
                            _nextRoom.SetPreviousRoom (_dungRoom.GetRoomDataObject ());
                            roomV2s.Add (_nextRoom.GetComponent<Room> ());
                            _validExits[i] = false;
                        }

                        if (currentExtensionRooms <= 0) {
                            _nextRoom = SetExitAtPosition (_dungRoom, (ExitDirection) i, false);
                            _nextRoom.SetPreviousRoom (_dungRoom.GetRoomDataObject ());
                            roomV2s.Add (_nextRoom.GetComponent<Room> ());
                            _validExits[i] = false;
                        }
                    }
                }
            }
        }

        rooms2VCount = roomV2s.Count;
        dungeonRoomCount = dungeonDict.Count;
        currentExtensionRooms--;
        if (currentExtensionRooms > -2) {
            //Debug.Log ("Clear Dictionary");
            dungeonDict = new Dictionary<Room, Vector2> ();
            dungeonRoomCount = dungeonDict.Count;
            occupiedDict = new Dictionary<Vector2, SpaceOccupied> ();
            MakeDictionaryRoomPositions ();
            DungeonComplete ();
        }

        if (currentExtensionRooms <= -2) {
            dungeonDict = new Dictionary<Room, Vector2> ();
            dungeonRoomCount = dungeonDict.Count;
            occupiedDict = new Dictionary<Vector2, SpaceOccupied> ();
            MakeDictionaryRoomPositions ();
            rooms2VCount = roomV2s.Count;
            roomParentCount = dungeon.roomParent.childCount + 1;
            canvasInputs.enabled = true;
        }
    }

    Room SetExtensionRoom (Room dungRoom, ExitDirection exitDirection) {
        Vector3 _roomPosition = GetNextRoomPosition (dungRoom, exitDirection);
        if (gridOccupied[(int) (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) + 1, (int) (nextRoomPosition.z / dungeon.roomHeight) + 1] == SpaceOccupied.No) {
            newRoom = Instantiate (dungeon.roomHolderPrefab, dungeon.roomParent); //create room parent
            // if there are extensions rooms, spawn a new room
            //Debug.Log ("Spawn Extension Room");

            if ((int) _roomPosition.z / dungeon.roomHeight >= 1 && (int) _roomPosition.z / dungeon.roomHeight <= dungeon.height - 1 && (int) (_roomPosition.x / dungeon.roomWidth) >= 1 + (-dungeon.width / 2) && (int) (_roomPosition.x / dungeon.roomWidth) <= -1 + (dungeon.width / 2)) {
                newRoom.SetGridPosition (((int) (_roomPosition.x / dungeon.roomWidth) + dungeon.width / 2) + 1, ((int) _roomPosition.z / dungeon.roomHeight) + 1);
                currentRoom = GetCompatibleRooms (dungRoom.GetRoomDataObject (), exitDirection);
                SetNewRoom (currentRoom, _roomPosition);
                gridOccupied[(int) (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) + 1, (int) (nextRoomPosition.z / dungeon.roomHeight) + 1] = SpaceOccupied.Yes;
                //newRoom.SetSpaceOccupied (gridOccupied[(int) (nextRoomPosition.x / 3 + width / 2) + 1, (int) (nextRoomPosition.z / 3) + 1]);
                //Debug.Log ($"Has Extension Room in {nextRoomPosition / 3}");
                return newRoom;
            } else {
                SetExitAtPosition (newRoom, exitDirection, true);
                //Debug.Log ($"Has End Room in {nextRoomPosition / 3}");
                return newRoom;
            }
        }

        if (newRoom == null) {
            return null;
        }

        return newRoom;
    }

    Room SetExitAtPosition (Room dungRoom, ExitDirection exitDirection, bool extension) {
        Vector3 _roomPosition = GetNextRoomPosition (dungRoom, exitDirection);
        if (gridOccupied[(int) (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) + 1, (int) (nextRoomPosition.z / dungeon.roomHeight) + 1] == SpaceOccupied.No) {
            if (!extension) {
                newRoom = Instantiate (dungeon.roomHolderPrefab, dungeon.roomParent); //create room parent
                newRoom.SetGridPosition (((int) (_roomPosition.x / dungeon.roomWidth) + dungeon.width / 2) + 1, ((int) _roomPosition.z / dungeon.roomHeight) + 1);
            }

            if (extension) {
                dungRoom.SetGridPosition (((int) (_roomPosition.x / dungeon.roomWidth) + dungeon.width / 2) + 1, ((int) _roomPosition.z / dungeon.roomHeight) + 1);
            }


            currentRoom = SpawnEndRoom (exitDirection);
            //Debug.Log ("Spawn End Room");
            SetNewRoom (currentRoom, _roomPosition);
            gridOccupied[(int) (nextRoomPosition.x / dungeon.roomWidth + dungeon.width / 2) + 1, (int) (nextRoomPosition.z / dungeon.roomHeight) + 1] = SpaceOccupied.Yes;
        }

        return newRoom;
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