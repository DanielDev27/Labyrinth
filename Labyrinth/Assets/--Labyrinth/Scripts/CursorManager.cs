using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;
    [SerializeField] public bool usingGamepad;
    [SerializeField] public PlayerInput playerInput;
    private void Awake()
    {
        Instance = this;
    }
    private void FixedUpdate()
    {
        InputDeviceCheck();
    }
    //Switch input device based on player inputs - references control schemes
    void InputDeviceCheck()
    {
        usingGamepad = playerInput.currentControlScheme == "Gamepad";
    }
    public void InputDeviceUIAssign()
    {
        if (usingGamepad)
        {
            CursorSettings(false, CursorLockMode.Confined);
        }
        else
        {
            CursorSettings(true, CursorLockMode.Confined);
        }
    }
    public void CursorOff() { CursorSettings(false, CursorLockMode.Confined); }
    public void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockMode)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}
