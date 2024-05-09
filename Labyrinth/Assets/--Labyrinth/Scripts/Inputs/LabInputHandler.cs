using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class LabInputHandler
{
    public static LabyrinthPlayerInputs labInputs;
    //Events
    public static UnityEvent<Vector2> OnMovePerformed = new UnityEvent<Vector2>();
    public static UnityEvent<Vector2> OnLookPerformed = new UnityEvent<Vector2>();
    public static UnityEvent<bool> OnSprintPerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnDodgePerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnAttackPerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnShieldPerformed = new UnityEvent<bool>();
    public static UnityEvent<bool> OnPausePerformed = new UnityEvent<bool>();
    //Values
    [ShowInInspector] public static Vector2 moveInput;
    [ShowInInspector] public static Vector2 lookInput;
    [ShowInInspector] public static bool sprinting = false;
    [ShowInInspector] public static bool dodging = false;
    [ShowInInspector] public static bool attacking = false;
    [ShowInInspector] public static bool shielding = false;

    static LabInputHandler instance;
    public static LabInputHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LabInputHandler();
            }
            return instance;
        }
        private set { instance = value; }
    }
    public static void Enable()
    {
        Debug.Log("Enable Input Handler");
        if (labInputs == null)
        {
            labInputs = new LabyrinthPlayerInputs();
        }
        RegisterInputs();
        labInputs.Enable();
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
    static void RegisterInputs()
    {
        //Move
        labInputs.Player.Move.performed += MovePerformed;
        labInputs.Player.Move.canceled += MovePerformed;
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
    public static void MovePerformed(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.ReadValue<Vector2>().normalized != Vector2.zero)
        {
            moveInput = incomingValue.ReadValue<Vector2>().normalized;
            Debug.Log("Move");
        }
        if (incomingValue.ReadValue<Vector2>().normalized == Vector2.zero)
        {
            moveInput = Vector2.zero;
        }
        OnMovePerformed?.Invoke(moveInput);
    }
    public static void LookPerformed(InputAction.CallbackContext incomingValue)
    {
        lookInput = incomingValue.ReadValue<Vector2>().normalized;
        //Debug.Log("Look");
        OnLookPerformed?.Invoke(lookInput);
    }
    public static void SprintStarted(InputAction.CallbackContext context)
    {
        sprinting = true;
        Debug.Log("Sprint Start");
        OnSprintPerformed?.Invoke(sprinting);
    }
    public static void SprintCanceled(InputAction.CallbackContext context)
    {
        sprinting = false;
        Debug.Log("Sprint Canceled");
        OnSprintPerformed?.Invoke(sprinting);
    }
    public static void DodgeStarted(InputAction.CallbackContext context)
    {
        dodging = true;
        Debug.Log("Dodge Start");
        OnDodgePerformed?.Invoke(dodging);
    }
    public static void DodgeCanceled(InputAction.CallbackContext context)
    {
        dodging = false;
        Debug.Log("Dodge Canceled");
        OnDodgePerformed?.Invoke(dodging);
    }
    public static void AttackStarted(InputAction.CallbackContext context)
    {
        attacking = true;
        Debug.Log("Attacking Started");
        OnAttackPerformed?.Invoke(attacking);
    }
    public static void AttackCanceled(InputAction.CallbackContext context)
    {
        attacking = false;
        Debug.Log("Attacking Canceled");
        OnAttackPerformed?.Invoke(attacking);
    }
    public static void ShieldStarted(InputAction.CallbackContext context)
    {
        shielding = true;
        Debug.Log("Shield Started");
        OnShieldPerformed?.Invoke(shielding);
    }
    public static void ShieldCanceled(InputAction.CallbackContext context)
    {
        shielding = false;
        Debug.Log("Shield Canceled");
        OnShieldPerformed?.Invoke(shielding);
    }
}
