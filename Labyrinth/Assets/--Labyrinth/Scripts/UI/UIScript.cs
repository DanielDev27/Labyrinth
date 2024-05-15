using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class UIScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Canvas mainMenuCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] Canvas creditsCanvas;
    [SerializeField] CanvasGroup loadingScreenGroup;
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

    public void GoToLevel(int level)//Changing scenes to the main game level
    {
        mainMenuCanvas.enabled = false;
        loadingScreenGroup.alpha = 1;
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(level);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToControls()//changing canvases to view the controls
    {
        controlsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }

    public void GoToCredits()//changing canvases to see the credits
    {
        creditsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(creditsMenuFirst);
    }

    public void GoToMenu()//changing canvases to go to the main menu
    {
        mainMenuCanvas.enabled = true;
        controlsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
    }

    public void Quit()//Quit application
    {
        Application.Quit();
    }

    private void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockState)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockState;
    }
}