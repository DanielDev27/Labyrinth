using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CharacterController : MonoBehaviour {
    public static CharacterController Instance;
    Vector2 moveInput;
    Vector3 moveDirection;
    Vector2 lookInput;
    float xRotation;
    float yRotation;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] Transform cameraHolder;
    [SerializeField] Animator animator;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] HealthSystem healthSystem;

    [Header ("Settings")]
    [SerializeField] float speedMultiplier = 5;

    [SerializeField] float horizontalSensitivity = 1;
    [SerializeField] float verticalSensitivity = -1;
    [SerializeField] float clampMinValue = -90;
    [SerializeField] float clampMaxValue = 90;
    [SerializeField] float boredTrigger = 10;
    [SerializeField] int maxHealth = 20;
    [SerializeField] Weapon weapon;
    [SerializeField] CinemachineFreeLook cinemachineFreeLook;

    [Header ("Debug")]
    [SerializeField] bool usingGamepad;

    [SerializeField] bool isMoving;
    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] float boredCount = 0;
    [SerializeField] bool pause = false;
    public int Health;
    public bool IsAttack;
    [SerializeField] AIBehaviour ai;
    [SerializeField] Collider viewable;
    [SerializeField] Collider damageable;
    public bool IsBlock;

    void Awake () {
        Instance = this;
        Health = maxHealth;
        healthSystem.UpdateHealth (Health);
    }

    void Start () {
        StartCoroutine (IdleBored ());
    }

    void FixedUpdate () {
        //InputDeviceCheck ();
        OnPlayerMove ();
        CursorSettings (false, CursorLockMode.Locked);
    }

    void LateUpdate () {
        OnPlayerLook ();
        Health = healthSystem.GetHealth ();
        healthSystem.UpdateHealth (Health);
        if (Health <= 0) {
            healthSystem.PlayerDie ();
        }
    }

    void InputDeviceCheck () {
        if (usingGamepad) {
            Debug.Log ("Switched to Gamepad");
        } else {
            Debug.Log ("Switched to mouse/keyboard");
        }
    }

    IEnumerator IdleBored () {
        if (isMoving) {
            animator.SetInteger ("IdleAnimation", 0);
            boredCount = 0;
        } else {
            if (boredCount >= boredTrigger) {
                animator.SetInteger ("IdleAnimation", Random.Range (1, 3));
                int boredAni = animator.GetInteger ("IdleAnimation");
                //Debug.Log ((animator.GetCurrentAnimatorStateInfo (0).length + 0.1f));
                switch (boredAni) {
                    case 1:
                        yield return new WaitForSeconds (3.6f);
                        break;
                    case 2:
                        yield return new WaitForSeconds (7.5f);
                        break;
                    case 3:
                        yield return new WaitForSeconds (8.6f);
                        break;
                }

                animator.SetInteger ("IdleAnimation", 0);
                boredCount = 0;
            }

            if (boredCount < boredTrigger && animator.GetInteger ("IdleAnimation") == 0) {
                yield return new WaitForSeconds (1);
                boredCount += 1;

                StartCoroutine (IdleBored ());
            }
        }
    }

    public void OnMoveInput (InputAction.CallbackContext incomingValue) {
        moveInput = incomingValue.ReadValue<Vector2> ();
        if (incomingValue.performed) {
            isMoving = true;
            animator.SetBool ("isMoving", true);
            StartCoroutine (IdleBored ());
        } else if (incomingValue.canceled) {
            isMoving = false;
            animator.SetBool ("isMoving", false);
            StartCoroutine (IdleBored ());
        }
    }

    void OnPlayerMove () {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.forward;
        Vector3 moveCombined = new Vector3 (moveInput.x, 0, moveInput.y);
        if (moveCombined != Vector3.zero) {
            boredCount = 0;
        }

        if (isRunning) {
            animator.SetFloat ("forward", moveCombined.z * 2, 0.2f, Time.deltaTime);
            animator.SetFloat ("right", moveCombined.x * 2, 0.2f, Time.deltaTime);
        } else {
            animator.SetFloat ("forward", moveCombined.z, 0.2f, Time.deltaTime);
            animator.SetFloat ("right", moveCombined.x, 0.2f, Time.deltaTime);
        }
    }

    public void OnLookInput (InputAction.CallbackContext incomingValue) {
        lookInput = incomingValue.ReadValue<Vector2> ().normalized;
        boredCount = 0;
    }

    void OnPlayerLook () {
        xRotation += 0; //lookInput.y * verticalSensitivity;
        yRotation += lookInput.x * horizontalSensitivity;
        xRotation = Mathf.Clamp (xRotation, clampMinValue, clampMaxValue);
        transform.rotation = Quaternion.Euler (0, yRotation, 0);
        cameraHolder.rotation = Quaternion.Euler (xRotation, yRotation, 0).normalized;
    }

    public void OnRun (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            isRunning = true;
        } else if (incomingValue.canceled) { isRunning = false; }
    }


    public void OnAttack (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed && !IsAttack) {
            StartCoroutine (AttackState ());
        }
    }

    IEnumerator AttackState () {
        IsAttack = true;
        weapon.StartDamage ();
        boredCount = 0;
        animator.SetBool ("isAttack", true);
        yield return new WaitForSeconds (1);
        animator.SetBool ("isAttack", false);
        //new WaitForSeconds (1 * Time.deltaTime);
        IsAttack = false;
        weapon.EndDamage ();
    }

    public void OnBlock (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            IsBlock = true;
            //Debug.Log ("Block");
            boredCount = 0;
            animator.SetBool ("isBlock", true);
        }

        if (incomingValue.canceled) {
            IsBlock = false;
            //Debug.Log ("Block Canceled");
            animator.SetBool ("isBlock", false);
        }
    }

    public void OnDodge (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            isDodging = true;
            boredCount = 0;
            animator.SetBool ("isDodging", true);
        }

        if (incomingValue.canceled) {
            isDodging = false;
            animator.SetBool ("isDodging", false);
        }
    }

    public void OnPause (InputAction.CallbackContext incomingValue) {
        pause = !pause;
        if (pause) {
            pauseCanvas.enabled = pause;
            CursorSettings (true, CursorLockMode.Confined);
            Time.timeScale = 0;
        }

        if (!pause) {
            pauseCanvas.enabled = pause;
            CursorSettings (false, CursorLockMode.Locked);
            Time.timeScale = 1;
        }
    }

    void OnTriggerStay (Collider viewable) {
        viewable.TryGetComponent (out AIBehaviour _aiBehaviour);
        ai = _aiBehaviour;
    }

    void OnTriggerEnter (Collider damageable) {
        if (damageable.TryGetComponent (out WeaponAI _weaponAI) && ai.isAttacking) {
            healthSystem.TakeDamage (5);
        }
    }

    void CursorSettings (bool cursorVisibility, CursorLockMode cursorLockMode) {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}