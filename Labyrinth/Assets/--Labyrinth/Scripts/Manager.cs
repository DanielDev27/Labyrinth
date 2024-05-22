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
    [SerializeField] bool check;
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        //Set UIs
        endFail.enabled = false;
        endWin.enabled = false;
        //Fetch the number of AIs
        aIs = aiParent.gameObject.GetComponentsInChildren<AIBehaviour>().Length;
        //Activate Maze
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            FindAnyObjectByType<PCGDungeonGenerator>().Generate();
        }
    }
    private void Start()
    {
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
                if (endFail.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(lossFirst);
                }
                if (endWin.enabled == true)
                {
                    EventSystem.current.SetSelectedGameObject(winFirst);
                }
            }
        }
        if (endFail.enabled == false && endWin.enabled == false && pauseCanvas.enabled == false && controlsCanvas.enabled == false)
        {
            CursorManager.Instance.CursorOff();
        }
    }
    public void GameWin()//Logic for winning the game - Triggered after each AI death
    {
        aIs = aiParent.gameObject.GetComponentsInChildren<AIBehaviour>().Length;
        if (aIs <= 0)//Logic only completes if there are no more AIs
        {
            PauseScript.Instance.gameOver = true;
            endWin.enabled = true;
            PlayerController.Instance.OnDisable();
            CursorManager.Instance.InputDeviceUIAssign();
            EventSystem.current.SetSelectedGameObject(winFirst);
            aiParent.gameObject.SetActive(false);
        }
    }
    public void GameOver()//Logic for losing the game
    {
        PauseScript.Instance.gameOver = true;
        camera2.gameObject.SetActive(true);
        PlayerController.Instance.OnDisable();
        CursorManager.Instance.InputDeviceUIAssign();
        endFail.enabled = true;
        EventSystem.current.SetSelectedGameObject(lossFirst);
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
        SceneLoaderManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()//Quit Application
    {
        Application.Quit();
    }
}