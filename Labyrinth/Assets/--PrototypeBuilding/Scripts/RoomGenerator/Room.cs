using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] RoomDataObject roomDataObject;
    [SerializeField] GameObject roomGameObject;
    [SerializeField] Vector3 roomPosition;
    [SerializeField] RoomGenerator roomGenerator;
    [SerializeField] DoorDirection chosenDoor;
    [SerializeField] DoorDirection moveDirection;

    public void SetRoomData (RoomDataObject roomDataObject, DoorDirection _chosenDoor, DoorDirection _moveDirection, out List<RoomDataObject> nextRooms) {
        this.roomDataObject = roomDataObject;
        chosenDoor = _chosenDoor;
        moveDirection = _moveDirection;
        SpawnDoors (out nextRooms);
        SetModel ();
    }

    void SetModel () {
        roomGameObject = Instantiate (roomDataObject.RoomModel, this.transform);
        //Debug.Log ($"Fill Empty Room");
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }

    void SpawnDoors (out List<RoomDataObject> nextRooms) {
        nextRooms = new List<RoomDataObject> ();
        if (roomDataObject.doorN) {
            //Debug.Log (message: $"Spawning Door [N]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorE) {
            //Debug.Log (message: $"Spawning Door [E]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorS) {
            //Debug.Log (message: $"Spawning Door [S]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorW) {
            //Debug.Log (message: $"Spawning Door [W]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }
    }
}