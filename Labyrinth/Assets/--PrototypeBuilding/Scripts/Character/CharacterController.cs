using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header ("Debug")]
    [SerializeField] bool isMoving;

    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] float boredCount = 0;
    [SerializeField] bool pause = false;
    [SerializeField] int health;
    public bool isAttack;
    [SerializeField] AIBehaviour ai;
    [SerializeField] Collider viewable;
    [SerializeField] Collider damageable;

    void Awake () {
        Instance = this;
        health = maxHealth;
        healthSystem.UpdateHealth (health);
    }

    void Start () {
        StartCoroutine (IdleBored ());
    }

    void FixedUpdate () {
        OnPlayerMove ();
        CursorSettings (false, CursorLockMode.Locked);
    }

    void LateUpdate () {
        OnPlayerLook ();
        health = healthSystem.GetHealth ();
        healthSystem.UpdateHealth (health);
        if (health <= 0) {
            healthSystem.PlayerDie ();
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
                if (boredAni == 1) {
                    yield return new WaitForSeconds (3.6f);
                }

                if (boredAni == 2) {
                    yield return new WaitForSeconds (7.5f);
                }

                if (boredAni == 3) {
                    yield return new WaitForSeconds (8.6f);
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
        xRotation += lookInput.y * verticalSensitivity;
        yRotation += lookInput.x * horizontalSensitivity;
        xRotation = Mathf.Clamp (xRotation, clampMinValue, clampMaxValue);
        transform.rotation = Quaternion.Euler (0, yRotation, 0);
        cameraHolder.rotation = Quaternion.Euler (xRotation, yRotation, 0);
    }

    public void OnRun (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            isRunning = true;
        } else if (incomingValue.canceled) { isRunning = false; }
    }

    void CursorSettings (bool cursorVisibility, CursorLockMode cursorLockMode) {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }

    public void OnAttack (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed && !isAttack) {
            StartCoroutine (AttackState ());
        }
    }

    IEnumerator AttackState () {
        isAttack = true;
        weapon.StartDamage ();
        boredCount = 0;
        animator.SetBool ("isAttack", true);
        yield return new WaitForSeconds (1);
        animator.SetBool ("isAttack", false);
        //new WaitForSeconds (1 * Time.deltaTime);
        isAttack = false;
        weapon.EndDamage ();
    }

    public void OnBlock (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            //Debug.Log ("Block");
            boredCount = 0;
            animator.SetBool ("isBlock", true);
        }

        if (incomingValue.canceled) {
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
}