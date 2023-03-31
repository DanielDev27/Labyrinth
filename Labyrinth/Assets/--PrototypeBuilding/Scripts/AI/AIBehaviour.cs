using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour {
    public static AIBehaviour _instance;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] WeaponAI weapon;

    [Header ("Debug")]
    [SerializeField] bool coroutineInProgress;

    public AiStates currentAiState;
    [SerializeField] GameObject playerReference;
    [SerializeField] bool isSprinting;
    [SerializeField] bool canSeePlayer;
    public bool isAttacking = false;
    [SerializeField] int health;
    [SerializeField] CharacterController charCont;

    [Header ("Settings")]
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Collider searchZone;
    [SerializeField] float fovAngle = 120;
    [SerializeField] Transform eyeLine;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] Animator agentAnimator;
    [SerializeField] int maxHealth = 10;

    void Awake () {
        _instance = this;
        health = maxHealth;
        healthSystem.UpdateHealth (health);
    }

    void Update () {
        FieldOfViewCheck ();
        if (!coroutineInProgress)
            switch (currentAiState) {
                case AiStates.Idle:
                    //Debug.Log ("Idle");
                    StartCoroutine (OnIdle ());
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing");
                    agentAnimator.SetBool ("isMoving", true);
                    OnChasing ();
                    break;
                case AiStates.Attacking:
                    //Debug.Log ("Attacking");
                    if (!isAttacking) {
                        StartCoroutine (OnAttack ());
                    }

                    break;
            }

        OnAnimatorUpdate ();
    }

    void LateUpdate () {
        health = healthSystem.GetHealth ();
        healthSystem.UpdateHealth (health);
        if (health <= 0) {
            healthSystem.EnemyDie ();
        }
    }

    IEnumerator OnIdle () {
        coroutineInProgress = true;
        //Debug.Log ("Is Idle");
        yield return new WaitForSeconds (1);
        if (playerReference != null) {
            currentAiState = AiStates.Chasing;
            coroutineInProgress = false;
        } else {
            currentAiState = AiStates.Idle;
        }

        coroutineInProgress = false;
    }


    void FieldOfViewCheck () {
        if (playerReference != null) {
            Vector3 directionToTarget = (playerReference.transform.position - transform.position).normalized;
            if (Vector3.Angle (transform.forward, directionToTarget) <= fovAngle / 2) {
                float distanceToPlayer = Vector3.Distance (transform.position, playerReference.transform.position);
                if (!Physics.Raycast (eyeLine.position, directionToTarget, distanceToPlayer, obstructionLayer) && !canSeePlayer) {
                    canSeePlayer = true;
                    currentAiState = AiStates.Chasing;
                }
            } else {
                canSeePlayer = false;
                playerReference = null;
                currentAiState = AiStates.Idle;
            }
        }
    }

    void OnChasing () {
        if (playerReference != null) {
            transform.LookAt (playerReference.transform.position);
            agent.SetDestination (playerReference.transform.position);
            if (Vector3.Distance (transform.position, playerReference.transform.position) > 5f) {
                agent.isStopped = false;
                isSprinting = true;
            }

            if (Vector3.Distance (transform.position, playerReference.transform.position) <= 5f) {
                agent.isStopped = false;
                isSprinting = false;
            }

            if (Vector3.Distance (transform.position, playerReference.transform.position) <= 2.5f) {
                agent.isStopped = true;
                agentAnimator.SetBool ("isMoving", false);
                isSprinting = false;
                currentAiState = AiStates.Attacking;
            }
        } else { currentAiState = AiStates.Idle; }
    }

    IEnumerator OnAttack () {
        isAttacking = true;
        weapon.canDamage = true;
        //Debug.Log ("Attack");
        transform.LookAt (playerReference.transform.position);
        agentAnimator.SetBool ("isAttacking", true);
        yield return new WaitForSeconds (1);
        agentAnimator.SetBool ("isAttacking", false);
        isAttacking = false;
        weapon.canDamage = false;
        currentAiState = AiStates.Idle;
    }

    void OnAnimatorUpdate () {
        if (isSprinting) {
            agentAnimator.SetFloat ("speed", Mathf.Clamp (agent.velocity.sqrMagnitude, 1, 2));
        } else {
            agentAnimator.SetFloat ("speed", Mathf.Clamp (agent.velocity.sqrMagnitude, 0, 1));
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //add a target on a collider enter
        {
            charCont = _characterController;
            playerReference = other.gameObject;
        }
    }

    void OnTriggerStay (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //add a target on a collider enter
        {
            charCont = _characterController;
            playerReference = other.gameObject;
        }
    }

    void OnTriggerExit (Collider other) {
        if (other.TryGetComponent (out CharacterController _characterController)) //remove a target on a collider exit
        {
            playerReference = null;
            charCont = null;
        }
    }

    public enum AiStates {
        Idle,
        Chasing,
        Attacking
    }
}