using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] RoomDataObject roomDataObject;

    [SerializeField] GameObject roomGameObject;
    // [SerializeField] Vector3 roomPosition;
    // [SerializeField] RoomGenerator roomGenerator;
    // [SerializeField] DoorDirection chosenDoor;
    // [SerializeField] DoorDirection moveDirection;

    public void SetRoomData (RoomDataObject roomDataObject) {
        this.roomDataObject = roomDataObject;
        SetModel ();
    }

    public List<KeyValuePair<RoomDataObject, DoorDirection>> GetNextRooms () {
        List<KeyValuePair<RoomDataObject, DoorDirection>> nextRooms = new List<KeyValuePair<RoomDataObject, DoorDirection>> ();
        if (roomDataObject.doorN) {
            //Debug.Log (message: $"Spawning Door [N]");
            nextRooms.Add (new KeyValuePair<RoomDataObject, DoorDirection> (roomDataObject.GetRandomCompatibleRoom (), DoorDirection.North));
        }

        if (roomDataObject.doorE) {
            //Debug.Log (message: $"Spawning Door [E]");
            nextRooms.Add (new KeyValuePair<RoomDataObject, DoorDirection> (roomDataObject.GetRandomCompatibleRoom (), DoorDirection.East));
        }

        if (roomDataObject.doorS) {
            nextRooms.Add (new KeyValuePair<RoomDataObject, DoorDirection> (roomDataObject.GetRandomCompatibleRoom (), DoorDirection.South));
            //Debug.Log (message: $"Spawning Door [S]");
        }

        if (roomDataObject.doorW) {
            nextRooms.Add (new KeyValuePair<RoomDataObject, DoorDirection> (roomDataObject.GetRandomCompatibleRoom (), DoorDirection.West));
            //Debug.Log (message: $"Spawning Door [W]");
        }

        return nextRooms;
    }

    void SetModel () {
        roomGameObject = Instantiate (roomDataObject.RoomModel, this.transform);
        //Debug.Log ($"Fill Empty Room");
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }
}