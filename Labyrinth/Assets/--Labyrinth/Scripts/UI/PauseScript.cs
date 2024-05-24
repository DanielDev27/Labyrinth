using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class PauseScript : MonoBehaviour
{
    public static PauseScript Instance;
    [Header("References")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] CanvasGroup loadingGroup;
    [SerializeField] public bool pause;
    [Header("First Selections")]//First buttons selected on each menu
    [SerializeField] GameObject pauseMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;
    [Header("Debug")]
    [SerializeField] bool check;
    [SerializeField] public bool gameOver;
    public UnityEvent<bool> pauseEvent = new UnityEvent<bool>();
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        controlsCanvas.enabled = false;
        CursorManager.Instance.InputDeviceUIAssign();
        check = CursorManager.Instance.usingGamepad;
    }
    void FixedUpdate()
    {
        if (check != CursorManager.Instance.usingGamepad)
        {
            CursorManager.Instance.InputDeviceUIAssign();
            check = CursorManager.Instance.usingGamepad;
            if (check)
            {
                if (pauseCanvas.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
                }
                if (controlsCanvas.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
                }
            }
        }
        if (pauseCanvas.enabled == false && controlsCanvas.enabled == false && !gameOver)
        {
            CursorManager.Instance.CursorOff();
        }
    }
    //Pause Input Logic
    public void OnPause(InputAction.CallbackContext incomingValue)
    {
        if (incomingValue.performed)
        {
            if (!gameOver)
            {
                pause = !pause;
                if (pause)
                {
                    pauseEvent.Invoke(pause);
                    //Turn Off Player Controller Input Listeners
                    PlayerController.Instance.OnDisable();
                    //Turn On Canvas elements
                    pauseCanvas.enabled = pause;
                    EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
                    CursorManager.Instance.InputDeviceUIAssign();
                }
                if (!pause)
                {
                    //Turn On Player Controller Input Listeners
                    PlayerController.Instance.OnEnable();
                    //Turn Off Canvas elements
                    pauseCanvas.enabled = pause;
                    EventSystem.current.SetSelectedGameObject(null);
                    CursorManager.Instance.CursorOff();
                    pauseEvent.Invoke(pause);
                }
            }
        }
    }
    public void GoToControls()
    {
        controlsCanvas.enabled = true;
        pauseCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }
    public void GoToPause()
    {
        pauseCanvas.enabled = true;
        controlsCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
    }
}