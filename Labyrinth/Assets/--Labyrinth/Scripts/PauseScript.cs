using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class PauseScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] bool pause = false;
    [Header("First Selections")]
    [SerializeField] GameObject pauseMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;

    void Start()
    {
        controlsCanvas.enabled = false;
        CursorSettings(true, CursorLockMode.Confined);
    }
    public void OnPause(InputAction.CallbackContext incomingValue)
    {
        pause = !pause;
        if (pause)
        {
            CharacterController.Instance.enabled = false;
            pauseCanvas.enabled = pause;
            EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
            CursorSettings(true, CursorLockMode.Confined);
            Time.timeScale = 0;
        }

        if (!pause)
        {
            CharacterController.Instance.enabled = true;
            pauseCanvas.enabled = pause;
            EventSystem.current.SetSelectedGameObject(null);
            CursorSettings(false, CursorLockMode.Locked);
            Time.timeScale = 1;
        }
    }

    public void GoToMain()
    {
        SceneManager.LoadScene(0);
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

    public void Reload()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockState)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockState;
    }
}