using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomV2 : MonoBehaviour {
    //public static RoomV2 InstanceRoom;
    [SerializeField] RoomDataObject roomDataObject;
    [SerializeField] GameObject roomGameObject;

    [Header ("Debug")]
    [SerializeField] ExitDirection chosenExit;

    [SerializeField] List<RoomDataObject> _combatableRooms;

    void Awake () {
        //InstanceRoom = this;
        _combatableRooms = new List<RoomDataObject> ();
    }

    public void SetRoomDataObject (RoomDataObject roomDataObject) {
        this.roomDataObject = roomDataObject;
        SetRoomModel (roomDataObject);
        for (int i = 0; i < this.roomDataObject.compatibleRooms.Length; i++) {
            _combatableRooms.Add(this.roomDataObject.compatibleRooms[i]);
        }
    }

    void SetRoomModel (RoomDataObject roomObject) {
        roomGameObject = Instantiate (roomObject.RoomModel, this.transform);
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }

    public void SetExit (ExitDirection _chosenExit) {
        chosenExit = _chosenExit;
    }
}