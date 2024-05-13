using UnityEngine;
public class Weapon : MonoBehaviour
{
    public int damage;
    [SerializeField] Collider hurtBox;
    [Header("Weapon Settings")]
    [SerializeField] bool charcterWeapon;
    [SerializeField] bool aIWeapon;
    private void Start()
    {
        DisableHurtBox();
    }
    public void EnableHurtBox()
    {
        hurtBox.enabled = true;
    }
    public void DisableHurtBox()
    {
        hurtBox.enabled = false;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (charcterWeapon)
        {
            if (other.gameObject.GetComponent<AIBehaviour>() != null && hurtBox.enabled == true)
            {
                AIBehaviour ai = other.gameObject.GetComponent<AIBehaviour>();
                ai.GetComponent<HealthSystem>().TakeDamage(damage);
                DisableHurtBox();
            }
        }
        if (aIWeapon)
        {
            if (other.gameObject.GetComponent<PlayerController>() != null && hurtBox.enabled == true)
            {
                PlayerController _char = other.gameObject.GetComponent<PlayerController>();
                if (_char.IsBlock)
                {
                    DisableHurtBox();
                }
                else
                {
                    _char.GetComponent<HealthSystem>().TakeDamage(damage);
                    DisableHurtBox();
                }
            }
        }
    }
}