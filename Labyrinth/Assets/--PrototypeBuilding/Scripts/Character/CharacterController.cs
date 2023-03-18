using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour {
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

    [Header ("Debug")]
    [SerializeField] bool isMoving;

    [SerializeField] bool isRunning;
    [SerializeField] bool isDodging;


    //void Awake () { }

    void FixedUpdate () {
        OnPlayerMove ();
        CursorSettings (false, CursorLockMode.Locked);
    }

    void LateUpdate () {
        OnPlayerLook ();
    }

    public void OnMoveInput (InputAction.CallbackContext incomingValue) {
        moveInput = incomingValue.ReadValue<Vector2> ();
        if (incomingValue.performed) {
            isMoving = true;
            animator.SetBool ("isMoving", true);
        } else if (incomingValue.canceled) {
            isMoving = false;
            animator.SetBool ("isMoving", false);
        }
    }

    void OnPlayerMove () {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.forward;
        Vector3 moveCombined = new Vector3 (moveInput.x, 0, moveInput.y);
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
            Debug.Log ("Attack");
        }

        if (incomingValue.canceled) {
            Debug.Log ("Attack Canceled");
        }
    }

    public void OnBlock (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            Debug.Log ("Block");
        }

        if (incomingValue.canceled) {
            Debug.Log ("Block Canceled");
        }
    }

    public void OnDodge (InputAction.CallbackContext incomingValue) {
        if (incomingValue.performed) {
            isDodging = true;
        }

        if (incomingValue.canceled) {
            isDodging = false;
        }
    }
}