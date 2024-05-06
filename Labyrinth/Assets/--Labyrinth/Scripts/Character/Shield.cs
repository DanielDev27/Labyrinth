using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] Collider shieldCollider;
    // Start is called before the first frame update
    void Start()
    {
        DisableShield();
    }

    public void EnableShield() { shieldCollider.enabled = true; }
    public void DisableShield() { shieldCollider.enabled = false; }
}
