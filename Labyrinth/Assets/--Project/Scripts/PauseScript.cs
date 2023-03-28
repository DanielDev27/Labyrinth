using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] Canvas pauseMenu;
    [SerializeField] Canvas controlsMenu;

    void Start () {
        //pauseMenu.enabled = true;
        controlsMenu.enabled = false;
        CursorSettings (true, CursorLockMode.Confined);
    }

    public void GoToMain () {
        SceneManager.LoadScene (0);
    }

    public void GoToControls () {
        controlsMenu.enabled = true;
        pauseMenu.enabled = false;
    }

    public void GoToPause () {
        pauseMenu.enabled = true;
        controlsMenu.enabled = false;
    }

    public void Quit () {
        Application.Quit ();
    }

    private void CursorSettings (bool cursorVisibility, CursorLockMode cursorLockState) {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockState;
    }
}