using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScript : MonoBehaviour {
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas controlsMenu;

    [SerializeField] Canvas creditsMenu;
    //[SerializeField] Canvas pauseMenu;

    void Start () {
        mainMenu.enabled = true;
        controlsMenu.enabled = false;
        creditsMenu.enabled = false;
        //pauseMenu.enabled = false;
        CursorSettings (true, CursorLockMode.Confined);
    }

    public void GoToLevel () {
        SceneManager.LoadScene (1);
        Time.timeScale = 1;
    }

    public void GoToControls () {
        controlsMenu.enabled = true;
        mainMenu.enabled = false;
    }

    public void GoToCredits () {
        creditsMenu.enabled = true;
        mainMenu.enabled = false;
    }

    public void GoToMenu () {
        mainMenu.enabled = true;
        controlsMenu.enabled = false;
        creditsMenu.enabled = false;
    }

    public void Quit () {
        Application.Quit ();
    }

    private void CursorSettings (bool cursorVisibility, CursorLockMode cursorLockState) {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockState;
    }
}