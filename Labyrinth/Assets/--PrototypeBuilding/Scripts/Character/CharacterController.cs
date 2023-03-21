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

    [Header ("Settings")]
    [SerializeField] float speedMultiplier = 5;

    [SerializeField] float horizontalSensitivity = 1;
    [SerializeField] float verticalSensitivity = -1;
    [SerializeField] float clampMinValue = -90;
    [SerializeField] float clampMaxValue = 90;
    [SerializeField] float boredTrigger = 10;

    [Header ("Debug")]
    [SerializeField] bool isMoving;

    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;
    [SerializeField] float boredCount = 0;

    void Awake () {
        Instance = this;
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
        if (incomingValue.performed) {
            //Debug.Log ("Attack");
            boredCount = 0;
            animator.SetBool ("isAttack", true);
        }

        if (incomingValue.canceled) {
            //Debug.Log ("Attack Canceled");
            animator.SetBool ("isAttack", false);
        }
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
}