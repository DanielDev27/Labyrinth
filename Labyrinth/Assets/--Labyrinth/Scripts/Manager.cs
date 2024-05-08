using UnityEngine;
using UnityEngine.EventSystems;
public class Manager : MonoBehaviour
{
    public static Manager Instance;
    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;
    [SerializeField] CharacterController characterController;
    [SerializeField] Canvas endFail;
    [SerializeField] Canvas endWin;
    [SerializeField] GameObject aiParent;
    [SerializeField] Canvas pauseCanvas;
    [SerializeField] Canvas controlsCanvas;
    [SerializeField] CanvasGroup loadingGroup;
    [Header("First Selections")]
    [SerializeField] GameObject winFirst;
    [SerializeField] GameObject lossFirst;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        endFail.enabled = false;
        endWin.enabled = false;
    }
    private void OnEnable()
    {
        HealthSystem.Instance.AIDeath.AddListener(GameWin);
    }
    private void OnDisable()
    {
        HealthSystem.Instance.AIDeath.RemoveListener(GameWin);
    }

    void GameWin()
    {
        if (aiParent.gameObject.GetComponentsInChildren<AIBehaviour>().Length <= 0)
        {
            endWin.enabled = true;
            EventSystem.current.SetSelectedGameObject(winFirst);
            CursorSettings(true, CursorLockMode.Confined);
            characterController.enabled = false;
            aiParent.gameObject.SetActive(false);
        }
    }
    public void GameOver()
    {
        camera2.gameObject.SetActive(true);
        endFail.enabled = true;
        EventSystem.current.SetSelectedGameObject(lossFirst);
        CursorSettings(true, CursorLockMode.Confined);
        characterController.enabled = false;
        aiParent.gameObject.SetActive(false);
    }

    public void GoToMain()
    {
        Debug.Log("Return to main menu");
        pauseCanvas.enabled = false;
        loadingGroup.alpha = 1.0f;
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(0);
    }

    public void Reload()
    {
        Time.timeScale = 1;
        SceneLoaderManager.Instance.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    void CursorSettings(bool cursorVisibility, CursorLockMode cursorLockMode)
    {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}