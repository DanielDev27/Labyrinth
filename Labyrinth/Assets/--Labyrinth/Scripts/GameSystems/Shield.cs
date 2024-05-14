using UnityEngine;
public class Shield : MonoBehaviour
{
    [SerializeField] Collider shieldCollider;
    void Start()
    {
        DisableShield();
    }
    //Enable Shield
    public void EnableShield() { shieldCollider.enabled = true; }
    //Disable Shield
    public void DisableShield() { shieldCollider.enabled = false; }
}
