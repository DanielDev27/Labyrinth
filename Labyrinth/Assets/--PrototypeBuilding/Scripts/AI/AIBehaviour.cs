using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour {
    public static AIBehaviour _instance;

    public enum AiStates {
        Idle,
        Chasing,
        Attacking
    }

    [Header ("Debug")]
    [SerializeField] bool coroutineInProgress;

    public AiStates currentAiState;
    [SerializeField] GameObject playerReference;
    [SerializeField] bool isSprinting;

    [Header ("Settings")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Collider searchZone;
    [SerializeField] float fovAngle = 120;
    [SerializeField] Transform eyeLine;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] Animator agentAnimator;


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
                case AiStates.Chasing:
                    //Debug.Log ("Chasing");
                    OnChasing ();
                    break;
                case AiStates.Attacking:
                    //Debug.Log ("Attacking");
                    OnAttack ();
                    break;
            }

        FieldOfViewCheck ();
        OnAnimatorUpdate ();
    }

    IEnumerator OnIdle () {
        coroutineInProgress = true;
        //Debug.Log ("Is Idle");
        yield return new WaitForSeconds (1);
        if (playerReference) {
            currentAiState = AiStates.Chasing;
        }

        coroutineInProgress = false;
    }

    void OnTriggerEnter (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //add a target on a collider enter
        {
            playerReference = other.gameObject;
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //remove a target on a collider exit
        {
            playerReference = null;
        }
    }

    void FieldOfViewCheck () {
        if (playerReference != null) {
            Vector3 directionToTarget = (playerReference.transform.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, directionToTarget) <= fovAngle / 2) {
                float distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
                if (!Physics.Raycast (eyeLine.position, directionToTarget, distanceToPlayer, obstructionLayer)) {
                    currentAiState = AiStates.Chasing;
                } else {
                    currentAiState = AiStates.Idle;
                }
            } else {
                currentAiState = AiStates.Idle;
            }
        }
    }

    void OnChasing () {
        agent.SetDestination (playerReference.transform.position);
        isSprinting = true;
        if (Vector3.Distance (transform.position, playerReference.transform.position) <= 1.0f) {
            agent.isStopped = true;
            isSprinting = false;
            currentAiState = AiStates.Attacking;
        }
    }

    void OnAttack () {
        Debug.Log ("Attack");
        currentAiState = AiStates.Idle;
    }

    void OnAnimatorUpdate () {
        if (isSprinting) {
            agentAnimator.SetFloat ("speed", agent.velocity.sqrMagnitude);
        } else {
            agentAnimator.SetFloat ("speed", Mathf.Clamp (agent.velocity.sqrMagnitude, 0, 1));
        }
    }
}