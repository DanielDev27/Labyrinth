using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class UIScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] Canvas creditsCanvas;
    [Header("First Selections")]
    [SerializeField] GameObject mainMenuFirst;
    [SerializeField] GameObject controlsMenuFirst;
    [SerializeField] GameObject creditsMenuFirst;
    void Start()
    {
        mainMenuCanvas.enabled = true;
        controlsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
        CursorSettings(true, CursorLockMode.Confined);
    }

    public void GoToLevel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToControls()
    {
        controlsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }

    public void GoToCredits()
    {
        creditsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(creditsMenuFirst);
    }

    public void GoToMenu()
    {
        mainMenuCanvas.enabled = true;
        controlsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
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