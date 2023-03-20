using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour {
    public static AIBehaviour _instance;
    NavMeshAgent agent;

    public enum AiStates {
        Idle,
        Search,
        Chasing,
        Attacking
    }

    //[SerializeField] GameObject aiAvatar;

    [SerializeField] bool coroutineInProgress;
    public AiStates currentAiState;
    [SerializeField] Collider searchZone;
    [SerializeField] List<CharacterController> charInRange = new List<CharacterController> ();
    [SerializeField] bool haveTarget;

    void Awake () {
        _instance = this;
    }

    void Update () {
        if (!coroutineInProgress)
            switch (currentAiState) {
                case AiStates.Idle:
                    //Debug.Log ("Idle");
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Search:
                    //Debug.Log ("Searching");
                    StartCoroutine (OnSearch ());
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing");
                    OnChasing ();
                    break;
                case AiStates.Attacking:
                    //Debug.Log ("Attacking");
                    break;
            }
    }

    IEnumerator OnIdle () {
        coroutineInProgress = true;
        Debug.Log ("Is Idle");
        yield return new WaitForSeconds (1);
        currentAiState = AiStates.Search;
        coroutineInProgress = false;
    }

    IEnumerator OnSearch () {
        coroutineInProgress = true;
        if (charInRange.Count > 0) {
            Debug.Log ("Is Searching");
            yield return new WaitForSeconds (1);
            currentAiState = AiStates.Chasing;
            coroutineInProgress = false;
        } else if (charInRange.Count == 0) {
            yield return new WaitForSeconds (1);
            currentAiState = AiStates.Idle;
            coroutineInProgress = false;
        }
    }

    private void OnTriggerEnter (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //add a target on a collider enter
        {
            charInRange.Add (_characterController);
        }
    }

    private void OnTriggerExit (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //remove a target on a collider exit
        {
            charInRange.Remove (_characterController);
        }
    }

    void OnChasing () { }
}