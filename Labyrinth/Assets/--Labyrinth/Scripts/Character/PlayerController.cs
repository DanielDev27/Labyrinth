using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("Debug")]//Values that are useful visual checks
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector2 lookInput;
    float yRotation;
    [SerializeField] bool usingGamepad;
    [SerializeField] bool isMoving;
    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] bool isLockedOn;
    [SerializeField] float boredCount;
    public int Health;
    public bool IsAttack;
    [SerializeField] int attackCount = 0;
    public bool IsBlock;
    [Header("References")]//Objects that are required from other scripts
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Animator animator;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Weapon weapon;
    [SerializeField] Shield shield;
    //[SerializeField] CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] LabyrinthPlayerInputs labInputs;
    LabInputHandler labInputHandler;
    [SerializeField] public AIBehaviour ai;
    [Header("Settings")]//Settings required for the script's functioning
    [SerializeField] float speedMultiplier;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] float horizontalSensitivity;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float controllerSensitivity;
    [SerializeField] float boredTrigger;
    [SerializeField, Unity.Collections.ReadOnly] public int maxHealth;
    [Header("Anim Settings")]//Settings/References specifically for some animations
    [SerializeField] AnimationClip attackAnimSlash;
    [SerializeField] AnimationClip attackAnimBack;
    [SerializeField] AnimationClip dodgeAnim;
    [SerializeField] float dodgeCooldown;

    void Awake()
    {
        //Debug.Log("Awake Character Controller");
        Instance = this;
        Health = maxHealth;
        //Instantiate classes required by PlayerController
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        if (labInputHandler == null)
        {
            labInputHandler = new LabInputHandler();
        }
        if (labInputs == null)
        {
            labInputs = LabInputHandler.labInputs;
        }
    }
    void Start()
    {
        StartCoroutine(IdleBored());//Trigger the Idle counter in order to add bored animations if the player leaves the character inactive
    }
    void OnEnable()//Add event listeners for inputs
    {
        LabInputHandler.Enable();
        Debug.Log("Initialized");
        LabInputHandler.OnMovePerformed.AddListener(InputMove);
        LabInputHandler.OnLookPerformed.AddListener(InputLook);
        LabInputHandler.OnSprintPerformed.AddListener(OnRun);
        LabInputHandler.OnDodgePerformed.AddListener(OnDodge);
        LabInputHandler.OnAttackPerformed.AddListener(OnAttack);
        LabInputHandler.OnShieldPerformed.AddListener(OnBlock);
        LabInputHandler.OnLockOnPerformed.AddListener(OnLockOn);
    }
    void OnDisable()//Remove event listeners for inputs
    {
        LabInputHandler.OnMovePerformed.RemoveListener(InputMove);
        LabInputHandler.OnLookPerformed.RemoveListener(InputLook);
        LabInputHandler.OnSprintPerformed.RemoveListener(OnRun);
        LabInputHandler.OnDodgePerformed.RemoveListener(OnDodge);
        LabInputHandler.OnAttackPerformed.RemoveListener(OnAttack);
        LabInputHandler.OnShieldPerformed.RemoveListener(OnBlock);
        LabInputHandler.OnLockOnPerformed.RemoveListener(OnLockOn);
    }
    private void Update()
    {
        if (moveInput != Vector2.zero)//Player can move when there is an input
        {
            OnPlayerMove();
        }
        if (ai == null)
        {
            isLockedOn = false;
        }
    }
    void FixedUpdate()
    {
        OnPlayerLook();
        InputDeviceCheck();
        CursorSettings(false, CursorLockMode.Locked);
    }
    void LateUpdate()
    {
        //Update Health values and systems
        Health = healthSystem.UpdateHealth();
        if (Health <= 0)
        {
            PlayerDie();
        }
    }
    void InputDeviceCheck()
    {//Switch input device based on player inputs - references control schemes
        usingGamepad = playerInput.currentControlScheme == "Gamepad";
    }
    IEnumerator IdleBored()//Behaviour for bored animations if the player leaves the character inactive
    {
        if (isMoving || IsAttack || isDodging || IsBlock)
        {//When moving the bored counter resets to zero
            animator.SetInteger("IdleAnimation", 0);
            boredCount = 0;
        }
        else
        {
            if (boredCount >= boredTrigger)
            {//Once triggered, one of 3 Idle animations will play
                animator.SetInteger("IdleAnimation", Random.Range(1, 3));
                int boredAni = animator.GetInteger("IdleAnimation");
                //Debug.Log ((animator.GetCurrentAnimatorStateInfo (0).length + 0.1f));
                switch (boredAni)
                {
                    case 1:
                        yield return new WaitForSeconds(3.6f);
                        break;
                    case 2:
                        yield return new WaitForSeconds(7.5f);
                        break;
                    case 3:
                        yield return new WaitForSeconds(8.6f);
                        break;
                }
                animator.SetInteger("IdleAnimation", 0);
                boredCount = 0;
            }
            if (boredCount < boredTrigger && animator.GetInteger("IdleAnimation") == 0)
            {//Count up bored count each second whilst the normal Idle Animation is active
                yield return new WaitForSeconds(1);
                boredCount += 1;
                StartCoroutine(IdleBored());
            }
        }
    }
    void InputMove(Vector2 _moveInput)
    {//Move input listener logic
        this.moveInput = _moveInput;
        if (moveInput != Vector2.zero && !IsAttack)
        {
            isMoving = true;
            animator.SetBool("isMoving", true);
            StartCoroutine(IdleBored());
        }
        else
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
            StartCoroutine(IdleBored());
        }
    }
    public void OnPlayerMove()//behaviour for player movement
    {
        //Set the move direction relative to the player
        moveDirection = moveInput.x * Camera.main.transform.right + moveInput.y * Camera.main.transform.forward;
        //Combine the moveInput into a V3
        Vector3 moveCombined = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveCombined != Vector3.zero && !IsAttack)
        {//Logic for when player can move
            playerAvatar.transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
            boredCount = 0;
            playerRigidbody.velocity = new Vector3(moveDirection.x * speedMultiplier, 0, moveDirection.z * speedMultiplier);
            if (isRunning)
            {//Running modifiers
                speedMultiplier = runSpeed;
                animator.SetFloat("speed", moveCombined.magnitude * 2, 0.2f, Time.deltaTime);
            }
            else
            {//Normal Walking Modifiers
                speedMultiplier = walkSpeed;
                animator.SetFloat("speed", moveCombined.magnitude, 0.2f, Time.deltaTime);
            }
            if (isDodging)
            {//Dodging Modifiers
                speedMultiplier = dodgeSpeed;
                dodgeCooldown = dodgeAnim.length;
                StartCoroutine(DodgeCoolDown());
            }
        }
        else
        {//When Player can't move
            playerRigidbody.velocity = Vector3.zero;
            animator.SetFloat("speed", 0);
        }
    }
    void InputLook(Vector2 lookInput)
    {//Look Input listener logic
        this.lookInput = lookInput;
    }
    void OnPlayerLook()//Behaviour for player looking
    {
        //Logic for bored
        if (lookInput != Vector2.zero)
        {
            boredCount = 0;
        }
        else
        {
            lookInput = Vector2.zero;
        }
        //Logic for input sensetivity
        if (usingGamepad)
        {
            horizontalSensitivity = controllerSensitivity;
        }
        else
        {
            horizontalSensitivity = mouseSensitivity;
        }
        //Logic for Freelook camera
        if (isLockedOn && ai != null)
        {
            cameraHolder.transform.LookAt(new Vector3(ai.transform.position.x, 0.5f, ai.transform.position.z));
        }
        else
        {
            yRotation += lookInput.x * horizontalSensitivity;
            cameraHolder.transform.rotation = Quaternion.Euler(0, yRotation, 0).normalized;
        }
    }
    public void OnRun(bool sprinting)//Run Input listener
    {
        isRunning = sprinting;
    }
    public void OnAttack(bool attacking)//Attack Input listener
    {
        if (attacking && !IsAttack)
        {
            //Reset Bored counter
            boredCount = 0;
            StartCoroutine(AttackState());
        }
    }
    IEnumerator AttackState()//Attack Logic
    {
        //Set attack true
        IsAttack = true;
        animator.SetTrigger("attackTrigger");
        animator.SetBool("isAttack", true);
        //Stop movement
        isMoving = false;
        animator.SetBool("isMoving", false);
        //Combo logic
        if (attackCount == 0)
        {
            float attackAnimTime = attackAnimSlash.averageDuration;
            yield return new WaitForSeconds(attackAnimTime - 0.5f);
            IsAttack = false;
            yield return new WaitForSeconds(0.5f);
            SetAttackFalse();
        }
        if (attackCount == 1)
        {
            float attackAnimTime = attackAnimBack.averageDuration;
            yield return new WaitForSeconds(attackAnimTime - 0.5f);
            IsAttack = false;
            yield return new WaitForSeconds(0.5f);
            SetAttackFalse();
        }
        attackCount++;
    }
    void SetAttackFalse()
    {
        //yield return new WaitForEndOfFrame();
        attackCount = 0;
        if (!IsAttack)
        {
            animator.SetBool("isAttack", false);//Exit Attack state
            if (moveInput != Vector2.zero)
            {//Turn movement back on
                isMoving = true;
                animator.SetBool("isMoving", true);
            }
        }
    }
    public void OnBlock(bool blocking)//Block Input logic
    {
        if (blocking)
        {
            IsBlock = true;
            //Debug.Log ("Block");
            boredCount = 0;
            animator.SetLayerWeight(1, 1);
            shield.EnableShield();
        }
        if (!blocking)
        {
            IsBlock = false;
            //Debug.Log ("Block Canceled");
            animator.SetLayerWeight(1, 0);
            shield.DisableShield();
        }
    }

    public void OnDodge(bool dodging)//Dodging Input Logic
    {
        if (dodging)
        {
            isDodging = true;
            boredCount = 0;
            animator.SetTrigger("dodgeTrigger");
            if (isDodging && moveInput == Vector2.zero)
            {//Dodging Modifiers
                speedMultiplier = dodgeSpeed;
                animator.SetFloat("speed", 0);
                dodgeCooldown = dodgeAnim.length;
                playerRigidbody.velocity = playerAvatar.transform.forward * -speedMultiplier;
                StartCoroutine(DodgeCoolDown());
            }
            //animator.SetBool("isDodging", true);
        }
    }
    IEnumerator DodgeCoolDown()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        if (moveInput == Vector2.zero)
        {
            playerRigidbody.velocity = Vector3.zero;
        }
        isDodging = false;
    }
    public void OnLockOn(bool lockOn)
    {
        isLockedOn = lockOn;
    }
    public void PlayerDie()//Triggering Death for Player
    {
        animator.SetBool("Dead", true);
        StartCoroutine(PlayerDeath());
    }
    IEnumerator PlayerDeath()
    {//Wait before removing gameobject
        playerRigidbody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(5);
        Manager.Instance.GameOver();
        animator.SetBool("Dead", false);
        this.gameObject.SetActive(false);
    }
    
    //Cursor logic
    void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockMode)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}