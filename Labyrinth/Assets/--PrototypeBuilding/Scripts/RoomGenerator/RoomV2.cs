using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomV2 : MonoBehaviour {
    //public static RoomV2 InstanceRoom;
    [SerializeField] RoomDataObject roomDataObject;
    [SerializeField] GameObject roomGameObject;

    [Header ("Debug")]
    [SerializeField] ExitDirection chosenEntrance;

    void Awake () {
        //InstanceRoom = this;
    }

    public void SetRoomDataObject (RoomDataObject roomDataObject) {
        this.roomDataObject = roomDataObject;
        SetRoomModel (roomDataObject);
    }

    void SetRoomModel (RoomDataObject roomObject) {
        roomGameObject = Instantiate (roomObject.RoomModel, this.transform);
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }

    public void SetExit (ExitDirection chosenExit) {
        chosenEntrance = chosenExit;
    }
}