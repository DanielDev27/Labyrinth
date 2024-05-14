using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class PauseScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] CanvasGroup loadingGroup;
    [SerializeField] bool pause;
    [Header("First Selections")]//First buttons selected on each menu
    [SerializeField] GameObject pauseMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;

    void Start()
    {
        controlsCanvas.enabled = false;
        CursorSettings(true, CursorLockMode.Confined);
    }
    //Pause Input Logic
    public void OnPause(InputAction.CallbackContext incomingValue)
    {
        pause = !pause;
        if (pause)
        {
            //Turn Off Player Controller
            PlayerController.Instance.enabled = false;
            //Turn On Canvas elements
            pauseCanvas.enabled = pause;
            EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
            CursorSettings(true, CursorLockMode.Confined);
            Time.timeScale = 0;
        }

        if (!pause)
        {
            //Turn On Player Controller
            PlayerController.Instance.enabled = true;
            //Turn Off Canvas elements
            pauseCanvas.enabled = pause;
            EventSystem.current.SetSelectedGameObject(null);
            CursorSettings(false, CursorLockMode.Locked);
            Time.timeScale = 1;
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

    private void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockState)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockState;
    }
}