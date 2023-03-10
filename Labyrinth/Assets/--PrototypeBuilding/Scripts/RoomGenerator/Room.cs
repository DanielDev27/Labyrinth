using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {
    [SerializeField] RoomDataObject roomDataObject;
    [SerializeField] GameObject roomGameObject;

    public void SetRoomData (RoomDataObject roomDataObject, out List<RoomDataObject> nextRooms) {
        this.roomDataObject = roomDataObject;
        SetModel ();
        SpawnDoors (out nextRooms);
    }

    void SetModel () {
        roomGameObject = Instantiate (this.roomDataObject.RoomModel);
        roomGameObject.SetActive (true);
        roomGameObject.transform.parent = this.transform;
    }

    void SpawnDoors (out List<RoomDataObject> nextRooms) {
        nextRooms = new List<RoomDataObject> ();
        if (roomDataObject.doorN) {
            Debug.Log (message: $"Spawning Door [N]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorE) {
            Debug.Log (message: $"Spawning Door [E]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorS) {
            Debug.Log (message: $"Spawning Door [S]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }

        if (roomDataObject.doorW) {
            Debug.Log (message: $"Spawning Door [W]");
            nextRooms.Add (roomDataObject.GetRandomCompatibleRoom ());
        }
    }
}