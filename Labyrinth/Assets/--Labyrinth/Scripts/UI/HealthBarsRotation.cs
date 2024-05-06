using UnityEngine;

public class HealthBarsRotation : MonoBehaviour
{
    [SerializeField] Transform mainCamera;
    [SerializeField] Canvas healthBarCanvas;

    // Update is called once per frame
    void Update()
    {
        healthBarCanvas.transform.LookAt(mainCamera.position, Vector3.up);
    }
}
