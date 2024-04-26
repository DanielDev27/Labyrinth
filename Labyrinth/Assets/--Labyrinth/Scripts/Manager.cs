using UnityEngine;

public class Manager : MonoBehaviour {
    [SerializeField] Camera camera1;
    [SerializeField] Camera camera2;
    [SerializeField] CharacterController characterController;
    [SerializeField] Canvas endFail;
    [SerializeField] Canvas endWin;
    [SerializeField] GameObject aiParent;

    void Awake () {
        Time.timeScale = 1;
        endFail.enabled = false;
        endWin.enabled = false;
    }

    void Update () {
        if (characterController.Health <= 0) {
            camera2.gameObject.SetActive (true);
            endFail.enabled = true;
            CursorSettings (true, CursorLockMode.Confined);
            Time.timeScale = 0;
        }

        if (aiParent.gameObject.GetComponentsInChildren<AIBehaviour> ().Length <= 0) {
            endWin.enabled = true;
            CursorSettings (true, CursorLockMode.Confined);
            Time.timeScale = 0;
        }
    }

    void CursorSettings (bool cursorVisibility, CursorLockMode cursorLockMode) {
        Cursor.visible = cursorVisibility;
        Cursor.lockState = cursorLockMode;
    }
}