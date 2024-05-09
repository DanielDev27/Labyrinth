using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using Unity.Collections;
using Sirenix.OdinInspector;

public class CharacterController : MonoBehaviour
{
    public static CharacterController Instance;
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] Vector2 lookInput;
    float yRotation;
    [Header("References")]
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Animator animator;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] Weapon weapon;
    [SerializeField] Shield shield;
    [SerializeField] CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] LabyrinthPlayerInputs labInputs;
    [ShowInInspector] LabInputHandler labInputHandler;
    [SerializeField] AIBehaviour ai;
    [SerializeField] Collider viewable;
    [SerializeField] Collider damageable;

    [Header("Settings")]
    [SerializeField] float speedMultiplier;
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] float horizontalSensitivity;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float controllerSensitivity;
    [SerializeField] float boredTrigger;
    [SerializeField, Unity.Collections.ReadOnly] public int maxHealth;

    [Header("Debug")]
    [SerializeField] bool usingGamepad;
    [SerializeField] bool isMoving;
    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] float boredCount;
    public int Health;
    public bool IsAttack;
    public bool IsBlock;
    void Awake()
    {
        Debug.Log("Awake Character Controller");
        Instance = this;
        Health = maxHealth;
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
    void OnEnable()
    {
        LabInputHandler.Enable();
        Debug.Log("Initialized");
        LabInputHandler.OnMovePerformed.AddListener(InputMove);
        LabInputHandler.OnLookPerformed.AddListener(InputLook);
        LabInputHandler.OnSprintPerformed.AddListener(OnRun);
        LabInputHandler.OnDodgePerformed.AddListener(OnDodge);
        LabInputHandler.OnAttackPerformed.AddListener(OnAttack);
        LabInputHandler.OnShieldPerformed.AddListener(OnBlock);

    }
    void OnDisable()
    {
        LabInputHandler.OnMovePerformed.RemoveListener(InputMove);
        LabInputHandler.OnLookPerformed.RemoveListener(InputLook);
        LabInputHandler.OnSprintPerformed.RemoveListener(OnRun);
        LabInputHandler.OnDodgePerformed.RemoveListener(OnDodge);
        LabInputHandler.OnAttackPerformed.RemoveListener(OnAttack);
        LabInputHandler.OnShieldPerformed.RemoveListener(OnBlock);
    }
    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            OnPlayerMove();
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
        Health = healthSystem.UpdateHealth();
        if (Health <= 0)
        {
            healthSystem.PlayerDie();
        }
    }
    void InputDeviceCheck()
    {
        usingGamepad = playerInput.currentControlScheme == "Gamepad";
    }
    IEnumerator IdleBored()//Behaviour for bored animations if the player leaves the character inactive
    {
        if (isMoving)
        {
            animator.SetInteger("IdleAnimation", 0);
            boredCount = 0;
        }
        else
        {
            if (boredCount >= boredTrigger)
            {
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
            {
                yield return new WaitForSeconds(1);
                boredCount += 1;
                StartCoroutine(IdleBored());
            }
        }
    }
    void InputMove(Vector2 _moveInput)
    {
        this.moveInput = _moveInput;
        if (moveInput != Vector2.zero)
        {
            isMoving = true;
            animator.SetBool("isMoving", true);
            StartCoroutine(IdleBored());
        }
        else if (moveInput == Vector2.zero)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
            StartCoroutine(IdleBored());
        }
    }
    public void OnPlayerMove()//behaviour for player movement
    {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.forward;
        Vector3 moveCombined = new Vector3(moveInput.x, 0, moveInput.y);
        if (moveCombined != Vector3.zero && !IsBlock && !IsAttack)
        {
            boredCount = 0;
            playerRigidbody.velocity = new Vector3(moveDirection.x * speedMultiplier, 0, moveDirection.z * speedMultiplier);
            if (isRunning)
            {
                speedMultiplier = runSpeed;
                animator.SetFloat("forward", moveCombined.z * 2, 0.2f, Time.deltaTime);
                animator.SetFloat("right", moveCombined.x * 2, 0.2f, Time.deltaTime);
            }
            else
            {
                speedMultiplier = walkSpeed;
                animator.SetFloat("forward", moveCombined.z, 0.2f, Time.deltaTime);
                animator.SetFloat("right", moveCombined.x, 0.2f, Time.deltaTime);
            }
        }
        else
        {
            playerRigidbody.velocity = Vector3.zero;
            animator.SetFloat("forward", 0);
            animator.SetFloat("right", 0);
        }
    }
    void InputLook(Vector2 lookInput)
    {
        this.lookInput = lookInput;
    }
    void OnPlayerLook()//Behaviour for player looking
    {
        if (lookInput != Vector2.zero)
        {
            boredCount = 0;
        }
        else
        {
            lookInput = Vector2.zero;
        }
        if (usingGamepad)
        {
            horizontalSensitivity = controllerSensitivity;
        }
        else
        {
            horizontalSensitivity = mouseSensitivity;
        }
        yRotation += lookInput.x * horizontalSensitivity;
        this.transform.rotation = Quaternion.Euler(0, yRotation, 0).normalized;
    }

    public void OnRun(bool sprinting)
    {
        isRunning = sprinting;
    }

    public void OnAttack(bool attacking)
    {
        if (attacking && !IsAttack)
        {
            StartCoroutine(AttackState());
        }
    }
    IEnumerator AttackState()
    {
        IsAttack = true;
        boredCount = 0;
        animator.SetBool("isAttack", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("isAttack", false);
        IsAttack = false;
    }

    public void OnBlock(bool blocking)
    {
        if (blocking)
        {
            IsBlock = true;
            //Debug.Log ("Block");
            boredCount = 0;
            animator.SetBool("isBlock", true);
            shield.EnableShield();
        }
        if (!blocking)
        {
            IsBlock = false;
            //Debug.Log ("Block Canceled");
            animator.SetBool("isBlock", false);
            shield.DisableShield();
        }
    }

    public void OnDodge(bool dodging)
    {
        if (dodging)
        {
            isDodging = true;
            boredCount = 0;
            animator.SetBool("isDodging", true);
        }
        if (!dodging)
        {
            isDodging = false;
            animator.SetBool("isDodging", false);
        }
    }

    void OnTriggerStay(Collider viewable)
    {
        viewable.TryGetComponent(out AIBehaviour _aiBehaviour);
        ai = _aiBehaviour;
    }

    void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockMode)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}