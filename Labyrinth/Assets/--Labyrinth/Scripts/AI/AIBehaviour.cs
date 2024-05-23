using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviour : MonoBehaviour
{
    public static AIBehaviour _instance;
    [Header("Debug")]//Useful values for visual debug
    [SerializeField] public Vector2 spawnPosition;
    [SerializeField] PlayerController charCont;
    [SerializeField] public GameObject playerReference;
    public AiStates currentAiState;
    [SerializeField] Vector3 directionToTarget;
    [SerializeField] float distanceToPlayer;
    [SerializeField] float visionCheck;
    [SerializeField] float startingDistanceToPlayer;
    [SerializeField] bool canSeePlayer;
    [SerializeField] bool coroutineInProgress;
    [SerializeField] bool isSprinting;
    public bool isAttacking;
    [SerializeField] bool isDead;
    [SerializeField] int health;
    [SerializeField] public bool immune = false;
    [SerializeField] float timerCD;
    [Header("References")]//References to necessary systems and objects
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Weapon weapon;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform eyeLine;
    [SerializeField] LayerMask obstructionLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Animator agentAnimator;
    [Header("Attack Anim Clips")]
    [SerializeField] AnimationClip attack0;
    [SerializeField] AnimationClip attack1;
    [SerializeField] AnimationClip attack2;
    [SerializeField] AnimationClip attack3;
    [SerializeField] AnimationClip attack4;
    [SerializeField] AnimationClip attack5;
    [Header("Settings")]//Settings for system function
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float fovAngle = 120;
    [SerializeField] float fovDistance;
    [SerializeField] public int maxHealth = 10;
    [SerializeField] float attackCD;

    void Awake()
    {
        _instance = this;
        health = maxHealth;
        coroutineInProgress = false;
        isDead = false;
        healthSystem.takeDamageEnemy.AddListener(LookToPlayer);
        charCont = FindObjectOfType<PlayerController>();
        playerReference = charCont.gameObject;
    }
    void Update()
    {
        if (!immune) { FieldOfViewCheck(); }
        //Health Checks
        health = healthSystem.UpdateHealth();
        if (health <= 0 && !isDead)
        {
            isDead = true;
            currentAiState = AiStates.Dead;
            EnemyDie();
        }
        if (currentAiState == AiStates.Idle || currentAiState == AiStates.Chasing)
        {
            timerCD += Time.deltaTime;
        }
        if (PauseScript.Instance.pause)
        {
            currentAiState = AiStates.Idle;
        }
        //State Switch
        if (!coroutineInProgress)
            switch (currentAiState)
            {
                case AiStates.Idle:
                    //Debug.Log ("Idle");
                    OnAnimatorUpdate();
                    StartCoroutine(OnIdle());
                    agentAnimator.SetBool("isMoving", false);
                    break;
                case AiStates.Chasing:
                    //Debug.Log ("Chasing");
                    OnAnimatorUpdate();
                    agentAnimator.SetBool("isMoving", true);
                    OnChasing();
                    break;
                case AiStates.Attacking:
                    //Debug.Log("Attacking");
                    if (!isAttacking)
                    {
                        isAttacking = true;
                        agentAnimator.SetFloat("speed", 0);
                        agentAnimator.SetBool("isMoving", false);
                        agentAnimator.SetBool("isAttacking", true);
                        StartCoroutine(OnAttack());
                    }
                    break;
                case AiStates.Dead:
                    agentAnimator.SetBool("isMoving", false);
                    agentAnimator.SetBool("Dead", true);
                    agentAnimator.SetTrigger("DeadTrigger");
                    StartCoroutine(OnDeath());
                    break;
            }
    }
    void FieldOfViewCheck()//Logic to check if player is within visual range of AI
    {
        Debug.DrawRay(transform.position + transform.up, transform.forward * 10, Color.blue);
        if (playerReference != null && !isDead)//Can see player and AI is alive
        {
            RaycastHit _hit;
            //Player reference values
            Vector3 playerPosition = playerReference.transform.position;
            directionToTarget = playerPosition - transform.position;
            distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
            //Vision Check
            visionCheck = Vector3.Angle(transform.forward, directionToTarget);
            if (visionCheck <= (fovAngle / 2) && distanceToPlayer <= fovDistance && !PauseScript.Instance.pause)
            {
                //Does AI see the player
                bool _hitLayer = Physics.Raycast(transform.position, directionToTarget, out _hit, 100, obstructionLayer, QueryTriggerInteraction.Ignore);
                //Does AI see player without obstruction
                if (_hitLayer && _hit.collider.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
                {
                    transform.LookAt(playerReference.transform.position);
                    Debug.DrawRay(transform.position, directionToTarget * 100, Color.green);
                    canSeePlayer = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, directionToTarget, Color.red);
                    canSeePlayer = false;
                }
            }
            else
            {
                Debug.DrawRay(eyeLine.position, directionToTarget, Color.black);
                canSeePlayer = false;
            }
        }
    }
    void OnAnimatorUpdate()//Logic to change Animator states for movement
    {
        if (!isDead)
        {
            if (isSprinting)
            {
                agentAnimator.SetFloat("speed", Mathf.Clamp(agent.velocity.sqrMagnitude, 1, 2), 0.05f, Time.deltaTime);
            }
            if (!isSprinting)
            {
                agentAnimator.SetFloat("speed", Mathf.Clamp(agent.velocity.sqrMagnitude, 0, 1), 0.05f, Time.deltaTime);
            }
            if (isAttacking)
            {
                agentAnimator.SetFloat("speed", 0);
            }
        }
        if (isDead)
        {
            agentAnimator.SetFloat("speed", 0);
        }
    }
    IEnumerator OnIdle()//Idle Logic
    {
        coroutineInProgress = true;
        if (playerReference != null)
        {
            startingDistanceToPlayer = Vector3.Distance(transform.position, playerReference.transform.position);
        }
        //Debug.Log("Is Idle");
        if (!PauseScript.Instance.pause)
        {

            if (!isDead && !isAttacking)//AI is fully Idle
            {
                yield return new WaitForSeconds(1);
                if (playerReference != null && canSeePlayer)
                {//AI can see player -> chase
                    currentAiState = AiStates.Chasing;
                    coroutineInProgress = false;
                }
                else
                {//AI cannot see player -> Idle
                    currentAiState = AiStates.Idle;
                }
            }
            else//AI is not Idle but not chasing
            {
                if (isDead)//Dead switch
                {
                    currentAiState = AiStates.Dead;
                }
                if (isAttacking)//Attack switch
                {
                    currentAiState = AiStates.Attacking;
                }
            }
        }
        else
        {
            currentAiState = AiStates.Idle;
            isSprinting = false;
            isAttacking = false;
            agent.isStopped = true;
        }
        coroutineInProgress = false;
    }
    void OnChasing()//Chasing logic
    {
        if (playerReference != null && !isDead && canSeePlayer)//Player target exists and is visible
        {
            agent.SetDestination(playerReference.transform.position);
            //Player target is far away
            //if (Vector3.Distance(transform.position, playerReference.transform.position) > 10f)
            if (startingDistanceToPlayer > 5f)
            {
                isSprinting = true;
                agent.speed = sprintSpeed;
                agent.isStopped = false;
            }
            //Player target is close
            if (startingDistanceToPlayer <= 5f)
            {
                isSprinting = false;
                agent.speed = walkSpeed;
                agent.isStopped = false;
            }
            //Reached Player target
            if (Vector3.Distance(transform.position, playerReference.transform.position) <= 2f && timerCD >= attackCD)
            {
                isSprinting = false;
                agent.isStopped = true;
                agentAnimator.SetBool("isMoving", false);
                currentAiState = AiStates.Attacking;
            }
            //In Cooldown
            if (timerCD < attackCD)
            {
                agent.isStopped = true;
                agentAnimator.SetBool("isMoving", false);
                isSprinting = false;
            }
        }
        //Can't see Player Target
        else { currentAiState = AiStates.Idle; }
    }
    IEnumerator OnAttack()//Attack Logic
    {
        //Debug.Log("Attack");
        //Turn to Player Target
        transform.LookAt(playerReference.transform.position);
        //Stop movement
        agentAnimator.SetBool("isMoving", false);
        GetComponent<Rigidbody>().isKinematic = true;
        //Attack animation
        int animClipNum = Random.Range(0, 6);
        agentAnimator.SetFloat("attack Num", animClipNum);
        float attackTime;
        switch (animClipNum)
        {
            case 0:
                attackTime = attack0.length;
                break;
            case 1:
                attackTime = attack1.length;
                break;
            case 2:
                attackTime = attack2.length;
                break;
            case 3:
                attackTime = attack3.length;
                break;
            case 4:
                attackTime = attack4.length;
                break;
            case 5:
                attackTime = attack5.length;
                break;
            default:
                attackTime = 1;
                break;
        }
        yield return new WaitForSeconds(attackTime);
        //Reset values
        agentAnimator.SetBool("isAttacking", false);
        GetComponent<Rigidbody>().isKinematic = false;
        isAttacking = false;
        timerCD = 0;
        currentAiState = AiStates.Chasing;
        if (playerReference != null)
        {
            startingDistanceToPlayer = Vector3.Distance(transform.position, playerReference.transform.position);
        }
    }
    IEnumerator OnDeath()//Death Logic
    {
        PlayerController.Instance.aiTarget = null;
        coroutineInProgress = true;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(6);
        coroutineInProgress = false;
    }
    public void EnemyDie()//Triggering Death for AI
    {
        StartCoroutine(EnemyDespawn());
    }
    IEnumerator EnemyDespawn()
    {//Wait before despawning gameobject
        yield return new WaitForSeconds(5);
        agentAnimator.SetBool("Dead", false);
        this.gameObject.SetActive(false);
        Manager.Instance.GameWin();
        PlayerController.Instance.CheckEnemies();
    }
    public void StartImmune()
    {
        immune = true;
    }
    public void StopImmune()
    {
        immune = false;
    }
    void LookToPlayer()
    {
        transform.LookAt(playerReference.transform.position);
        agentAnimator.SetTrigger("damage");
    }
    public enum AiStates//AI States for Logic
    {
        Idle,
        Chasing,
        Attacking,
        Dead,
    }
}