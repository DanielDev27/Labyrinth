using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    [SerializeField] float boredTrigger = 10;
    [SerializeField] int maxHealth = 20;

    [Header("Debug")]
    [SerializeField] bool usingGamepad;
    [SerializeField] bool isMoving;
    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] float boredCount = 0;
    public int Health;
    public bool IsAttack;
    public bool IsBlock;
    void Awake()
    {
        Instance = this;
        playerInput = new PlayerInput();
        Health = maxHealth;
        healthSystem.UpdateHealth(Health);
    }
    void OnEnable()
    {
        InputHandler.Enable();
    }
    void Start()
    {
        StartCoroutine(IdleBored());//Trigger the Idle counter in order to add bored animations if the player leaves the character inactive
    }
    void Update()
    {
        OnPlayerMove();
    }
    void FixedUpdate()
    {
        InputDeviceCheck();
        OnPlayerLook();
        CursorSettings(false, CursorLockMode.Locked);
    }
    void LateUpdate()
    {
        Health = healthSystem.GetHealth();
        healthSystem.UpdateHealth(Health);
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

    public void OnMoveInput(InputAction.CallbackContext incomingValue)
    {
        moveInput = incomingValue.ReadValue<Vector2>();
        if (incomingValue.performed)
        {
            isMoving = true;
            animator.SetBool("isMoving", true);
            StartCoroutine(IdleBored());
        }
        else if (incomingValue.canceled)
        {
            isMoving = false;
            animator.SetBool("isMoving", false);
            StartCoroutine(IdleBored());
        }
    }

    void OnPlayerMove()//behaviour for player movement
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

    public void OnLookInput(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.ReadValue<Vector2>().normalized != Vector2.zero)
        {
            lookInput = incomingValue.ReadValue<Vector2>().normalized;
            boredCount = 0;
        }
        else
        {
            lookInput = Vector2.zero;
        }
    }

    void OnPlayerLook()//Behaviour for player looking
    {
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

    public void OnRun(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.performed)
        {
            isRunning = true;
        }
        else if (incomingValue.canceled) { isRunning = false; }
    }

    public void OnAttack(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.performed && !IsAttack)
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

    public void OnBlock(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.performed)
        {
            IsBlock = true;
            //Debug.Log ("Block");
            boredCount = 0;
            animator.SetBool("isBlock", true);
            shield.EnableShield();
        }
        if (incomingValue.canceled)
        {
            IsBlock = false;
            //Debug.Log ("Block Canceled");
            animator.SetBool("isBlock", false);
            shield.DisableShield();
        }
    }

    public void OnDodge(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.performed)
        {
            isDodging = true;
            boredCount = 0;
            animator.SetBool("isDodging", true);
        }
        if (incomingValue.canceled)
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