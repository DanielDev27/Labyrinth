using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class Manager : MonoBehaviour
{
    public static Manager Instance;
    [Header("References - Objects")]
    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;
    [SerializeField] GameObject aiParent;
    [Header("References - UI")]
    [SerializeField] Canvas endFail;
    [SerializeField] Canvas endWin;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] CanvasGroup loadingGroup;
    [Header("First Selections")]
    [SerializeField] GameObject winFirst;
    [SerializeField] GameObject lossFirst;
    [Header("Debug")]
    [SerializeField] int aIs;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        //Set UIs
        endFail.enabled = false;
        endWin.enabled = false;
        //Fetch the number of AIs
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            aIs = aiParent.gameObject.GetComponentsInChildren<AIBehaviour>().Length;
        }
        //Activate Maze
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            FindAnyObjectByType<PCGDungeonGenerator>().Generate();
        }
    }
    public void GameWin()//Logic for winning the game - Triggered after each AI death
    {
        aIs = aiParent.gameObject.GetComponentsInChildren<AIBehaviour>().Length;
        if (aIs <= 0)//Logic only completes if there are no more AIs
        {
            endWin.enabled = true;
            EventSystem.current.SetSelectedGameObject(winFirst);
            CursorSettings(true, CursorLockMode.Confined);
            PlayerController.Instance.enabled = false;
            aiParent.gameObject.SetActive(false);
        }
    }
    public void GameOver()//Logic for losing the game
    {
        camera2.gameObject.SetActive(true);
        endFail.enabled = true;
        EventSystem.current.SetSelectedGameObject(lossFirst);
        CursorSettings(true, CursorLockMode.Confined);
        PlayerController.Instance.enabled = false;
        aiParent.gameObject.SetActive(false);
    }

    public void GoToMain()//Change Scenes to the Main Menu
    {
        Debug.Log("Return to main menu");
        pauseCanvas.enabled = false;
        loadingGroup.alpha = 1.0f;
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(0);
    }

    public void Reload()//Reload the Game Scene to start over
    {
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(1);
    }

    public void Quit()//Quit Application
    {
        Application.Quit();
    }

    void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockMode)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}