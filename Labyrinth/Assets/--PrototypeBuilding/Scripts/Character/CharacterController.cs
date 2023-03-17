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

    [Header ("Settings")]
    [SerializeField] float speedMultiplier = 5;

    [SerializeField] float horizontalSensitivity = 1;
    [SerializeField] float verticalSensitivity = -1;
    [SerializeField] float clampMinValue = -90;
    [SerializeField] float clampMaxValue = 90;


    //void Awake () { }

    void FixedUpdate () {
        OnPlayerMove ();
    }

    void LateUpdate () {
        OnPlayerLook ();
    }

    public void OnMoveInput (InputAction.CallbackContext incomingValue) {
        moveInput = incomingValue.ReadValue<Vector2> ();
    }

    void OnPlayerMove () {
        moveDirection = moveInput.x * transform.right + moveInput.y * transform.forward;

        Vector3 moveCombined = playerRigidbody.position + new Vector3 (moveDirection.x, 0, moveDirection.z) * (speedMultiplier * Time.fixedDeltaTime);
        playerRigidbody.MovePosition (moveCombined);
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
}