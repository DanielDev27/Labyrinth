using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomV2 : MonoBehaviour {
    [SerializeField] RoomDataObject roomDataObject;
    [SerializeField] GameObject roomGameObject;

    public void GetNextRooms () { }

    public void SetRoomDataObject (RoomDataObject roomObject) {
        roomDataObject = roomObject;
        SetRoomModel (roomObject);
    }

    void SetRoomModel (RoomDataObject roomObject) {
        roomGameObject = Instantiate (roomObject.RoomModel, this.transform);
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }
}