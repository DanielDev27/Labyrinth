using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] public GameObject roomGameObject;
    [SerializeField] ExitDirection chosenExit;
    [SerializeField] RoomDataObject currentRoom;
    [SerializeField] public Vector2Int gridPosition;
    [SerializeField] RoomDataObject previousRoom;
    [SerializeField] Vector2Int previousRoomGridPosition;

    [FormerlySerializedAs("spaceOccupied")]
    [SerializeField] SpaceOccupied gridOccupied;
    [SerializeField] List<bool> exits;
    [SerializeField] List<RoomDataObject> combatibleRoomsList = new List<RoomDataObject>();
    [Header("References")]
    [SerializeField] PCGDungeonGenerator dungeonGenerator;
    private void Awake()
    {
        dungeonGenerator = FindObjectOfType<PCGDungeonGenerator>();
    }
    public void SetRoomDataObject(RoomDataObject roomDataObject)
    {
        if (roomGameObject != null)
        {
            roomGameObject.SetActive(false);
            roomGameObject = null;
        }

        combatibleRoomsList.Clear();
        currentRoom = roomDataObject;
        SetRoomModel(roomDataObject); //prefab model of the room comes from the incoming room DataObject
        exits = roomDataObject.GetExitDoors();
        for (int i = 0; i < roomDataObject.CompatibleRooms.Count; i++)
        {
            combatibleRoomsList.Add(roomDataObject.CompatibleRooms[i]);
        } //make a list of compatible rooms from the room DataObject
    }

    public RoomDataObject GetRoomDataObject()
    {
        return currentRoom;
    }

    void SetRoomModel(RoomDataObject roomObject)
    {
        //set the prefab model from the room DataObject
        roomGameObject = Instantiate(roomObject.RoomModel, this.transform);
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive(true);
    }

    public void SetPreviousRoom(RoomDataObject _previousRoom, Vector3 _previousRoomPosition)
    {
        previousRoom = _previousRoom;
        previousRoomGridPosition = new Vector2Int(((int)_previousRoomPosition.x / dungeonGenerator.dungeon.roomWidth)
        + dungeonGenerator.dungeon.width / 2 + 1, (int)_previousRoomPosition.z / dungeonGenerator.dungeon.roomHeight + 1);
    }

    public void SetExit(ExitDirection _chosenExit)
    {
        //debug function to alter selected exit
        chosenExit = _chosenExit;
    }

    public void SetPossibleExits(RoomDataObject roomDataObject, ExitDirection chosenExit)
    {
        exits[(int)chosenExit] = false;
    }

    public List<bool> GetPossibleExits()
    {
        return exits;
    }

    public void SetGridPosition(int _gridX, int _gridY) { gridPosition = new Vector2Int(_gridX, _gridY); }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    //public void SetSpacePosition (int _gridX, int _gridY) { spacePosition = new Vector2Int (_gridX, _gridY + 1); }
    public void SetGridOccupied(SpaceOccupied _spaceOccupied) { gridOccupied = _spaceOccupied; }
    public SpaceOccupied GetGridOccupied() { return gridOccupied; }
}