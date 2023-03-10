using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Room Data", menuName = "Data/Room Data", order = 0)]
public class RoomDataObject : ScriptableObject {
    public GameObject RoomModel;
    [SerializeReference] public List<ISpawnable> spawnables = new List<ISpawnable> ();
}