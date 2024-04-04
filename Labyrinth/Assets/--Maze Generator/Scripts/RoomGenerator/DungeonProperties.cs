using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DungeonProperties {
    public int roomHeight;
    public int roomWidth;
    public float waitTime;
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