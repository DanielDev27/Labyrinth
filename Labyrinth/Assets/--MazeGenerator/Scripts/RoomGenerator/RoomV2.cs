using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomV2 : MonoBehaviour {
    [Header ("Debug")]
    [SerializeField] GameObject roomGameObject;

    [SerializeField] ExitDirection chosenExit;
    [SerializeField] List<RoomDataObject> combatibleRooms = new List<RoomDataObject> ();

    public void SetRoomDataObject (RoomDataObject roomDataObject) {
        SetRoomModel (roomDataObject); //prefab model of the room comes from the incoming room DataObject
        for (int i = 0; i < roomDataObject.compatibleRooms.Length; i++) {
            combatibleRooms.Add (roomDataObject.compatibleRooms[i]);
        } //make a list of compatible rooms from the room DataObject
    }

    void SetRoomModel (RoomDataObject roomObject) {
        //set the prefab model from the room DataObject
        roomGameObject = Instantiate (roomObject.RoomModel, this.transform);
        roomGameObject.transform.localPosition = Vector3.zero;
        roomGameObject.SetActive (true);
    }

    public void SetExit (ExitDirection _chosenExit) {
        //debug function to see selected exit
        chosenExit = _chosenExit;
    }
}