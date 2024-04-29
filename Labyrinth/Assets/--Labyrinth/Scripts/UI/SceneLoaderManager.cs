using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance;
    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Image loadingProgressBar;
    [SerializeField] bool isActive;
    private void Awake()
    {
        Instance = this;
    }
    public void StartGame(int scene)
    {
        LoadScene(scene);
    }
    public void LoadScene(int sceneID)
    {
        Time.timeScale = 1f;
        StartCoroutine(LoadingSceneAsync(sceneID));
    }
    IEnumerator LoadingSceneAsync(int sceneID)
    {
        loadingScreen?.SetActive(true);
        yield return new WaitForSeconds(1f);
        AsyncOperation _operation = SceneManager.LoadSceneAsync(sceneID);
        while (!_operation.isDone)
        {
            float progress = Mathf.Clamp01(_operation.progress / 1f);
            loadingProgressBar.fillAmount = progress;
            yield return null;
        }
        Time.timeScale = 1f;
    }
}
