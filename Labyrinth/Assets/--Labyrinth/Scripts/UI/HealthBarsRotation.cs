using UnityEngine;
public class HealthBarsRotation : MonoBehaviour
{
    [SerializeField] Transform mainCamera;
    [SerializeField] Canvas healthBarCanvas;
    void Update()
    {
        //Constantly turn the health bars to face the Player
        healthBarCanvas.transform.LookAt(mainCamera.position, Vector3.up);
    }
}
