using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawnable : MonoBehaviour, ISpawnable {
    public virtual void Spawn () {
        Debug.Log (message:$"[Spawnable] Spawn");
    }
}