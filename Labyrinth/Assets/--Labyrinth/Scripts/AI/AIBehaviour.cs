using System.Collections;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class AIBehaviour : MonoBehaviour
{
    public static AIBehaviour _instance;
    [Header("References")]
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Weapon weapon;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Collider searchZone;
    [SerializeField] Transform eyeLine;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] Animator agentAnimator;
    [Header("Settings")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float fovAngle = 120;
    [SerializeField, ReadOnly] public int maxHealth = 10;
    [Header("Debug")]
    [SerializeField] CharacterController charCont;
    [SerializeField] GameObject playerReference;
    public AiStates currentAiState;
    [SerializeField] bool canSeePlayer;
    [SerializeField] bool coroutineInProgress;
    [SerializeField] bool isSprinting;
    public bool isAttacking = false;
    [SerializeField] bool isDead;
    [SerializeField] int health;

    void Awake()
    {
        _instance = this;
        health = maxHealth;
        coroutineInProgress = false;
        isDead = false;
    }
    void Update()
    {
        FieldOfViewCheck();
        health = healthSystem.UpdateHealth();
        if (health <= 0 && !isDead)
        {
            isDead = true;
            currentAiState = AiStates.Dead;
            healthSystem.EnemyDie();
        }
        if (!coroutineInProgress)
            switch (currentAiState)
            {
                case AiStates.Idle:
                    //Debug.Log ("Idle");
                    StartCoroutine(OnIdle());
                    agentAnimator.SetBool("isMoving", false);
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing");
                    agentAnimator.SetBool("isMoving", true);
                    OnChasing();
                    break;
                case AiStates.Attacking:
                    //Debug.Log ("Attacking");
                    if (!isAttacking)
                    {
                        StartCoroutine(OnAttack());
                    }
                    break;
                case AiStates.Dead:
                    agentAnimator.SetBool("isMoving", false);
                    agentAnimator.SetBool("Dead", true);
                    StartCoroutine(OnDeath());
                    break;
            }
        OnAnimatorUpdate();
    }
    void FieldOfViewCheck()
    {
        if (playerReference != null && !isDead)
        {
            Vector3 directionToTarget = (playerReference.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) <= fovAngle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerReference.transform.position);
                if (!Physics.Raycast(eyeLine.position, directionToTarget, distanceToPlayer, obstructionLayer) && !canSeePlayer)
                {
                    canSeePlayer = true;
                    currentAiState = AiStates.Chasing;
                }
            }
            else
            {
                canSeePlayer = false;
                playerReference = null;
                currentAiState = AiStates.Idle;
            }
        }
    }
    IEnumerator OnIdle()
    {
        coroutineInProgress = true;
        //Debug.Log ("Is Idle");
        if (!isDead)
        {
            yield return new WaitForSeconds(1);
            if (playerReference != null && !isDead)
            {
                currentAiState = AiStates.Chasing;
                coroutineInProgress = false;
            }
            else
            {
                currentAiState = AiStates.Idle;
            }
        }
        else
        {
            currentAiState = AiStates.Dead;
        }
        coroutineInProgress = false;
    }
    void OnChasing()
    {
        if (playerReference != null && !isDead)
        {
            transform.LookAt(playerReference.transform.position);
            agent.SetDestination(playerReference.transform.position);
            if (Vector3.Distance(transform.position, playerReference.transform.position) > 2.5f)
            {
                agent.isStopped = false;
                agent.speed = sprintSpeed;
                isSprinting = true;
            }
            if (Vector3.Distance(transform.position, playerReference.transform.position) <= 2.5f)
            {
                agent.isStopped = false;
                agent.speed = walkSpeed;
                isSprinting = false;
            }
            if (Vector3.Distance(transform.position, playerReference.transform.position) <= 1f)
            {
                agent.isStopped = true;
                agentAnimator.SetBool("isMoving", false);
                isSprinting = false;
                currentAiState = AiStates.Attacking;
            }
        }
        else { currentAiState = AiStates.Idle; }
    }
    IEnumerator OnAttack()
    {
        isAttacking = true;
        //Debug.Log ("Attack");
        transform.LookAt(playerReference.transform.position);
        agentAnimator.SetBool("isAttacking", true);
        float attackTime = agentAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(attackTime);
        agentAnimator.SetBool("isAttacking", false);
        isAttacking = false;
        currentAiState = AiStates.Chasing;
    }

    IEnumerator OnDeath()
    {
        coroutineInProgress = true;
        yield return new WaitForSeconds(6);
        coroutineInProgress = false;
    }
    void OnAnimatorUpdate()
    {
        if (isSprinting)
        {
            agentAnimator.SetFloat("speed", Mathf.Clamp(agent.velocity.sqrMagnitude, 1, 2), 0.05f, Time.deltaTime);
        }
        else if (!isDead)
        {
            agentAnimator.SetFloat("speed", Mathf.Clamp(agent.velocity.sqrMagnitude, 0, 1), 0.05f, Time.deltaTime);
        }
        else if (isDead)
        {
            agentAnimator.SetFloat("speed", Mathf.Clamp(agent.velocity.sqrMagnitude, 0, 0), 0.05f, Time.deltaTime);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterController _characterController)) //add a target on a collider enter
        {
            charCont = _characterController;
            playerReference = other.gameObject;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out CharacterController _characterController)) //keep a target on a collider stay
        {
            charCont = _characterController;
            playerReference = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CharacterController _characterController)) //remove a target on a collider exit
        {
            playerReference = null;
            charCont = null;
        }
    }
    public enum AiStates
    {
        Idle,
        Chasing,
        Attacking,
        Dead,
    }
}