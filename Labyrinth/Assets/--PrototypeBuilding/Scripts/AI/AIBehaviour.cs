using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

public class AIBehaviour : MonoBehaviour {
    NavMeshAgent agent;

    public enum AiStates {
        Idle,
        Search,
        Chasing,
        Attacking
    }

    public AiStates currentAiState;
    [SerializeField] Collider searchZone;


    void Update () {
        switch (currentAiState) {
            case AiStates.Idle:
                Debug.Log ("Idle");
                StartCoroutine (OnIdle ());
                break;
            case AiStates.Search:
                Debug.Log ("Searching");
                OnSearch ();
                break;
            case AiStates.Chasing:
                Debug.Log ("Chasing");
                OnChasing ();
                break;
            case AiStates.Attacking:
                Debug.Log ("Attacking");
                break;
        }
    }

    IEnumerator OnIdle () {
        yield return new WaitForSeconds (0.2f);
        currentAiState = AiStates.Search;
    }

    void OnSearch () {
        OnTriggerEnter (searchZone);
        if (OnTriggerEnter (searchZone)) {
            currentAiState = AiStates.Chasing;
        } else {
            currentAiState = AiStates.Idle;
        }
    }

    bool OnTriggerEnter (Collider searchZone) {
        if (searchZone.CompareTag ("Player")) {
            Debug.Log ("Character in range");
            return true;
        } else {
            Debug.Log ("No target");
            return false;
        }
    }

    void OnChasing () { }
}