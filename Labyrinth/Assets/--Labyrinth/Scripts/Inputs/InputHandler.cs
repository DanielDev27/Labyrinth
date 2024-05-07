using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    static InputHandler instance;
    public static InputHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputHandler();
            }
            return instance;
        }
        private set { instance = value; }
    }
    static LabyrinthPlayerInputs labInputs;
    //Events
    public static event EventHandler<Vector2> OnMovePerformed;
    public static event EventHandler<Vector2> OnLookPerformed;
    public static event EventHandler<bool> OnSprintPerformed;
    public static event EventHandler<bool> OnDodgePerformed;
    public static event EventHandler<bool> OnAttackPerformed;
    public static event EventHandler<bool> OnShieldPerformed;
    public static event EventHandler<bool> OnPausePerformed;
    //Values
    [ShowInInspector] public static Vector2 moveInput;
    [ShowInInspector] public static Vector2 lookInput;
    [ShowInInspector] public static bool sprinting = false;
    [ShowInInspector] public static bool dodging = false;
    [ShowInInspector] public static bool attacking = false;
    [ShowInInspector] public static bool shielding = false;
    public static void Enable()
    {
        labInputs ??= new LabyrinthPlayerInputs();
        labInputs.Enable();
        Instance.RegisterInputs();
    }
    public static void Disable()
    {
        if (labInputs == null)
        {
            return;
        }
        labInputs.Disable();
    }
    public static void Dispose()
    {
        if (labInputs == null)
        {
            return;
        }
        labInputs.Dispose();
    }
    void RegisterInputs()
    {
        //Move
        labInputs.Player.Move.performed += MovePerformed;
        //Look
        labInputs.Player.Look.performed += LookPerformed;
        //Sprint
        labInputs.Player.Sprint.started += SprintStarted;
        labInputs.Player.Sprint.canceled += SprintCanceled;
        //Dodge
        labInputs.Player.Dodge.started += DodgeStarted;
        labInputs.Player.Dodge.canceled += DodgeCanceled;
        //Attack
        labInputs.Player.Attack.started += AttackStarted;
        labInputs.Player.Attack.canceled += AttackCanceled;
        //Shield
        labInputs.Player.Shield.started += ShieldStarted;
        labInputs.Player.Shield.canceled += ShieldCanceled;
    }
    public void MovePerformed(InputAction.CallbackContext incomingValue)
    {
        moveInput = incomingValue.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(this, moveInput);
    }
    public void LookPerformed(InputAction.CallbackContext incomingValue)
    {
        lookInput = incomingValue.ReadValue<Vector2>();
        OnLookPerformed?.Invoke(this, lookInput);
    }
    public void SprintStarted(InputAction.CallbackContext context)
    {
        sprinting = true;
        OnSprintPerformed?.Invoke(this, sprinting);
    }
    public void SprintCanceled(InputAction.CallbackContext context)
    {
        sprinting = false;
        OnSprintPerformed?.Invoke(this, sprinting);
    }
    public void DodgeStarted(InputAction.CallbackContext context)
    {
        dodging = true;
        OnDodgePerformed?.Invoke(this, dodging);
    }
    public void DodgeCanceled(InputAction.CallbackContext context)
    {
        dodging = false;
        OnDodgePerformed?.Invoke(this, dodging);
    }
    public void AttackStarted(InputAction.CallbackContext context)
    {
        attacking = true;
        OnAttackPerformed?.Invoke(this, attacking);
    }
    public void AttackCanceled(InputAction.CallbackContext context)
    {
        attacking = false;
        OnAttackPerformed?.Invoke(this, attacking);
    }
    public void ShieldStarted(InputAction.CallbackContext context)
    {
        shielding = true;
        OnShieldPerformed?.Invoke(this, shielding);
    }
    public void ShieldCanceled(InputAction.CallbackContext context)
    {
        shielding = false;
        OnShieldPerformed?.Invoke(this, shielding);
    }
}
