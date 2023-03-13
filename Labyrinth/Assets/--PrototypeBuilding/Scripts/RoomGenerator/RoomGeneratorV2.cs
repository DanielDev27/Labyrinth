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

    [Header ("Debug Info")]
    [SerializeField] List<ExitDirection> possibleDirections;

    [SerializeField] ExitDirection chosenExit;
    [SerializeField] ExitDirection moveDirection;

    [SerializeField] RoomDataObject nextRoom;

    [Header ("Input Info")]
    [SerializeField] RoomDataObject startRoom;

    [SerializeField] RoomDataObject[] endRooms;
    [SerializeField] Room roomPrefab;
    [SerializeField] int maxInbetweenRooms;
    [SerializeField] int maxBranches = 1;


    [SerializeField] RoomDataObject[] inbetweenRooms;
    [SerializeField] List<Vector2Int> spawnedCoordinates = new List<Vector2Int> ();

    void Awake () {
        Instance = this;
    }

    [ContextMenu ("Generate Rooms")]
    void Generate () {
        StartCoroutine (SpawnRoom (startRoom));
    }

    IEnumerator SpawnRoom (RoomDataObject roomData) {
        yield return new WaitForSeconds (1);
        StartCoroutine (SpawnRoom(nextRoom));
    }
}