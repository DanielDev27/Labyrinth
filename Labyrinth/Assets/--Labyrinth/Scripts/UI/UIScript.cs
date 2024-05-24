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
    [Header("Debug")]
    [SerializeField] bool check;
    void Start()
    {
        mainMenuCanvas.enabled = true;
        controlsCanvas.enabled = false;
        creditsCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
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
                if (mainMenuCanvas.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(mainMenuFirst);
                }
                if (controlsCanvas.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
                }
                if (creditsCanvas.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(creditsMenuFirst);
                }
            }
        }
        if (mainMenuCanvas.enabled == false && controlsCanvas.enabled == false && creditsCanvas.enabled == false)
        {
            CursorManager.Instance.CursorOff();
        }
    }
    public void GoToLevel(int level)//Changing scenes to the main game level
    {
        Debug.Log("Go to game");
        mainMenuCanvas.enabled = false;
        loadingScreenGroup.alpha = 1;
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(level);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void GoToControls()//changing canvases to view the controls
    {
        Debug.Log("Go to controls");
        controlsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(controlsMenuFirst);
    }

    public void GoToCredits()//changing canvases to see the credits
    {
        Debug.Log("Go to credits");
        creditsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        EventSystem.current.SetSelectedGameObject(creditsMenuFirst);
    }

    public void GoToMenu()//changing canvases to go to the main menu
    {
        mainMenuCanvas.enabled = true;
        EventSystem.current.SetSelectedGameObject(mainMenuFirst);
        if (controlsCanvas.enabled == true)
        {
            Debug.Log("Go to main menu");
            controlsCanvas.enabled = false;
        }
        if (creditsCanvas.enabled == true)
        {
            Debug.Log("Go to main menu");
            creditsCanvas.enabled = false;
        }
    }

    public void Quit()//Quit application
    {
        Application.Quit();
    }
}